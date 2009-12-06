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
using System.Reflection;
using System.Reflection.Emit;
using Fmacj.Core.Framework;

namespace Fmacj.Core.Emitter
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
                    Console.Error.WriteLine("Something went wrong during parallelization of type {3}.\n{0}: {1}\n{2}\n",
                                            ex.GetType(), ex.Message, ex.StackTrace, sourceType.Name);
                }
#endif
            }

            return assemblyBuilder;
        }
    }
}
