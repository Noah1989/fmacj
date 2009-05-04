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
using Fmacj.Framework;
using Fmacj.Runtime.Network.Communication;

namespace Fmacj.Executables.WorkClient
{	
	internal class WorkClient : Client
	{	
		private WorkClientTicket ticket;
		
		public WorkClient(Stream serverStream) : base(serverStream)
		{		
		}
		
		public void Start()
		{
			Register();
			
			
		}
		
		private void Register()
		{
			RegisterWorkClientRequest request = new RegisterWorkClientRequest();
			Response response = SendRequest(request);
			Type responseType = response.GetType();
			if(responseType == typeof(WorkClientRegisterdResponse))
				ticket = (response as WorkClientRegisterdResponse).Ticket;
			else
				throw new NotSupportedException(String.Format("The response of type {0} was not understood.", responseType.Name));
		}
	}
}
