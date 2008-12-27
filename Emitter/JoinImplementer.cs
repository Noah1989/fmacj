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
