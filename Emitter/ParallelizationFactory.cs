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