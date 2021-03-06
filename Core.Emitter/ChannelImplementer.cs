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
using Fmacj.Core.Runtime;
using Fmacj.Core.Framework;

namespace Fmacj.Core.Emitter
{	
	internal class ChannelImplementer
	{		
		private readonly TypeBuilder target;
		
		private readonly Dictionary<string, FieldInfo> channelFields =
            new Dictionary<string, FieldInfo>();
		
		public ChannelImplementer(TypeBuilder target)
		{
			this.target = target;
		}
		
		private FieldInfo GetChannelFieldInternal(string internalname, Type type)
		{
			Type channelType = typeof(Channel<>).MakeGenericType(new Type[] { type });
			
			FieldInfo result;
						
			if (!channelFields.TryGetValue(internalname, out result))
            {
				result = target.DefineField(internalname, 
				                            channelType,
				                            FieldAttributes.Private);
				
                channelFields.Add(internalname, result);
            }
			else
				if (result.FieldType != channelType)
					throw new InconsistentChannelTypeException(internalname, target.Name);
				
			

            return result;
		}
		
		public FieldInfo GetChannelField(string name, Type type)
		{   
			return GetChannelFieldInternal(name + "Channel", type);
        }
		
		public FieldInfo GetJoinChannelField(string name, Type type)
		{
			return GetChannelFieldInternal(name + "ChannelJ", type);
		}
		
		public FieldInfo GetJoinParameterChannelField(string joinName, int parameterIndex, Type type)
		{
			return GetChannelFieldInternal(joinName + "ChannelJP" + parameterIndex, type);
		}
		
		public void ImplementChannelInitialization(ILGenerator generator)
		{
			foreach (FieldInfo field in channelFields.Values)
			{
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Newobj, field.FieldType.GetConstructor(new Type[] {}));
				generator.Emit(OpCodes.Stfld, field);
			}
		}
	}
}
