using System;
using System.Collections.Generic;
using System.Reflection;
using Fmacj.Framework;

namespace Fmacj.Emitter
{
    internal struct ForkGroup
    {
        public ForkGroup(Type sourceType, MethodInfo forkMethod, MethodInfo parallelMethod) : this()
        {
            SourceType = sourceType;
            ForkMethod = forkMethod;
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
        public MethodInfo ForkMethod { get; private set; }
        public MethodInfo ParallelMethod { get; private set; }

        public string Name { get { return ForkMethod.Name; } }
        public Type[] ParameterTypes
        {
            get
            {
                ParameterInfo[] parameters = ForkMethod.GetParameters();
                Type[] result = new Type[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                    result[i] = parameters[i].ParameterType;

                return result;
            }
        }

        public ParameterInfo[] Parameters
        {
            get { return ForkMethod.GetParameters(); }
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
