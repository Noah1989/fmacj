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

namespace Fmacj.Emitter
{	
	internal class DisposeImplementer
	{		
		private FieldBuilder disposingEventHandlerField;
        private FieldBuilder disposedField;
		
		private readonly TypeBuilder target;
        private readonly Type baseType;
		
		public DisposeImplementer(TypeBuilder target, Type baseType)
        {
            this.target = target;
            this.baseType = baseType;
        }
		
		public void ImplementDisposalBehavior()
        {
			// IL: Prepare "disposingEventHandler" and "disposed" fields
            disposingEventHandlerField = target.DefineField("disposingEventHandler", typeof (EventHandler), FieldAttributes.Private);
            disposedField = target.DefineField("disposed", typeof(bool), FieldAttributes.Private);
			
            target.DefineMethodOverride(GetDisposeMethodBody(), typeof(IDisposable).GetMethod("Dispose", new Type[] {}));            
        }

        private MethodInfo GetDisposeMethodBody()
        {
            MethodBuilder result = target.DefineMethod("Dispose",
                                                              MethodAttributes.Virtual | MethodAttributes.Public,
                                                              typeof(void), new Type[] { });

            ILGenerator generator = result.GetILGenerator();

            Label returnLabel = generator.DefineLabel();

            // IL: If disposed, return
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, disposedField);
            generator.Emit(OpCodes.Brtrue, returnLabel);

			// IL: Call base.Dispose()
		    MethodInfo baseDispose = baseType.GetMethod("Dispose",BindingFlags.Instance | BindingFlags.Public
                                                     	| BindingFlags.NonPublic, null, new Type[] { }, null);
			if(!baseDispose.IsAbstract)
			{
            	generator.Emit(OpCodes.Ldarg_0);
            	generator.Emit(OpCodes.Call, baseDispose);
			}
					
            // IL: Set disposed = true
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldc_I4_1);
            generator.Emit(OpCodes.Stfld, disposedField);

            // IL: GC.SuppressFinalize(this)
            generator.Emit(OpCodes.Ldarg_0);
            generator.EmitCall(OpCodes.Call,
                               typeof (GC).GetMethod("SuppressFinalize", new Type[] {typeof (object)}),
                               null);

            // IL: If eventHandler == null, return
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, disposingEventHandlerField);
            generator.Emit(OpCodes.Brfalse, returnLabel);

            // IL: Invoke disposingEventHandler
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
            
            // IL: Return
            generator.MarkLabel(returnLabel);
            generator.Emit(OpCodes.Ret);

            return result;
        }
			
	    public FieldInfo GetDisposingEventHandlerField()
	    {
			return disposingEventHandlerField;
		}
	}
}