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
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Fmacj.Core.Runtime.Network.Communication
{	
	[Serializable]
	public abstract class Request : ISerializable
	{	
		private static BinaryFormatter formatter = new BinaryFormatter();			
		
		public Response Send(Stream serverStream)
		{			
			formatter.Serialize(serverStream, this);
			return formatter.Deserialize(serverStream) as Response;
		}
		
		public static Request Receive(Stream stream)
		{
			return formatter.Deserialize(stream) as Request;
		}
		
		public abstract void GetObjectData(SerializationInfo info, StreamingContext context);
	}
}