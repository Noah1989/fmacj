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
using Fmacj.Emitter;
using Fmacj.Framework;

namespace Fmacj.Emitter
{
    public static class ParallelizationFactory 
    {
        private static readonly Dictionary<Assembly, Assembly> parallelizedAssemblies =
            new Dictionary<Assembly, Assembly>();

        public static void Clear()
        {
            parallelizedAssemblies.Clear();
        }
            
        public static void Parallelize(Assembly assembly)
        {
            if (parallelizedAssemblies.ContainsKey(assembly))
                throw new InvalidOperationException(String.Format("The assembly {0} has already been parallelized.",
                                                                  assembly.FullName));

            AssemblyParallelizer parallelizer = new AssemblyParallelizer(assembly);
            parallelizedAssemblies.Add(assembly, parallelizer.GetParallelizedAssembly());
        }

        public static T GetParallelized<T>() where T : class, IParallelizable
        {
            Type source = typeof (T);
            Assembly assembly = source.Assembly;
            if (!parallelizedAssemblies.ContainsKey(assembly))
                throw new InvalidOperationException(String.Format("The assembly {0} has not been parallelized.",
                                                                  assembly.FullName));

            Assembly parallelizedAssembly = parallelizedAssemblies[assembly];

            Type parallelizedType = parallelizedAssembly.GetType(TypeParallelizer.GetParallelizedTypeName(source));

            if (parallelizedType == null)
                throw new TypeNotFoundException(source.Name);

            return parallelizedType.GetConstructor(new Type[] {}).Invoke(new object[] {}) as T;
        }
    }
}