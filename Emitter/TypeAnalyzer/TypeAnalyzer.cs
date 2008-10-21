using System;
using System.Collections.Generic;
using System.Reflection;
using Fmacj.Framework;

namespace Fmacj.Emitter
{
    internal partial class TypeAnalyzer
    {
        private readonly Type source;

        public TypeAnalyzer(Type source)
        {
            this.source = source;
        }
		
        private static bool SignatureMatch(MethodInfo forkMethod , MethodInfo parallelMethod)
        {
            if (forkMethod.Name != parallelMethod.Name)
                return false;

            IEnumerable<ParameterInfo> parallelMethodParameters = GetNonChannelParameters(parallelMethod);
            IEnumerator<ParameterInfo> parallelMethodParameterEnumerator = parallelMethodParameters.GetEnumerator();

            foreach (ParameterInfo forkMethodParameter in forkMethod.GetParameters())
            {
                if(!parallelMethodParameterEnumerator.MoveNext())
                    return false;

                ParameterInfo parallelMethodParameter = parallelMethodParameterEnumerator.Current;
                
                if(forkMethodParameter.ParameterType != parallelMethodParameter.ParameterType)
                    return false;
            }

            if (parallelMethodParameterEnumerator.MoveNext())
                return false;

        
            return true;
        }

        private static IEnumerable<ParameterInfo> GetNonChannelParameters(MethodInfo method)
        {
            foreach (ParameterInfo parameter in method.GetParameters())
                if(parameter.GetCustomAttributes(typeof(ChannelAttribute),false).Length == 0)
                    yield return parameter;
        }
    }
}