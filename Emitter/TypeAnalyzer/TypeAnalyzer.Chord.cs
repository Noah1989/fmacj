using System;
using System.Collections.Generic;
using System.Reflection;
using Fmacj.Framework;

namespace Fmacj.Emitter
{
    internal partial class TypeAnalyzer
    {
		public IEnumerable<ChordInfo> GetChords()
        {
            foreach (MethodInfo chordMethod in
                source.GetMethods(BindingFlags.Instance | BindingFlags.Static |
                                  BindingFlags.Public | BindingFlags.NonPublic))
                if (chordMethod.GetCustomAttributes(typeof(ChordAttribute), false).Length > 0)
                    yield return new ChordInfo(source, chordMethod);
        }
	}
}