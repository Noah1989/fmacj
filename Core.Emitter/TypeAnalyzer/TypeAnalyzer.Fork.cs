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
                if (IsParallel(method)
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