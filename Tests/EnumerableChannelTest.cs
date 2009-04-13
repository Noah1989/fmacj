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
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Fmacj.Emitter;
using Fmacj.Framework;
using Fmacj.Runtime;
using NUnit.Framework;

namespace Fmacj.Tests
{
    [TestFixture]
    public class EnumerableChannelTest : AssertionHelper
    {		
        [Parallelizable]
        public abstract class EnumerableChannelTestClass : IParallelizable
        {

			[Fork]
            public abstract void TestMethod1(int val);
            [Asynchronous]
            protected void TestMethod1(int val, [Channel("TestChannel1")] out int result)
            {
                result = -val;
            }
 
            [Chord]
            protected void SimpleChord([Channel("TestChannel1", Enumerable = true)] IChannelEnumerable<int> values)
            {		
				TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Loopback, 23000);
                BinaryWriter binaryWriter = new BinaryWriter(tcpClient.GetStream());
				
				Thread thread =
                	new Thread(
                    	delegate()
                        	{								
								foreach (int val in values.Take(10))
                					binaryWriter.Write(val);
                        	});
            	
				thread.Start();

            	ThreadTimeout.Timeout(thread, 10000);

				binaryWriter.Flush();
                tcpClient.Close();				
            }
			
            public abstract void Dispose();
        }

		[SetUp]
        public void SetUp()
        {
            ParallelizationFactory.Clear();
            ParallelizationFactory.Parallelize(typeof(EnumerableChannelTestClass).Assembly);
		}

        [Test]
        public void SimpleChord()
        {
            using (EnumerableChannelTestClass enumerableChannelTestClass = ParallelizationFactory.GetParallelized<EnumerableChannelTestClass>())
			{
				TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 23000);
				tcpListener.Start();

				for (int n = 0; n < 20; n++)
					enumerableChannelTestClass.TestMethod1(n);
				
				List<int> results = new List<int>();					
				
				for(int c = 0; c < 2; c++)
				{				
					int i = 0;
					while (!tcpListener.Pending())
					{
						Thread.Sleep(200);
						if (i++ > 20)
						{
							tcpListener.Stop();
							throw new TimeoutException();
						}
					}
				
					TcpClient tcpClient = tcpListener.AcceptTcpClient();
					BinaryReader reader = new BinaryReader(tcpClient.GetStream());

			    
					for (int n = 0; n < 10; n++)
						results.Add(reader.ReadInt32());				
					
					tcpClient.Close();
				}
				
				for (int n = 0; n < 20; n++)
					Expect(results.Contains(-n));
								
				tcpListener.Stop();				
			}
		}
    }
}
