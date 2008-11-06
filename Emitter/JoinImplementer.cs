using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Fmacj.Runtime;

namespace Fmacj.Emitter
{
    internal class JoinImplementer
    {
        private readonly TypeBuilder target;
		private readonly ChannelImplementer channelImplementer;
  
        public JoinImplementer(TypeBuilder target, ChannelImplementer channelImplementer)
        {
            this.target = target;
			this.channelImplementer = channelImplementer;
        }

        public void Implement(JoinGroup joinGroup)
        {
            target.DefineMethodOverride(GetBody(joinGroup), joinGroup.JoinMethod);
        }
		
		private MethodBuilder GetBody(JoinGroup joinGroup)
        {
			Type returnType = joinGroup.JoinMethod.ReturnType;
			
            MethodBuilder result = target.DefineMethod(joinGroup.Name, MethodAttributes.Virtual | MethodAttributes.Public,
                                                       returnType, new Type[] {} );            

            ILGenerator generator = result.GetILGenerator();
			
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldfld, channelImplementer
			               .GetJoinChannelField(joinGroup.Name,
			                                    returnType));
				
			generator.EmitCall(OpCodes.Call, typeof(Channel<>)
			                   .MakeGenericType(new Type[] { returnType })
			                   .GetMethod("Receive"), null);
			
			generator.Emit(OpCodes.Ret);

            return result;

        }
    }
}
