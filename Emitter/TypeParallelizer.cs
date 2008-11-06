﻿using System;
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
            ChordImplementer chordImplementer = new ChordImplementer(typeBuilder, source, channelImplementer);
			JoinImplementer joinImplementer = new JoinImplementer(typeBuilder, channelImplementer);

            foreach (ForkGroup forkGroup in typeAnalyzer.GetForkGroups())
                forkImplementer.Implement(forkGroup);

            foreach (ChordInfo chord in typeAnalyzer.GetChords())
                chordImplementer.Implement(chord);
			
			foreach (JoinGroup joinGroup in typeAnalyzer.GetJoinGroups())
				joinImplementer.Implement(joinGroup);

            chordImplementer.ImplementConstructor();

            return typeBuilder.CreateType();
        }
    }
}
