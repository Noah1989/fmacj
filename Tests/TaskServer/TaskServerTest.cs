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
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using Fmacj.Core.Framework.Distribution;
using Fmacj.Components.TaskServer;

namespace Fmacj.Tests.TaskServer
{
	[TestFixture]
	public class TaskServerTest : AssertionHelper
	{
		private ConnectionFactory factory = new ConnectionFactory();
		private IProtocol protocol = Protocols.DefaultProtocol;
		private string hostname = "localhost";
		
		private IConnection conn;
		private IModel channel;
		
		private Fmacj.Components.TaskServer.TaskServer taskServer;
		private SimpleRpcClient client;
		
		[SetUp]
		public void SetUp()
		{  
			conn = factory.CreateConnection(protocol, hostname);
			channel = conn.CreateModel();
			
			Subscription subscription = new Subscription(channel);
			taskServer = new Fmacj.Components.TaskServer.TaskServer(subscription);
			//taskServer.MainLoop();
			
			client = new SimpleRpcClient(channel);
		}
		
		[Test]
		public void GetTaskTicket()
		{
			var request = TaskRequest.GetTaskTicket;
			var formatter = new BinaryFormatter();
		    var requestStream = new MemoryStream();
			formatter.Serialize(requestStream, request);
			var requestBytes = requestStream.ToArray();
						
			var responseBytes = client.Call(requestBytes);
			var responseStream = new MemoryStream(responseBytes, false);
			var response = formatter.Deserialize(responseStream);
			
			Expect(response, InstanceOfType(typeof(TaskTicket)));
		}			
		
		[TearDown]
		public void TearDown()
		{
			channel.Close();
			conn.Close();
		}
	}
}
