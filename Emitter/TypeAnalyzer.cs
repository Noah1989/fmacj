using System;
using System.Collections.Generic;
using System.Reflection;
using Fmacj.Framework;

namespace Fmacj.Emitter
{
    internal class TypeAnalyzer
    {
        private readonly Type source;

        public TypeAnalyzer(Type source)
        {
            this.source = source;
        }

        public IEnumerable<FutureGroup> GetFutureGroups()
        {
            foreach (MethodInfo futureMethod in GetFutureMethods())
            {
                if (!futureMethod.IsAbstract && !IsParallel(futureMethod))
                    throw new InvalidMethodException(source.Name, futureMethod.Name,
                                                     "The [Future] method is not abstract and not a channellless future group.");

                yield return new FutureGroup(source, futureMethod, GetParallelMethod(futureMethod));
            }
        }

        private IEnumerable<MethodInfo> GetFutureMethods()
        {
            foreach (MethodInfo method in
                source.GetMethods(BindingFlags.Instance | BindingFlags.Static |
                                  BindingFlags.Public | BindingFlags.NonPublic))
                if (method.GetCustomAttributes(typeof(FutureAttribute), false).Length > 0)
                    yield return method;
        }

        private MethodInfo GetParallelMethod(MethodInfo futureMethod)
        {
            MethodInfo result = null;
            MethodInfo[] methods =
                source.GetMethods(BindingFlags.Instance | BindingFlags.Static |
                                  BindingFlags.Public | BindingFlags.NonPublic);
            foreach (MethodInfo method in methods)
                if ((IsParallel(method))
                    && SignatureMatch(futureMethod, method))
                {
                    if (result == null)
                        result = method;
                    else
                        throw new InvalidMethodException(source.Name, futureMethod.ToString(),
                                                         "There is more than one parallel method which has a nonchannel parameter signature that matches this [Future] method.");

                }

            if (result == null)
                throw new InvalidMethodException(source.Name, futureMethod.ToString(),
                                                 "There is no parallel method which has a nonchannel parameter signature that matches this [Future] method.");

            return result;
        }

        private static bool IsParallel(MethodInfo method)
        {
            return method.GetCustomAttributes(typeof(AsynchronousAttribute), false).Length > 0
                   || method.GetCustomAttributes(typeof(MovableAttribute), false).Length > 0;
        }

        private static bool SignatureMatch(MethodInfo futureMethod , MethodInfo parallelMethod)
        {
            if (futureMethod.Name != parallelMethod.Name)
                return false;

            IEnumerable<ParameterInfo> parallelMethodParameters = GetNonChannelParameters(parallelMethod);
            IEnumerator<ParameterInfo> parallelMethodParameterEnumerator = parallelMethodParameters.GetEnumerator();

            foreach (ParameterInfo futureMethodParameter in futureMethod.GetParameters())
            {
                if(!parallelMethodParameterEnumerator.MoveNext())
                    return false;

                ParameterInfo parallelMethodParameter = parallelMethodParameterEnumerator.Current;
                
                if(futureMethodParameter.ParameterType != parallelMethodParameter.ParameterType)
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