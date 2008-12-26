using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Fmacj.Runtime;
using Fmacj.Framework;

namespace Fmacj.Emitter
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
