using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using Fmacj.Framework;

namespace Fmacj.Emitter
{
    internal class AssemblyParallelizer
    {
        private readonly Assembly source;
        private readonly AssemblyName parallelizedAssemblyName;

        public AssemblyParallelizer(Assembly source)
        {
            this.source = source;
            parallelizedAssemblyName = source.GetName();
            parallelizedAssemblyName.Name += ".Parallelized";
        }

        public AssemblyName ParallelizedAssemblyName
        {
            get { return parallelizedAssemblyName; } 
        }

        public Assembly GetParallelizedAssembly()
        {
            AssemblyBuilder assemblyBuilder =
                AppDomain.CurrentDomain.DefineDynamicAssembly(ParallelizedAssemblyName, AssemblyBuilderAccess.Run);

            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(ParallelizedAssemblyName.Name);

            foreach (Type sourceType in source.GetTypes())
            {
#if DEBUG
                try
                {
#endif
                    if (sourceType.GetCustomAttributes(typeof (ParallelizableAttribute), false).Length > 0)
                    {
                        if (!TypeValidator.IsParallelizable(sourceType))
                            throw new InvalidTypeException(sourceType.Name);

                        TypeParallelizer typeParallelizer = new TypeParallelizer(sourceType);
                        typeParallelizer.DefineParallelizedType(moduleBuilder);
                    }
#if DEBUG
                }
                catch (Exception ex)
                {
                    Debug.Print("Something went wrong during parallelization of type {3}.\n{0}: {1}\n{2}\n",
                                ex.GetType(), ex.Message, ex.StackTrace, sourceType.Name);
                }
#endif
            }

            return assemblyBuilder;
        }
    }
}
