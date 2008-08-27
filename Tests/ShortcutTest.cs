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
        [Serializable]
        public abstract class ShortcutTestClass : IParallelizable
        {
            protected ShortcutTestClass()
            {
            }

            protected ShortcutTestClass(SerializationInfo info, StreamingContext context)
            {
            }

            [Future]
            [Asynchronous]
            public virtual void NoChannelFutureGroup(string testString)
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Loopback, 23000);
                StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream());
                streamWriter.Write(testString);
                streamWriter.Flush();
                tcpClient.Close();
            }
            

            public object Clone()
            {
                throw new NotImplementedException();
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                throw new NotImplementedException();
            }
        }

        [SetUp]
        public void SetUp()
        {
            ParallelizationFactory.Clear();
            ParallelizationFactory.Parallelize(typeof(ShortcutTestClass).Assembly);
        }

        [Test]
        public void NoChannelFutureGroup()
        {
            ShortcutTestClass shortcutTestClass = ParallelizationFactory.GetParallelized<ShortcutTestClass>();
            
            TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 23000);
            tcpListener.Start();
            
            shortcutTestClass.NoChannelFutureGroup("Test");

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
