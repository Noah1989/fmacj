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
	internal class YieldImplementer
	{		
		private readonly TypeBuilder target;
		private readonly ChannelImplementer channelImplementer;

		public YieldImplementer(TypeBuilder target, ChannelImplementer channelImplementer)
		{
			this.target = target;            
			this.channelImplementer = channelImplementer;
		}

		public void Implement(YieldInfo yieldInfo)
        {
            target.DefineMethodOverride(GetBody(yieldInfo), yieldInfo.YieldMethod);
        }

		private MethodBuilder GetBody(YieldInfo yieldInfo)
        {
         	MethodBuilder result = target.DefineMethod(yieldInfo.Name, MethodAttributes.Virtual | MethodAttributes.Public,
                                                       typeof(void), yieldInfo.ParameterTypes);
			int parameterCount = yieldInfo.Parameters.Length;
			
            ILGenerator generator = result.GetILGenerator();

			for (int parameterIndex = 0; parameterIndex < parameterCount; parameterIndex++)
            {
            	// Parameters are to be attributed and named nicely
                ParameterInfo parameter = yieldInfo.Parameters[parameterIndex];
                result.DefineParameter(parameterIndex + 1, parameter.Attributes, parameter.Name);

				// Check for ChannelAttribute
               	if (parameter.GetCustomAttributes(typeof(ChannelAttribute), false).Length != 1)
                	throw new InvalidMethodException(target.Name, yieldInfo.Name, 
					                                 "All [Yield] method parameters must have exactly one [Channel] attribute.");

				// Feed value into channel
				ChannelAttribute channelAttribute 
					= parameter.GetCustomAttributes(typeof(ChannelAttribute), false)[0] 
					as ChannelAttribute;
				
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, channelImplementer
				               .GetChannelField(channelAttribute.Name, parameter.ParameterType));

                generator.Emit(OpCodes.Ldarg, parameterIndex + 1);
				generator.EmitCall(OpCodes.Call, typeof(Channel<>)
				                   .MakeGenericType(new Type[] { parameter.ParameterType })
				                   .GetMethod("Send"), null);
				
          	}			
			
             generator.Emit(OpCodes.Ret);

            return result;
        }
	}
}
