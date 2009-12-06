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
        private readonly Type source;

        public TypeAnalyzer(Type source)
        {
            this.source = source;
        }
		
        private static bool SignatureMatch(MethodInfo shortMethod , MethodInfo longMethod)
        {
            if (shortMethod.Name != longMethod.Name)
                return false;
			
			if (shortMethod.ReturnType != longMethod.ReturnType)
                return false;

            IEnumerable<ParameterInfo> longMethodParameters = GetNonChannelParameters(longMethod);
            IEnumerator<ParameterInfo> longMethodParameterEnumerator = longMethodParameters.GetEnumerator();

            foreach (ParameterInfo shortMethodParameter in shortMethod.GetParameters())
            {
                if(!longMethodParameterEnumerator.MoveNext())
                    return false;

                ParameterInfo longMethodParameter = longMethodParameterEnumerator.Current;
                
                if(shortMethodParameter.ParameterType != longMethodParameter.ParameterType)
                    return false;
            }

            if (longMethodParameterEnumerator.MoveNext())
                return false;

        
            return true;
        }

        private static IEnumerable<ParameterInfo> GetNonChannelParameters(MethodInfo method)
        {
            foreach (ParameterInfo parameter in method.GetParameters())
                if(parameter.GetCustomAttributes(typeof(ChannelAttribute),false).Length == 0)
                    yield return parameter;
        }
    }
}