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
		
		public FieldInfo GetChannelField(string name, Type type)
		{   

			Type channelType = typeof(Channel<>).MakeGenericType(new Type[] { type });
			
			FieldInfo result;
						
			if (!channelFields.TryGetValue(name, out result))
            {
				result = target.DefineField(name + "Channel", 
				                            channelType,
				                            FieldAttributes.Private);
				
                channelFields.Add(name, result);
            }
			else
				if (result.FieldType != channelType)
					throw new InconsistentChannelTypeException(name, target.Name);
				
			

            return result;
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
