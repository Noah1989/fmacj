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
using System.Net.Sockets;
using NUnit.Framework;
using Fmacj.Core.Framework;
using Fmacj.Components.TaskClient;

namespace Fmacj.Tests
{
    [TestFixture]
	public class DistributionTest : AssertionHelper
	{
		public static void TestEntryPoint()
		{
			Console.WriteLine("Running");
		}

		private TcpClient tcpClient;
		private Stream stream;
		private TaskClient taskClient;
		
		[SetUp]
        public void SetUp()
        {
			try 
			{
				tcpClient = new TcpClient("localhost", Constants.DefaultTaskServerPort);				
			}
			catch (Exception ex)
			{
            	Assert.Ignore("Could not connect to distribution server: {0}.", ex);			
			}

			tcpClient.SendTimeout = 5000;
			tcpClient.ReceiveTimeout = 5000;
			stream = tcpClient.GetStream();
			taskClient = new TaskClient(stream);
		}

		[Test]
		public void Test()
		{
			taskClient.RunTask(TestEntryPoint);
		}
	}
}