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
    internal struct ForkGroup
    {
        public ForkGroup(Type sourceType, MethodInfo forkMethod, MethodInfo parallelMethod) : this()
        {
            SourceType = sourceType;
            ForkMethod = forkMethod;
            ParallelMethod = parallelMethod;
            FindChannelParameters();
        }

        private void FindChannelParameters()
        {
            List<ParameterInfo> channelParameters = new List<ParameterInfo>();
            
            foreach (ParameterInfo parallelParameter in ParallelMethod.GetParameters())
            {
                if (parallelParameter.GetCustomAttributes(typeof(ChannelAttribute), false).Length > 0)
                    channelParameters.Add(parallelParameter);
            }

            ChannelParameters = channelParameters.ToArray();
        }

        public Type SourceType { get; private set; }
        public MethodInfo ForkMethod { get; private set; }
        public MethodInfo ParallelMethod { get; private set; }

        public string Name { get { return ForkMethod.Name; } }
        public Type[] ParameterTypes
        {
            get
            {
                ParameterInfo[] parameters = ForkMethod.GetParameters();
                Type[] result = new Type[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                    result[i] = parameters[i].ParameterType;

                return result;
            }
        }

        public ParameterInfo[] Parameters
        {
            get { return ForkMethod.GetParameters(); }
        }
       
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
