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
    internal struct ChordInfo
    {
        public ChordInfo(Type sourceType, MethodInfo chordMethod): this()
        {
            SourceType = sourceType;
            ChordMethod = chordMethod;
            FindParameters();
        }

        private void FindParameters()
        {
			List<ParameterInfo> joinParameters = new List<ParameterInfo>();
            List<ParameterInfo> inChannelParameters = new List<ParameterInfo>();
            List<ParameterInfo> outChannelParameters = new List<ParameterInfo>();

            foreach (ParameterInfo parameter in ChordMethod.GetParameters())
            {
                if (parameter.GetCustomAttributes(typeof(ChannelAttribute), false).Length > 0)
                {
					if(parameter.IsOut)
					{	
                			outChannelParameters.Add(parameter);                		
                		}
                		else
                		{                    	
						if(outChannelParameters.Count != 0)
                				throw new InvalidMethodException(SourceType.Name, ChordMethod.Name, "Output channel prameters may not precede input channel parameters.");
						
						inChannelParameters.Add(parameter);
                		}
                }
				else
				{
				    if(inChannelParameters.Count != 0)
                		throw new InvalidMethodException(SourceType.Name, ChordMethod.Name, "Input channel prameters may not precede join parameters.");

					if(outChannelParameters.Count != 0)
                		throw new InvalidMethodException(SourceType.Name, ChordMethod.Name, "Output channel prameters may not precede join parameters.");
					
					joinParameters.Add(parameter);
				}
            }

			JoinParameters = joinParameters.ToArray();
            InChannelParameters = inChannelParameters.ToArray();
            OutChannelParameters = outChannelParameters.ToArray();
        }

        public Type SourceType { get; private set; }
        public MethodInfo ChordMethod { get; private set; }

        public string Name { get { return ChordMethod.Name; } }
        
		public ParameterInfo[] JoinParameters
		{
			get; private set;
		}
		
        public ParameterInfo[] InChannelParameters
        {
            get; private set;
        }

        public ParameterInfo[] OutChannelParameters
        {
            get; private set;
        }
		
        public string[] InChannelNames
        {
            get
            {
                string[] result = new string[InChannelParameters.Length];

                for (int i = 0; i < InChannelParameters.Length; i++)
                    result[i] =
                        ((ChannelAttribute)
                         InChannelParameters[i].GetCustomAttributes(typeof (ChannelAttribute), false)[0]).Name;

                return result;
            }
        }
		
		public ChannelAttribute[] InChannelAttributes
        {
            get
            {
                ChannelAttribute[] result = new ChannelAttribute[InChannelParameters.Length];

                for (int i = 0; i < InChannelParameters.Length; i++)
                    result[i] = (ChannelAttribute)
						InChannelParameters[i].GetCustomAttributes(typeof (ChannelAttribute), false)[0];

                return result;
            }
        }

        public string[] OutChannelNames
        {
            get
            {
                string[] result = new string[OutChannelParameters.Length];

                for (int i = 0; i < OutChannelParameters.Length; i++)
                    result[i] =
                        ((ChannelAttribute)
                         OutChannelParameters[i].GetCustomAttributes(typeof (ChannelAttribute), false)[0]).Name;

                return result;
            }
        }
		
	    public Type GetEnumerableInChannelType(int channelIndex)
		{
			Type parameterType = InChannelParameters[channelIndex].ParameterType;

			if(parameterType.GetGenericTypeDefinition() != typeof(IChannelEnumerable<>))
				throw new InvalidMethodException(SourceType.Name, ChordMethod.Name, "Enumerable channel prameter type must be IChannelEnumerable<>.");

			return parameterType.GetGenericArguments()[0];
		}		
    }
}
