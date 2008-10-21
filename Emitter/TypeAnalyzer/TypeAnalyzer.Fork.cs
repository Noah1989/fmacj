using System;
using System.Collections.Generic;
using System.Reflection;
using Fmacj.Framework;

namespace Fmacj.Emitter
{
    internal partial class TypeAnalyzer
    {
		public IEnumerable<ForkGroup> GetForkGroups()
        {
            foreach (MethodInfo forkMethod in GetForkMethods())
            {
                if (!forkMethod.IsAbstract && !IsParallel(forkMethod))
                    throw new InvalidMethodException(source.Name, forkMethod.Name,
                                                     "The [Fork] method is not abstract and not a channellless fork group.");

                yield return new ForkGroup(source, forkMethod, GetParallelMethod(forkMethod));
            }
        }

        private IEnumerable<MethodInfo> GetForkMethods()
        {
            foreach (MethodInfo method in
                source.GetMethods(BindingFlags.Instance | BindingFlags.Static |
                                  BindingFlags.Public | BindingFlags.NonPublic))
                if (method.GetCustomAttributes(typeof(ForkAttribute), false).Length > 0)
                    yield return method;
        }

        private MethodInfo GetParallelMethod(MethodInfo forkMethod)
        {
            MethodInfo result = null;
            MethodInfo[] methods =
                source.GetMethods(BindingFlags.Instance | BindingFlags.Static |
                                  BindingFlags.Public | BindingFlags.NonPublic);
            foreach (MethodInfo method in methods)
                if ((IsParallel(method))
                    && SignatureMatch(forkMethod, method))
                {
                    if (result == null)
                        result = method;
                    else
                        throw new InvalidMethodException(source.Name, forkMethod.ToString(),
                                                         "There is more than one parallel method which has a nonchannel parameter signature that matches this [Fork] method.");

                }

            if (result == null)
                throw new InvalidMethodException(source.Name, forkMethod.ToString(),
                                                 "There is no parallel method which has a nonchannel parameter signature that matches this [Fork] method.");

            return result;
        }

        private static bool IsParallel(MethodInfo method)
        {
            return method.GetCustomAttributes(typeof(AsynchronousAttribute), false).Length > 0
                   || method.GetCustomAttributes(typeof(MovableAttribute), false).Length > 0;
        }
	}
}