using System;
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
    public class ChordTest : AssertionHelper
    {
        [Parallelizable]
        [Serializable]
        public abstract class ChordTestClass : IParallelizable
        {
            protected ChordTestClass()
            {
            }

            protected ChordTestClass(SerializationInfo info, StreamingContext context)
            {
            }

            [Future]
            public abstract void TestMethod1(int value);
            [Asynchronous]
            protected void TestMethod1(int value, [Channel("TestChannel1")] out int result)
            {
                result = value*value;
            }

            [Future]
            public abstract void TestMethod2(int value);
            [Asynchronous]
            protected void TestMethod2(int value, [Channel("TestChannel2")] out double result)
            {
                result = 1/(double)value;
            }

            [Future]
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
                StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream());
                streamWriter.Write(value1 + value2);
                streamWriter.Flush();
                tcpClient.Close();
            }

            [Chord]
            protected void SimpleChord([Channel("TestChannel3")] int value)
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Loopback, 23000);
                StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream());
                streamWriter.Write(value);
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
            ParallelizationFactory.Parallelize(typeof(ChordTestClass).Assembly);
        }

        [Test]
        public void SimpleChord()
        {
            ChordTestClass chordTestClass = ParallelizationFactory.GetParallelized<ChordTestClass>();

            TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 23000);
            tcpListener.Start();

            chordTestClass.TestMethod3(5);

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
            Expect(new BinaryReader(tcpClient.GetStream()).ReadInt32(), EqualTo(-5));

            tcpClient.Close();
            tcpListener.Stop();

        }

        [Test]
        public void TwoChannelChord()
        {
            ChordTestClass chordTestClass = ParallelizationFactory.GetParallelized<ChordTestClass>();

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
            Expect(new BinaryReader(tcpClient.GetStream()).ReadDouble(), EqualTo(2*2 + 1d/3));

            tcpClient.Close();
            tcpListener.Stop();
            
        }

    }
}
