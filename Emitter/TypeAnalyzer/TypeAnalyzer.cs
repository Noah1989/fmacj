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
		
        private static bool SignatureMatch(MethodInfo shortMethod , MethodInfo longMethod)
        {
            if (shortMethod.Name != longMethod.Name)
                return false;
			
			if (shortMethod.ReturnType != longMethod.ReturnType)
                return false;

            IEnumerable<ParameterInfo> longMethodParameters = GetNonChannelParameters(longMethod);
            IEnumerator<ParameterInfo> longMethodParameterEnumerator = longMethodParameters.GetEnumerator();

            foreach (ParameterInfo shortMethodParameter in shortMethod.GetParameters())
            {
                if(!longMethodParameterEnumerator.MoveNext())
                    return false;

                ParameterInfo longMethodParameter = longMethodParameterEnumerator.Current;
                
                if(shortMethodParameter.ParameterType != longMethodParameter.ParameterType)
                    return false;
            }

            if (longMethodParameterEnumerator.MoveNext())
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