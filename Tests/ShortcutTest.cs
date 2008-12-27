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
using System.Runtime.Serialization;
using System.Threading;
using Fmacj.Emitter;
using Fmacj.Framework;
using NUnit.Framework;

namespace Fmacj.Tests
{
    [TestFixture]
    public class ShortcutTest : AssertionHelper
    {
        [Parallelizable]
        public abstract class ShortcutTestClass : IParallelizable
        {
            [Fork]
            [Asynchronous]
            public virtual void NoChannelForkGroup(string testString)
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Loopback, 23000);
                StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream());
                streamWriter.Write(testString);
                streamWriter.Flush();
                tcpClient.Close();
            }

            public abstract void Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            ParallelizationFactory.Clear();
            ParallelizationFactory.Parallelize(typeof(ShortcutTestClass).Assembly);
        }

        [Test]
        public void NoChannelForkGroup()
        {
            using (ShortcutTestClass shortcutTestClass = ParallelizationFactory.GetParallelized<ShortcutTestClass>())
			{
				TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 23000);
				tcpListener.Start();
				
				shortcutTestClass.NoChannelForkGroup("Test");
				
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
				Expect(new StreamReader(tcpClient.GetStream()).ReadToEnd(), EqualTo("Test"));
				
				tcpClient.Close();
				tcpListener.Stop();
			}
		}
	}
}