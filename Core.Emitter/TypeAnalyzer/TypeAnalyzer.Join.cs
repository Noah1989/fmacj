/*
    FMACJ Parallelization Framework for .NET
    Copyright (C) 2008  Stefan Noack

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using Fmacj.Core.Framework;

namespace Fmacj.Core.Emitter
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