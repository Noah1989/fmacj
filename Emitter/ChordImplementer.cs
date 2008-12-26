using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Fmacj.Runtime;
using Fmacj.Framework;

namespace Fmacj.Emitter
{
    internal class ChordImplementer
    {
        private readonly TypeBuilder target;
        private readonly Type baseType;
		private readonly ChannelImplementer channelImplementer;

        private readonly Dictionary<ChordInfo, MethodBuilder> callbacks = new Dictionary<ChordInfo, MethodBuilder>();
        
        private FieldBuilder disposingEventHandlerField;
        private FieldBuilder disposedField;

        public ChordImplementer(TypeBuilder target, Type baseType, ChannelImplementer channelImplementer)
        {
            this.target = target;
            this.baseType = baseType;
			this.channelImplementer = channelImplementer;
        }

        public void Implement(ChordInfo chord)
        {
            ImplementCallback(chord);
        }



        private void ImplementCallback(ChordInfo chord)
        {
            MethodBuilder callback = target.DefineMethod(string.Format("{0}Callback", chord.Name),
                                             MethodAttributes.Private, typeof (void),
                                             new Type[] {typeof (object), typeof (bool)});

            callback.DefineParameter(1, ParameterAttributes.In, "bus");
            callback.DefineParameter(2, ParameterAttributes.In, "unused");

            int channelParameterCount = chord.ChannelParameters.Length;

			Type returnType = chord.ChordMethod.ReturnType;		
			
            ILGenerator generator = callback.GetILGenerator();

			// IL: Get join channel			
			if (returnType != typeof(void))
			{
                generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldfld, channelImplementer
				               .GetJoinChannelField(chord.ChordMethod.Name,
				                                    returnType));
			}
			
            // IL: Receive from Bus and store array
            generator.DeclareLocal(typeof(object[]));
            generator.Emit(OpCodes.Ldarg_1);
            generator.EmitCall(OpCodes.Call, typeof (Bus).GetMethod("Receive", new Type[] {}), null);
            generator.Emit(OpCodes.Stloc_0);
            
            generator.Emit(OpCodes.Ldarg_0);
			
			// IL: Reregister bus
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldftn, callback);
            generator.Emit(OpCodes.Newobj, typeof(WaitOrTimerCallback).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }));
            generator.EmitCall(OpCodes.Call, typeof(ChordManager).GetMethod("RegisterBus", new Type[] { typeof(Bus), typeof(WaitOrTimerCallback) }), null);

            // IL: Unwrap value array
            for (int parameterIndex = 0; parameterIndex < channelParameterCount; parameterIndex++)
            {
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Ldc_I4, parameterIndex);
                generator.Emit(OpCodes.Ldelem_Ref);
                if (chord.ChannelParameters[parameterIndex].ParameterType.IsValueType)
                    generator.Emit(OpCodes.Unbox_Any, chord.ChannelParameters[parameterIndex].ParameterType);
            }					
		
			// IL: Call chord method			
            generator.EmitCall(OpCodes.Call, chord.ChordMethod, null);
			
			// IL: Send result to join channel			
			if (returnType != typeof(void))
			{							
				generator.EmitCall(OpCodes.Call, typeof(Channel<>)
				                   .MakeGenericType(new Type[] { returnType })
				                   .GetMethod("Send"), null);
			}            


            generator.Emit(OpCodes.Ret);

            callbacks.Add(chord, callback);
        }


        public void ImplementConstructor()
        {
            ConstructorBuilder ctor = target.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis,
                                                               new Type[] {});

            ILGenerator generator = ctor.GetILGenerator();

            generator.DeclareLocal(typeof (object[]));

            // IL: Call base constructor
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Call, baseType.GetConstructor(BindingFlags.Instance | BindingFlags.Public
                                                                  | BindingFlags.NonPublic, null, new Type[] { }, null));

            disposingEventHandlerField = target.DefineField("disposingEventHandler", typeof (EventHandler), FieldAttributes.Private);
            disposedField = target.DefineField("disposed", typeof(bool), FieldAttributes.Private);
            
			channelImplementer.ImplementChannelInitialization(generator);

            foreach (ChordInfo chord in callbacks.Keys)
            {
                int parameterCount = chord.ChannelParameters.Length;

                // IL: Create channel array
                generator.Emit(OpCodes.Ldc_I4, parameterCount);
                generator.Emit(OpCodes.Newarr, typeof(IChannel));
                generator.Emit(OpCodes.Stloc_0);

                for (int parameterIndex = 0; parameterIndex < parameterCount; parameterIndex++)
                {
                    // IL: Wrap up channel array
                    generator.Emit(OpCodes.Ldloc_0);
                    generator.Emit(OpCodes.Ldc_I4, parameterIndex);

                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldfld, channelImplementer
					               .GetChannelField(chord.ChannelNames[parameterIndex],
					                                chord.ChannelParameters[parameterIndex].ParameterType));

                    generator.Emit(OpCodes.Stelem_Ref);
                }


                // IL: Register chord
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldftn, callbacks[chord]);
                generator.Emit(OpCodes.Newobj, typeof(WaitOrTimerCallback).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }));
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldflda, disposingEventHandlerField);
                generator.EmitCall(OpCodes.Call,
                                   typeof (ChordManager).GetMethod("RegisterChord",
                                                                   new Type[]
                                                                       {
                                                                           typeof (IChannel[]),
                                                                           typeof (WaitOrTimerCallback),
                                                                           typeof (EventHandler).MakeByRefType()
                                                                       }), null);
            }

            generator.Emit(OpCodes.Ret);
        }


        public void ImplementDisposalBehavior()
        {
            target.DefineMethodOverride(GetDisposeMethodBody(), typeof(IDisposable).GetMethod("Dispose", new Type[] {}));
            target.DefineMethodOverride(GetFinalizeMethodBody(),
                                        typeof (object).GetMethod("Finalize",
                                                                  BindingFlags.Instance | BindingFlags.NonPublic));
        }

        private MethodInfo GetDisposeMethodBody()
        {
            MethodBuilder result = target.DefineMethod("Dispose",
                                                              MethodAttributes.Virtual | MethodAttributes.Public,
                                                              typeof(void), new Type[] { });

            ILGenerator generator = result.GetILGenerator();

            Label returnLabel = generator.DefineLabel();

            // If disposed, return
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, disposedField);
            generator.Emit(OpCodes.Brtrue, returnLabel);

            // Set disposed = true
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldc_I4_1);
            generator.Emit(OpCodes.Stfld, disposedField);

            // GC.SuppressFinalize(this)
            generator.Emit(OpCodes.Ldarg_0);
            generator.EmitCall(OpCodes.Call,
                               typeof (GC).GetMethod("SuppressFinalize", new Type[] {typeof (object)}),
                               null);

            // If eventHandler == null, return
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, disposingEventHandlerField);
            generator.Emit(OpCodes.Brfalse, returnLabel);

            // Invoke eventHandler
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, disposingEventHandlerField);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Newobj, typeof (EventArgs).GetConstructor(new Type[] {}));
            generator.EmitCall(OpCodes.Call,
                               typeof(EventHandler).GetMethod("Invoke",
                                                               new Type[]
                                                                   {
                                                                       typeof (object),
                                                                       typeof (EventArgs)
                                                                   }
                                   ), null);
            
            // Return
            generator.MarkLabel(returnLabel);
            generator.Emit(OpCodes.Ret);

            return result;
        }


        private MethodInfo GetFinalizeMethodBody()
        {
            MethodBuilder result = target.DefineMethod("Finalize",
                                                              MethodAttributes.Virtual | MethodAttributes.Family,
                                                              typeof(void), new Type[] { });

            ILGenerator generator = result.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);
            generator.EmitCall(OpCodes.Call, typeof(IDisposable).GetMethod("Dispose", new Type[] {}), null);
            generator.Emit(OpCodes.Ret);

            return result;
        }

    }
}
