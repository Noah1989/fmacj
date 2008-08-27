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
    public class FutureTest : AssertionHelper
    {
        [Parallelizable]
        [Serializable]
        public abstract class FutureTestClass : IParallelizable
        {
            protected FutureTestClass()
            {
            }

            protected FutureTestClass(SerializationInfo info, StreamingContext context)
            {
            }

            [Future]
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

            [Future]
            public abstract void TestMethod();
            [Asynchronous]
            protected void TestMethod([Channel("Dummy")] out object dummy)
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Loopback, 23000);
                StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream());
                streamWriter.Write("Test");
                streamWriter.Flush();
                tcpClient.Close();

                dummy = null;
            }

            [Future]
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

            [Future]
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

            [Future]
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
            ParallelizationFactory.Parallelize(typeof(FutureTestClass).Assembly);
        }

        [Test]
        public void WithParameter()
        {
            FutureTestClass futureTestClass = ParallelizationFactory.GetParallelized<FutureTestClass>();
            
            TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 23000);
            tcpListener.Start();
            
            futureTestClass.TestMethod("Test");

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

        [Test]
        public void WithoutParameter()
        {
            FutureTestClass futureTestClass = ParallelizationFactory.GetParallelized<FutureTestClass>();

            TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 23000);
            tcpListener.Start();

            futureTestClass.TestMethod();

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

        [Test]
        public void MultipleParameters()
        {
            FutureTestClass futureTestClass = ParallelizationFactory.GetParallelized<FutureTestClass>();

            TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 23000);
            tcpListener.Start();

            futureTestClass.TestMethod("Te", "st");

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

        [Test]
        public void MassiveInvoke()
        {
            FutureTestClass futureTestClass = ParallelizationFactory.GetParallelized<FutureTestClass>();

            List<TcpListener> tcpListeners = new List<TcpListener>();

            for (int i = 0; i < 1000; i++)
            {
                TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 23000 + i);
                tcpListeners.Add(tcpListener);
                tcpListener.Start();

                futureTestClass.MassiveTest(i.ToString());
            }

            List<int> results = new List<int>();
            
            int timeoutCount = 0;

            foreach (TcpListener tcpListener in tcpListeners)
            {
                int i = 0;
                var timeout = 2;
                while (!tcpListener.Pending())
                {
                    Thread.Sleep(100);
                    if (++i > timeout)
                    {
                        tcpListener.Stop();
                        if (timeoutCount++ > 10)
                            throw new TimeoutException();
                        break;
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

            for (int i = 0; i < 1000; i++)
            {
                Expect(results.Contains(i), string.Format("Missing value: {0}", i));
            }

        }

        [Test]
        public void ValueTypeParameter()
        {
            FutureTestClass futureTestClass = ParallelizationFactory.GetParallelized<FutureTestClass>();

            TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 23000);
            tcpListener.Start();

            futureTestClass.ValueTypeTest(235);

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
