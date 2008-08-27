using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Fmacj.Emitter
{
    internal class TypeParallelizer
    {
        private readonly Type source;
        private readonly string parallelizedTypeName;

        public TypeParallelizer(Type source)
        {
            this.source = source;
            parallelizedTypeName = GetParallelizedTypeName(source);
        }

        public string ParallelizedTypeName
        {
            get { return parallelizedTypeName; } 
        }

        public static string GetParallelizedTypeName(Type source)
        {
            return string.Format("Parallelized{0}", source.Name);
        }

        public Type DefineParallelizedType(ModuleBuilder moduleBuilder)
        {
            TypeBuilder typeBuilder = moduleBuilder.DefineType(ParallelizedTypeName);
            typeBuilder.SetParent(source);

            TypeAnalyzer typeAnalyzer = new TypeAnalyzer(source);
            FutureImplementer futureImplementer = new FutureImplementer(typeBuilder);


            foreach (FutureGroup futureGroup in typeAnalyzer.GetFutureGroups())
                futureImplementer.Implement(futureGroup);

            return typeBuilder.CreateType();
        }
    }
}
