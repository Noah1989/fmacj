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
using System.Runtime.Serialization;

namespace Fmacj.Runtime.Network.Communication
{	
	[Serializable]
	public class ExceptionResponse : DistributionServerResponse
	{		
		public Exception Exception { get; private set; }
		
		public ExceptionResponse(Exception exception)
		{
			Exception = exception;	
		}
		
		protected ExceptionResponse(SerializationInfo info, StreamingContext context)
		{
			Exception = info.GetValue("Exception", typeof(Exception)) as Exception;
		}
		
		public override void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Exception", Exception);
		}

	}
}
