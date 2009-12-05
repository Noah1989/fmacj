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
			ChannelImplementer channelImplementer = new ChannelImplementer(typeBuilder);
            ForkImplementer forkImplementer = new ForkImplementer(typeBuilder, channelImplementer);
            ChordImplementer chordImplementer = new ChordImplementer(typeBuilder, channelImplementer);
			JoinImplementer joinImplementer = new JoinImplementer(typeBuilder, channelImplementer);
			YieldImplementer yieldImplementer = new YieldImplementer(typeBuilder, channelImplementer);
			DisposeImplementer disposeImplementer = new DisposeImplementer(typeBuilder, source);
			ConstructorImplementer constructorImplementer = new ConstructorImplementer(typeBuilder, source, 
			                                                                           channelImplementer, 
			                                                                           chordImplementer, 
			                                                                           disposeImplementer);
			
            foreach (ForkGroup forkGroup in typeAnalyzer.GetForkGroups())
                forkImplementer.Implement(forkGroup);

            foreach (ChordInfo chord in typeAnalyzer.GetChords())
                chordImplementer.Implement(chord);
			
			foreach (JoinGroup joinGroup in typeAnalyzer.GetJoinGroups())
				joinImplementer.Implement(joinGroup);

			foreach (YieldInfo yieldInfo in typeAnalyzer.GetYields())
				yieldImplementer.Implement(yieldInfo);

            disposeImplementer.ImplementDisposalBehavior();
			constructorImplementer.ImplementConstructor();            

            return typeBuilder.CreateType();
        }
    }
}
