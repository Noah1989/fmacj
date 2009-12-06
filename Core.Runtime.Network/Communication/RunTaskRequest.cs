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
using System.Runtime.Serialization;

namespace Fmacj.Core.Runtime.Network.Communication
{	
	[Serializable]
	public class RunTaskRequest : Request
	{		
		public byte[] RawAssembly { get; private set; }
		public MethodInfo EntryPoint { get; private set; }
		
		public RunTaskRequest(byte[] rawAssembly, MethodInfo entryPoint)
		{
			RawAssembly = rawAssembly;
			EntryPoint = entryPoint;
		}
		
		protected RunTaskRequest(SerializationInfo info, StreamingContext context)
		{
			RawAssembly = info.GetValue("RawAssembly", typeof(byte[])) as byte[];
			EntryPoint = info.GetValue("EntryPoint", typeof(MethodInfo)) as MethodInfo;
		}
		
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("RawAssembly", RawAssembly);
			info.AddValue("EntryPoint", EntryPoint);
		}
	}
}
