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
using Fmacj.Runtime;
using Fmacj.Framework;

namespace Fmacj.Emitter
{
    internal class ChordImplementer
    {
        private readonly TypeBuilder target;
		private readonly ChannelImplementer channelImplementer;

        private readonly Dictionary<ChordInfo, MethodBuilder> callbacks = new Dictionary<ChordInfo, MethodBuilder>();
        
        public ChordImplementer(TypeBuilder target, ChannelImplementer channelImplementer)
        {
            this.target = target;
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

			int joinParameterCount = chord.JoinParameters.Length;
            int inChannelParameterCount = chord.InChannelParameters.Length;
            int outChannelParameterCount = chord.OutChannelParameters.Length;

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
            for (int parameterIndex = 0; parameterIndex < joinParameterCount; parameterIndex++)
            {
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Ldc_I4, parameterIndex);
                generator.Emit(OpCodes.Ldelem_Ref);
                if (chord.JoinParameters[parameterIndex].ParameterType.IsValueType)
                    generator.Emit(OpCodes.Unbox_Any, chord.JoinParameters[parameterIndex].ParameterType);
            }					
			for (int parameterIndex = 0; parameterIndex < inChannelParameterCount; parameterIndex++)
            {
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Ldc_I4, parameterIndex + joinParameterCount);
                generator.Emit(OpCodes.Ldelem_Ref);
                if (chord.InChannelParameters[parameterIndex].ParameterType.IsValueType)
                    generator.Emit(OpCodes.Unbox_Any, chord.InChannelParameters[parameterIndex].ParameterType);
            }					

			// IL: Prepare out channels
            for (int channelParameterIndex = 0; channelParameterIndex < outChannelParameterCount; channelParameterIndex++)
            {
                generator.DeclareLocal(chord.OutChannelParameters[channelParameterIndex].ParameterType.GetElementType());
                generator.Emit(OpCodes.Ldloca, channelParameterIndex + 1);
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

            // IL: Handle out channel results
            for (int channelParameterIndex = 0; channelParameterIndex < outChannelParameterCount; channelParameterIndex++)
            {
				Type channelType = chord.OutChannelParameters[channelParameterIndex]
				                                .ParameterType.GetElementType();
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, channelImplementer
				               .GetChannelField(chord.OutChannelNames[channelParameterIndex],
				                                channelType));

                generator.Emit(OpCodes.Ldloc, channelParameterIndex + 1);
				generator.EmitCall(OpCodes.Call, typeof(Channel<>)
				                   .MakeGenericType(new Type[] { channelType })
				                   .GetMethod("Send"), null);
            }

            generator.Emit(OpCodes.Ret);

            callbacks.Add(chord, callback);
        }		
				
		public Dictionary<ChordInfo, MethodBuilder> GetCallbacks()
		{
			return callbacks;
		}
    }
}