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
    public class ForkTest : AssertionHelper
    {
        [Parallelizable]
        public abstract class ForkTestClass : IParallelizable
        {
            [Fork]
            public abstract void TestMethod(string testString);
            [Asynchronous]
            protected void TestMethod(string testString, [Channel("Dummy")] out object dummy)
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Loopback, 23000);
                StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream());
                streamWriter.Write(testString);
                streamWriter.Flush();
                tcpClient.Close();

                dummy = null;
            }

            [Fork]
            public abstract void TestMethod();
            [Asynchronous]
            protected void TestMethod([Channel("Dummy")] out object dummy)
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Loopback, 23000);
                StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream());
                streamWriter.Write("Aloha ʻoe");
                streamWriter.Flush();
                tcpClient.Close();

                dummy = null;
            }

            [Fork]
            public abstract void TestMethod(string testString, string testString2);
            [Asynchronous]
            protected void TestMethod(string testString, string testString2, [Channel("Dummy")] out object dummy)
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Loopback, 23000);
                StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream());
                streamWriter.Write(testString + testString2);
                streamWriter.Flush();
                tcpClient.Close();

                dummy = null;
            }

            [Fork]
            public abstract void MassiveTest(string testValue);
            [Asynchronous]
            protected void MassiveTest(string testValue, [Channel("Dummy")] out object dummy)
            {
                int value = Convert.ToInt32(testValue);
                
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Loopback, 23000 + value);
                BinaryWriter binaryWriter = new BinaryWriter(tcpClient.GetStream());
                binaryWriter.Write(value);
                binaryWriter.Flush();
                tcpClient.Close();

                dummy = null;
            }

            [Fork]
            public abstract void ValueTypeTest(int testValue);
            [Asynchronous]
            protected void ValueTypeTest(int testValue, [Channel("Dummy")] out object dummy)
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Loopback, 23000);
                BinaryWriter binaryWriter = new BinaryWriter(tcpClient.GetStream());
                binaryWriter.Write(testValue);
                binaryWriter.Flush();
                tcpClient.Close();

                dummy = null;
            }

            public abstract void Dispose();
        }

        [SetUp]
        public void SetUp()
        {
			ConsoleOut.ShowAvailableThreadPoolThreads();
            ParallelizationFactory.Clear();
            ParallelizationFactory.Parallelize(typeof(ForkTestClass).Assembly);
        }

        [Test]
        public void WithParameter()
        {
            ForkTestClass forkTestClass = ParallelizationFactory.GetParallelized<ForkTestClass>();
            
            TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 23000);
            tcpListener.Start();

            forkTestClass.TestMethod("Aloha ʻoe");

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
            Expect(new StreamReader(tcpClient.GetStream()).ReadToEnd(), EqualTo("Aloha ʻoe"));

            tcpClient.Close();
            tcpListener.Stop();

        }

        [Test]
        public void WithoutParameter()
        {
            ForkTestClass forkTestClass = ParallelizationFactory.GetParallelized<ForkTestClass>();

            TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 23000);
            tcpListener.Start();

            forkTestClass.TestMethod();

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
            Expect(new StreamReader(tcpClient.GetStream()).ReadToEnd(), EqualTo("Aloha ʻoe"));

            tcpClient.Close();
            tcpListener.Stop();

        }

        [Test]
        public void MultipleParameters()
        {
            ForkTestClass forkTestClass = ParallelizationFactory.GetParallelized<ForkTestClass>();

            TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 23000);
            tcpListener.Start();

            forkTestClass.TestMethod("Aloha", " ʻoe");

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
            Expect(new StreamReader(tcpClient.GetStream()).ReadToEnd(), EqualTo("Aloha ʻoe"));

            tcpClient.Close();
            tcpListener.Stop();

        }

		[Test]
        public void MassiveInvoke()
        {
            ForkTestClass forkTestClass = ParallelizationFactory.GetParallelized<ForkTestClass>();

            List<TcpListener> tcpListeners = new List<TcpListener>();

            for (int i = 0; i < 500; i++)
            {
                TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 23000 + i);
                tcpListeners.Add(tcpListener);
                tcpListener.Start();

                forkTestClass.MassiveTest(i.ToString());
            }

            List<int> results = new List<int>();
            
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

                if(i > timeout)
                    continue;

                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                results.Add(new BinaryReader(tcpClient.GetStream()).ReadInt32());

                tcpClient.Close();
                tcpListener.Stop();
            }
            
            Debug.Print(string.Format("Received {0} results.", results.Count));

            for (int i = 0; i < 500; i++)
            {
                Expect(results.Contains(i), string.Format("Missing value: {0}", i));
            }

        }

        [Test]
        public void ValueTypeParameter()
        {
            ForkTestClass forkTestClass = ParallelizationFactory.GetParallelized<ForkTestClass>();

            TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 23000);
            tcpListener.Start();

            forkTestClass.ValueTypeTest(235);

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
            Expect(new BinaryReader(tcpClient.GetStream()).ReadInt32(), EqualTo(235));

            tcpClient.Close();
            tcpListener.Stop();
        }
    }
}
