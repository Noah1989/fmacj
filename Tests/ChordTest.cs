using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using Fmacj.Emitter;
using Fmacj.Framework;
using NUnit.Framework;

namespace Fmacj.Tests
{
    [TestFixture]
    public class ChordTest
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
            protected void TestMethod2(int value, [Channel("TestChannel2")] out int result)
            {
                result = 1/value;
            }

            [Chord]
            protected void TestChord([Channel("TestChannel1")] int value1, [Channel("TestChannel2")] int value2)
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Loopback, 23000);
                StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream());
                streamWriter.Write(value1 + value2);
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

    }
}
