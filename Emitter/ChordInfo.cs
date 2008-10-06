using System;
using System.Collections.Generic;
using System.Reflection;
using Fmacj.Framework;

namespace Fmacj.Emitter
{
    internal struct ChordInfo
    {
        public ChordInfo(Type sourceType, MethodInfo chordMethod): this()
        {
            SourceType = sourceType;
            ChordMethod = chordMethod;
            FindChannelParameters();
        }

        private void FindChannelParameters()
        {
            List<ParameterInfo> channelParameters = new List<ParameterInfo>();

            foreach (ParameterInfo parameter in ChordMethod.GetParameters())
            {
                if (parameter.GetCustomAttributes(typeof(ChannelAttribute), false).Length > 0)
                    channelParameters.Add(parameter);
            }

            ChannelParameters = channelParameters.ToArray();
        }

        public Type SourceType { get; private set; }
        public MethodInfo ChordMethod { get; private set; }

        public string Name { get { return ChordMethod.Name; } }
        
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
