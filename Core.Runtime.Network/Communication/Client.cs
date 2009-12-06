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
using Fmacj.Core.Framework;

namespace Fmacj.Core.Runtime.Network.Communication
{	
	public abstract class Client
	{		
		private Stream serverStream;
		
		public Client(Stream serverStream)
		{
			this.serverStream = serverStream;
		}
		
		protected Response SendRequest(Request request)
		{
			Response response = request.Send(serverStream);
			HandleExceptions(response);
			return response;
		}
		
		private void HandleExceptions(Response response)
		{
			ExceptionResponse exceptionResponse = response as ExceptionResponse;
			if(exceptionResponse == null) return;
			
			throw new RemoteException(exceptionResponse.Exception);
		}
	}
}
