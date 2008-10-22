using System;
using System.Collections.Generic;
using System.Reflection;
using Fmacj.Framework;

namespace Fmacj.Emitter
{
    internal partial class TypeAnalyzer
    {
		public IEnumerable<JoinGroup> GetJoinGroups()
        {
            foreach (MethodInfo joinMethod in GetJoinMethods())
            {
                if (!joinMethod.IsAbstract)
                    throw new InvalidMethodException(source.Name, joinMethod.Name,
                                                     "The [Join] method is not abstract.");
				if (joinMethod.ReturnType == typeof(void))
                    throw new InvalidMethodException(source.Name, joinMethod.Name,
                                                     "The [Join] method has void return type.");

                yield return new JoinGroup(source, joinMethod, GetChordMethod(joinMethod));
            }
        }

        private IEnumerable<MethodInfo> GetJoinMethods()
        {
            foreach (MethodInfo method in
                source.GetMethods(BindingFlags.Instance | BindingFlags.Static |
                                  BindingFlags.Public | BindingFlags.NonPublic))
                if (method.GetCustomAttributes(typeof(JoinAttribute), false).Length > 0)
                    yield return method;
        }

        private MethodInfo GetChordMethod(MethodInfo joinMethod)
        {
            MethodInfo result = null;
            MethodInfo[] methods =
                source.GetMethods(BindingFlags.Instance | BindingFlags.Static |
                                  BindingFlags.Public | BindingFlags.NonPublic);
            foreach (MethodInfo method in methods)
                if (IsChord(method)
                    && SignatureMatch(joinMethod, method))
                {
                    if (result == null)
                        result = method;
                    else
                        throw new InvalidMethodException(source.Name, joinMethod.ToString(),
                                                         "There is more than one [Chord] method which has a nonchannel parameter signature that matches this [Join] method.");

                }

            if (result == null)
                throw new InvalidMethodException(source.Name, joinMethod.ToString(),
                                                 "There is no [Chord] method which has a nonchannel parameter signature that matches this [Join] method.");

            return result;
        }

        private static bool IsChord(MethodInfo method)
        {
            return method.GetCustomAttributes(typeof(ChordAttribute), false).Length > 0;
        }
	}
}