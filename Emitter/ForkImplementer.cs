using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Fmacj.Runtime;

namespace Fmacj.Emitter
{
    internal class ForkImplementer
    {
        private readonly TypeBuilder target;

        public ForkImplementer(TypeBuilder target)
        {
            this.target = target;
        }

        public void Implement(ForkGroup forkGroup)
        {
            target.DefineMethodOverride(GetBody(forkGroup), forkGroup.ForkMethod);
        }


        private MethodBuilder GetBody(ForkGroup forkGroup)
        {
            MethodBuilder result = target.DefineMethod(forkGroup.Name, MethodAttributes.Virtual | MethodAttributes.Public,
                                                       typeof(void), forkGroup.ParameterTypes);
            int parameterCount = forkGroup.Parameters.Length;

            ILGenerator generator = result.GetILGenerator();

            // IL: Create argument array
            generator.DeclareLocal(typeof(object[]));
            generator.Emit(OpCodes.Ldc_I4, parameterCount);
            generator.Emit(OpCodes.Newarr, typeof(object));
            generator.Emit(OpCodes.Stloc_0);

            for (int parameterIndex = 0; parameterIndex < parameterCount; parameterIndex++)
            {
                // Parameters are to be attributed and named nicely
                ParameterInfo parameter = forkGroup.Parameters[parameterIndex];
                result.DefineParameter(parameterIndex + 1, parameter.Attributes, parameter.Name);

                // IL: Wrap up argument array
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Ldc_I4, parameterIndex);
                generator.Emit(OpCodes.Ldarg, parameterIndex + 1);
                if (forkGroup.ParameterTypes[parameterIndex].IsValueType)
                    generator.Emit(OpCodes.Box, forkGroup.ParameterTypes[parameterIndex]);
                generator.Emit(OpCodes.Stelem_Ref);
            }

            // IL: Start caller in new Thread (now using ThreadPool)
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldftn, GetCaller(forkGroup));
            generator.Emit(OpCodes.Newobj, typeof(WaitCallback).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }));
            generator.Emit(OpCodes.Ldloc_0);
            generator.EmitCall(OpCodes.Call, typeof(ThreadPool).GetMethod("QueueUserWorkItem", new Type[] { typeof(WaitCallback), typeof(object) }), null);
            generator.Emit(OpCodes.Pop);
            generator.Emit(OpCodes.Ret);

            return result;

        }

        private MethodBuilder GetCaller(ForkGroup forkGroup)
        {
            MethodBuilder result = target.DefineMethod(string.Format("{0}Caller", forkGroup.Name),
                                                       MethodAttributes.Private,
                                                       typeof(void), new Type[] { typeof(object) });
            result.DefineParameter(1, ParameterAttributes.In, "argumentArray");
            int parameterCount = forkGroup.Parameters.Length;
            int channelParameterCount = forkGroup.ChannelParameters.Length;

            ILGenerator generator = result.GetILGenerator();

            // IL: Unwrap argument array
            generator.Emit(OpCodes.Ldarg_0);
            for (int parameterIndex = 0; parameterIndex < parameterCount; parameterIndex++)
            {
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Ldc_I4, parameterIndex);
                generator.Emit(OpCodes.Ldelem_Ref);
                if (forkGroup.ParameterTypes[parameterIndex].IsValueType)
                    generator.Emit(OpCodes.Unbox_Any, forkGroup.ParameterTypes[parameterIndex]);
            }

            // IL: Prepare channels
            for (int channelParameterIndex = 0; channelParameterIndex < channelParameterCount; channelParameterIndex++)
            {
                generator.DeclareLocal(forkGroup.ChannelParameters[channelParameterIndex].ParameterType);
                generator.Emit(OpCodes.Ldloca, channelParameterIndex);
            }

            // IL: Call parallel method
            generator.EmitCall(OpCodes.Call, forkGroup.ParallelMethod, null);

            // IL: Handle channel results
            for (int channelParameterIndex = 0; channelParameterIndex < channelParameterCount; channelParameterIndex++)
            {
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldstr, forkGroup.ChannelNames[channelParameterIndex]);
                Type[] typeArguments = new Type[]
                                       {
                                           forkGroup.ChannelParameters[channelParameterIndex].
                                               ParameterType.GetElementType()
                                       };
                generator.EmitCall(OpCodes.Call,
                                   typeof(ChannelFactory<>).MakeGenericType(typeArguments).GetMethod("GetChannel"), null);

                generator.Emit(OpCodes.Ldloc, channelParameterIndex);
                generator.EmitCall(OpCodes.Call, typeof(Channel<>).MakeGenericType(typeArguments).GetMethod("Send"), null);
            }

            generator.Emit(OpCodes.Ret);

            return result;
        }
    }
}
