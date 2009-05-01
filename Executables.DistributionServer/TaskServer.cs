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
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Reflection;
using Fmacj.Framework;
using Fmacj.Runtime.Network.Communication;

namespace Fmacj.Executables.DistributionServer
{
	[Parallelizable]
	internal abstract class TaskServer : IParallelizable
	{		
		private readonly TcpListener tcpListener;
		private WorkServer workServer;
		
		public TaskServer()
		{			
			tcpListener = new TcpListener(IPAddress.Any, 23542);
		}

		public void Run(WorkServer workServer)
		{
			this.workServer = workServer;
			
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
					DistributionServerRequest request = DistributionServerRequest.Receive(clientStream);
					DistributionServerResponse response;
					
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
		
		private DistributionServerResponse HandleRequest(DistributionServerRequest request)
		{
			Type requestType = request.GetType();
			if (requestType == typeof(RunTaskRequest))
			{
				RunTaskRequest runTaskRequest = request as RunTaskRequest;				
				workServer.RunTask(runTaskRequest.RawAssembly, runTaskRequest.EntryPoint);
				
				throw new NotImplementedException();
			}
			else
			{
				throw new NotSupportedException(String.Format("The request of type {0} is not supported", requestType));
			}
		}

		public void Dispose()
		{
			tcpListener.Stop();
		}
	}
}
