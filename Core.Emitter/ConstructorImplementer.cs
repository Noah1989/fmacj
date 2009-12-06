/*
    FMACJ Parallelization Framework for .NET
    Copyright (C) 2008  Stefan Noack

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Fmacj.Core.Runtime;
using Fmacj.Core.Framework;

namespace Fmacj.Core.Emitter
{	
	internal class ConstructorImplementer
	{		
		private readonly TypeBuilder target;
        private readonly Type baseType;
		private readonly ChannelImplementer channelImplementer;
		private readonly ChordImplementer chordImplementer;
		private readonly DisposeImplementer disposeImplementer;
		
		public ConstructorImplementer(TypeBuilder target, Type baseType,
		                              ChannelImplementer channelImplementer, 
		                              ChordImplementer chordImplementer,
		                              DisposeImplementer disposeImplementer)
        {
            this.target = target;
            this.baseType = baseType;
			this.channelImplementer = channelImplementer;
			this.chordImplementer = chordImplementer;
			this.disposeImplementer = disposeImplementer;
        }
		
        public void ImplementConstructor()
        {
            ConstructorBuilder ctor = target.DefineConstructor(MethodAttributes.Public, 
			                                                   CallingConventions.HasThis,
                                                               new Type[] {});

            ILGenerator generator = ctor.GetILGenerator();

            generator.DeclareLocal(typeof (IChannel[]));
			generator.DeclareLocal(typeof (ChannelOptions[]));

            // IL: Call base constructor
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Call, baseType.GetConstructor(BindingFlags.Instance | BindingFlags.Public
                                                                  | BindingFlags.NonPublic, null, new Type[] { }, null));
           
			channelImplementer.ImplementChannelInitialization(generator);

			Dictionary<ChordInfo, MethodBuilder> callbacks = chordImplementer.GetCallbacks();
			
            foreach (ChordInfo chord in callbacks.Keys)
            {
				int joinParameterCount = chord.JoinParameters.Length;
				int inChannelParameterCount = chord.InChannelParameters.Length;
                int totalParameterCount = joinParameterCount + inChannelParameterCount;

                // IL: Create channel array
                generator.Emit(OpCodes.Ldc_I4, totalParameterCount);
                generator.Emit(OpCodes.Newarr, typeof (IChannel));
                generator.Emit(OpCodes.Stloc_0);
				
				 // IL: Create channel options array
                generator.Emit(OpCodes.Ldc_I4, totalParameterCount);
                generator.Emit(OpCodes.Newarr, typeof (ChannelOptions));
                generator.Emit(OpCodes.Stloc_1);

				// Join Parameter Channels
				for (int parameterIndex = 0; parameterIndex < joinParameterCount; parameterIndex++)
                {
                    // IL: Wrap up channel array
                    generator.Emit(OpCodes.Ldloc_0);
                    generator.Emit(OpCodes.Ldc_I4, parameterIndex);

                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldfld, channelImplementer
					               .GetJoinParameterChannelField(chord.Name, parameterIndex,
					                                             chord.JoinParameters[parameterIndex].ParameterType));
					
                    generator.Emit(OpCodes.Stelem_Ref);
					
					// IL: Wrap up channel options array
					generator.Emit(OpCodes.Ldloc_1);
                    generator.Emit(OpCodes.Ldc_I4, parameterIndex);
					
					generator.Emit(OpCodes.Ldc_I4, Convert.ToInt32(false));
					generator.Emit(OpCodes.Newobj, typeof(ChannelOptions).GetConstructor(new Type[] { typeof(bool) }));
			
					generator.Emit(OpCodes.Stelem_Ref);
                }
				
				// In Channels
                for (int parameterIndex = 0; parameterIndex < inChannelParameterCount; parameterIndex++)
                {
                    // IL: Wrap up channel array
                    generator.Emit(OpCodes.Ldloc_0);
                    generator.Emit(OpCodes.Ldc_I4, parameterIndex + joinParameterCount);

                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldfld, channelImplementer
					               .GetChannelField(chord.InChannelNames[parameterIndex],
					                                chord.InChannelAttributes[parameterIndex].Enumerable 
					                                ? chord.GetEnumerableInChannelType(parameterIndex)
					                                : chord.InChannelParameters[parameterIndex].ParameterType));

                    generator.Emit(OpCodes.Stelem_Ref);
					
					// IL: Wrap up channel options array
					generator.Emit(OpCodes.Ldloc_1);
                    generator.Emit(OpCodes.Ldc_I4, parameterIndex + joinParameterCount);
					
					generator.Emit(OpCodes.Ldc_I4, Convert.ToInt32(chord.InChannelAttributes[parameterIndex].Enumerable));
					generator.Emit(OpCodes.Newobj, typeof(ChannelOptions).GetConstructor(new Type[] { typeof(bool) }));
			
					generator.Emit(OpCodes.Stelem_Ref);
                }


                // IL: Register chord
                generator.Emit(OpCodes.Ldloc_0);
				generator.Emit(OpCodes.Ldloc_1);
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldftn, callbacks[chord]);
                generator.Emit(OpCodes.Newobj, typeof(WaitOrTimerCallback).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }));
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldflda, disposeImplementer.GetDisposingEventHandlerField());
                generator.EmitCall(OpCodes.Call,
                                   typeof(ChordManager).GetMethod("RegisterChord",
                                                                  new Type[]
                                                                      {
                                                                          typeof(IChannel[]),
					                                                      typeof(ChannelOptions[]),
                                                                          typeof(WaitOrTimerCallback),
                                                                          typeof(EventHandler).MakeByRefType()
                                                                      }), null);
            }

            generator.Emit(OpCodes.Ret);
        }				
	}
}