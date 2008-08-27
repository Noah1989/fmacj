using System;
using System.Collections.Generic;
using System.Reflection;
using Fmacj.Framework;

namespace Fmacj.Emitter
{
    internal struct FutureGroup
    {
        public FutureGroup(Type sourceType, MethodInfo futureMethod, MethodInfo parallelMethod) : this()
        {
            SourceType = sourceType;
            FutureMethod = futureMethod;
            ParallelMethod = parallelMethod;
            FindChannelParameters();
        }

        private void FindChannelParameters()
        {
            List<ParameterInfo> channelParameters = new List<ParameterInfo>();
            
            foreach (ParameterInfo parallelParameter in ParallelMethod.GetParameters())
            {
                if (parallelParameter.GetCustomAttributes(typeof(ChannelAttribute), false).Length > 0)
                    channelParameters.Add(parallelParameter);
            }

            ChannelParameters = channelParameters.ToArray();
        }

        public Type SourceType { get; private set; }
        public MethodInfo FutureMethod { get; private set; }
        public MethodInfo ParallelMethod { get; private set; }

        public string Name { get { return FutureMethod.Name; } }
        public Type[] ParameterTypes
        {
            get
            {
                ParameterInfo[] parameters = FutureMethod.GetParameters();
                Type[] result = new Type[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                    result[i] = parameters[i].ParameterType;

                return result;
            }
        }

        public ParameterInfo[] Parameters
        {
            get { return FutureMethod.GetParameters(); }
        }
       
        public ParameterInfo[] ChannelParameters
        {
            get; private set;
        }

        public string[] ChannelNames
        {
            get
            {
                string[] result = new string[ChannelParameters.Length];

                for (int i = 0; i < ChannelParameters.Length; i++)
                    result[i] =
                        ((ChannelAttribute)
                         ChannelParameters[i].GetCustomAttributes(typeof (ChannelAttribute), false)[0]).Name;

                return result;
            }
        }
    }
}
