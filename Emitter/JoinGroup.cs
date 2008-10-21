using System;
using System.Collections.Generic;
using System.Reflection;
using Fmacj.Framework;

namespace Fmacj.Emitter
{
    internal struct JoinGroup
    {
        public JoinGroup(Type sourceType, MethodInfo joinMethod, MethodInfo chordMethod) : this()
        {
            SourceType = sourceType;
            JoinMethod = joinMethod;
            ChordMethod = chordMethod;
        }

        public Type SourceType { get; private set; }
        public MethodInfo JoinMethod { get; private set; }
        public MethodInfo ChordMethod { get; private set; }

        public string Name { get { return JoinMethod.Name; } }
        public Type[] ParameterTypes
        {
            get
            {
                ParameterInfo[] parameters = JoinMethod.GetParameters();
                Type[] result = new Type[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                    result[i] = parameters[i].ParameterType;

                return result;
            }
        }

        public ParameterInfo[] Parameters
        {
            get { return JoinMethod.GetParameters(); }
        }
    }
}
