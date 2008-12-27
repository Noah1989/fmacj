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
            FindChannelParameters();
        }

        private void FindChannelParameters()
        {
            List<ParameterInfo> channelParameters = new List<ParameterInfo>();

            foreach (ParameterInfo parameter in ChordMethod.GetParameters())
            {
                if (parameter.GetCustomAttributes(typeof(ChannelAttribute), false).Length > 0)
                    channelParameters.Add(parameter);
            }

            ChannelParameters = channelParameters.ToArray();
        }

        public Type SourceType { get; private set; }
        public MethodInfo ChordMethod { get; private set; }

        public string Name { get { return ChordMethod.Name; } }
        
        public ParameterInfo[] ChannelParameters
        {
            get; private set;
        }

        public string[] ChannelNames
        {
            get
            {
                string[] result = new string[ChannelParameters.Length];

                for (int i = 0; i < ChannelParameters.Length; i++)
                    result[i] =
                        ((ChannelAttribute)
                         ChannelParameters[i].GetCustomAttributes(typeof (ChannelAttribute), false)[0]).Name;

                return result;
            }
        }
    }
}
