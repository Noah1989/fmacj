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
using Fmacj.Framework;

namespace Fmacj.Emitter
{
    internal partial class TypeAnalyzer
    {
		public IEnumerable<YieldInfo> GetYields()
        {
            foreach (MethodInfo yieldMethod in
                source.GetMethods(BindingFlags.Instance | BindingFlags.Static |
                                  BindingFlags.Public | BindingFlags.NonPublic))
                if (yieldMethod.GetCustomAttributes(typeof(YieldAttribute), false).Length > 0)
				{
					if(!yieldMethod.IsAbstract)
						throw new InvalidMethodException(source.Name, yieldMethod.Name, 
					                                     "The [Yield] method is not abstract.");
                    yield return new YieldInfo(source, yieldMethod);
				}
        }
	}
}