// Listener.cs created with MonoDevelop
// User: noah at 11:53 02/26/2009
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Reflection;
using Fmacj.Framework;

namespace Fmacj.Executables.DistributionServer
{
	[Parallelizable]
	internal abstract class Server : IParallelizable
	{		
		private readonly TcpListener tcpListener;

		public Server()
		{			
			tcpListener = new TcpListener(IPAddress.Any, 23542);
		}

		public void Run()
		{
			tcpListener.Start();
			while(true)
				HandleRequest(tcpListener.AcceptTcpClient());
		}

		[Fork]
		[Asynchronous]
		public virtual void HandleRequest(TcpClient client)
		{
			Stream stream = client.GetStream();
		}

		public void Dispose()
		{
			tcpListener.Stop();
		}
	}
}
