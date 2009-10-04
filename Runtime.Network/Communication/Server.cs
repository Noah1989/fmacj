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
using System.Net;
using System.Net.Sockets;
using Fmacj.Framework;

namespace Fmacj.Runtime.Network.Communication
{
	public abstract class Server : IParallelizable
	{		
		private readonly TcpListener tcpListener;
		
		protected Server(int port)		
		{			
			tcpListener = new TcpListener(IPAddress.Any, port);
		}
		
		[Fork]
		[Asynchronous]
		protected virtual void StartServer()
		{			
			tcpListener.Start();
			while(true)
				HandleClient(tcpListener.AcceptTcpClient());
		}
		
		[Fork]
		[Asynchronous]
		public virtual void HandleClient(TcpClient client)
		{			
			Stream clientStream = client.GetStream();
			
			bool running = true;
			while (running)
			{
				try
				{
					Request request = Request.Receive(clientStream);
					Response response;
					
					try
					{
						response = HandleRequest(request);
					}
					catch (Exception ex)
					{
						response = new ExceptionResponse(ex);
					}		
					
					response.Send(clientStream);
				}
				catch(EndOfStreamException)
				{
					running = false;
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex);
					running = false;
				}				
			}
		}
		
		protected abstract Response HandleRequest(Request request);
		
		public void Dispose()
		{
			tcpListener.Stop();
		}
	}
}