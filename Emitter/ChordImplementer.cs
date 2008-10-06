using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using Fmacj.Runtime;
using Fmacj.Framework;

namespace Fmacj.Emitter
{
    internal class ChordImplementer
    {
        private readonly TypeBuilder target;
        private readonly Type baseType;

        private readonly Dictionary<ChordInfo, MethodBuilder> callbacks = new Dictionary<ChordInfo, MethodBuilder>();


        public ChordImplementer(TypeBuilder target, Type baseType)
        {
            this.target = target;
            this.baseType = baseType;
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

            ILGenerator generator = callback.GetILGenerator();

            // IL: Receive from Bus and store array
            generator.DeclareLocal(typeof(object[]));
            generator.Emit(OpCodes.Ldarg_1);
            generator.EmitCall(OpCodes.Call, typeof (Bus).GetMethod("Receive", new Type[] {}), null);
            generator.Emit(OpCodes.Stloc_0);
            
            generator.Emit(OpCodes.Ldarg_0);

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

            // Reregister bus
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldftn, callback);
            generator.Emit(OpCodes.Newobj, typeof(WaitOrTimerCallback).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }));
            generator.EmitCall(OpCodes.Call, typeof(ChordManager).GetMethod("RegisterBus", new Type[] { typeof(Bus), typeof(WaitOrTimerCallback) }), null);


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
                    generator.Emit(OpCodes.Ldstr, chord.ChannelNames[parameterIndex]);
                    generator.EmitCall(OpCodes.Call,
                                       typeof(ChannelFactory<>)
                                           .MakeGenericType(new Type[]
                                                            {
                                                                chord.ChannelParameters[parameterIndex].
                                                                    ParameterType
                                                            })
                                           .GetMethod("GetChannel", new Type[]
                                                                    {
                                                                        typeof (IParallelizable),
                                                                        typeof (string)
                                                                    }
                                           ), null);

                    generator.Emit(OpCodes.Stelem_Ref);
                }


                // IL: Register chord
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldftn, callbacks[chord]);
                generator.Emit(OpCodes.Newobj, typeof(WaitOrTimerCallback).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }));
                generator.EmitCall(OpCodes.Call, typeof(ChordManager).GetMethod("RegisterChord", new Type[] { typeof(IChannel[]), typeof(WaitOrTimerCallback) }), null);
            }

            generator.Emit(OpCodes.Ret);
        }
    }
}
