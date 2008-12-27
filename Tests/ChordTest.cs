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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Fmacj.Emitter;
using Fmacj.Framework;
using NUnit.Framework;

namespace Fmacj.Tests
{	
	[TestFixture]
    public class ChordTest : AssertionHelper
    {
        [Parallelizable]
        public abstract class ChordTestClass : IParallelizable
        {
            [Fork]
            public abstract void TestMethod1(int value);
            [Asynchronous]
            protected void TestMethod1(int value, [Channel("TestChannel1")] out int result)
            {				
                result = value * value;
            }

            [Fork]
            public abstract void TestMethod2(int value);
            [Asynchronous]
            protected void TestMethod2(int value, [Channel("TestChannel2")] out double result)
            {
                result = 1 / (double)value;
            }

            [Fork]
            public abstract void TestMethod3(int value);
            [Asynchronous]
            protected void TestMethod3(int value, [Channel("TestChannel3")] out int result)
            {
                result = -value;
            }

            [Chord]
            protected void TestChord([Channel("TestChannel1")] int value1, [Channel("TestChannel2")] double value2)
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Loopback, 23000);
                BinaryWriter binaryWriter = new BinaryWriter(tcpClient.GetStream());
                binaryWriter.Write(value1 + value2);
                binaryWriter.Flush();
                tcpClient.Close();
            }

            [Chord]
            protected void SimpleChord([Channel("TestChannel3")] int value)
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Loopback, 23000);
                BinaryWriter binaryWriter = new BinaryWriter(tcpClient.GetStream());
                binaryWriter.Write(value);
                binaryWriter.Flush();
                tcpClient.Close();
            }

            [Fork]
            public abstract void TestMethod4(string value);
            [Asynchronous]
            protected void TestMethod4(string value, [Channel("TestChannel4")] out string result)
            {
                Thread.Sleep(random.Next(20));
                result = string.Format("{0},{{0}}", value);
            }

            [Fork]
            public abstract void TestMethod5(int value);
            [Asynchronous]
            protected void TestMethod5(int value, [Channel("TestChannel5")] out int result)
            {
                Thread.Sleep(random.Next(20));
                result = 23000 + value;
            }

            [Chord]
            protected void ComplexChord([Channel("TestChannel4")] string value1, [Channel("TestChannel5")] int value2)
            {
                string value = String.Format(value1, value2);

                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Loopback, value2);
                BinaryWriter binaryWriter = new BinaryWriter(tcpClient.GetStream());
                binaryWriter.Write(value);
                binaryWriter.Flush();
                tcpClient.Close();
            }

            private readonly Random random = new Random();


            public abstract void Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            ParallelizationFactory.Clear();
            ParallelizationFactory.Parallelize(typeof(ChordTestClass).Assembly);			
        }

        [Test]
        public void SimpleChord()
        {
            using (ChordTestClass chordTestClass = ParallelizationFactory.GetParallelized<ChordTestClass>())
			{
				TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 23000);
				tcpListener.Start();
				
				chordTestClass.TestMethod3(5);
				
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
				Expect(new BinaryReader(tcpClient.GetStream()).ReadInt32(), EqualTo(-5));
				
				tcpClient.Close();
				tcpListener.Stop();
				
			}
		}
        
		[Test]
        public void TwoChannelChord()
        {
            using (ChordTestClass chordTestClass = ParallelizationFactory.GetParallelized<ChordTestClass>())
			{
				TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 23000);
				tcpListener.Start();
				
				chordTestClass.TestMethod1(2);
				chordTestClass.TestMethod2(3);
				
				int i = 0;
				while (!tcpListener.Pending())
				{
					Thread.Sleep(100);
                if (i++ > 20)
					{
						tcpListener.Stop();
						throw new TimeoutException();
					}
				}
				
				TcpClient tcpClient = tcpListener.AcceptTcpClient();
				Expect(new BinaryReader(tcpClient.GetStream()).ReadDouble(), EqualTo(2 * 2 + 1d / 3));
				
				tcpClient.Close();
				tcpListener.Stop();
			}		
		}
		
        [Test]
        public void MassiveInvoke()
        {
            using (ChordTestClass chordTestClass = ParallelizationFactory.GetParallelized<ChordTestClass>())
			{
				List<TcpListener> tcpListeners = new List<TcpListener>();

				for (int i = 0; i < 500; i++)
				{
					TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 23000 + i);
					tcpListeners.Add(tcpListener);
					tcpListener.Start();
					chordTestClass.TestMethod4(string.Format("V{0}", i));
					chordTestClass.TestMethod5(i);
				}
				
				List<string> results = new List<string>();
				
				foreach (TcpListener tcpListener in tcpListeners)
				{
					int i = 0;
					var timeout = 10;
					while (!tcpListener.Pending())
					{
						Thread.Sleep(100);
						if (++i > timeout)
						{
							tcpListener.Stop();
							throw new TimeoutException();
						}
					}
					
					TcpClient tcpClient = tcpListener.AcceptTcpClient();
					results.Add(new BinaryReader(tcpClient.GetStream()).ReadString());
					
					tcpClient.Close();
					tcpListener.Stop();
				}
				
				Debug.Print(string.Format("Received {0} results.", results.Count));
				
				List<string> results1 = new List<string>();
				List<string> results2 = new List<string>();
				
				foreach(string value in results)
				{
					string[] values = value.Split(',');
					results1.Add(values[0]);
					results2.Add(values[1]);
				}
				
				for (int i = 0; i < 500; i++)
				{
					Expect(results1.Contains(string.Format("V{0}", i)),
					       string.Format("Missing value1: {0}", i));
					Expect(results2.Contains(string.Format("{0}", 23000 + i)),
					       string.Format("Missing value2: {0}", i));
				}
				
			}
		}
	}
}