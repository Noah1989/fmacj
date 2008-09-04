using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using Fmacj.Emitter;
using Fmacj.Framework;
using Fmacj.Runtime;
using NUnit.Framework;

namespace Fmacj.Tests
{
    [TestFixture]
    public class BusTest : AssertionHelper
    {
        [Parallelizable]
        [Serializable]
        public abstract class BusTestClass : IParallelizable
        {
            protected BusTestClass()
            {
            }

            protected BusTestClass(SerializationInfo info, StreamingContext context)
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
            ParallelizationFactory.Parallelize(typeof(BusTestClass).Assembly);
        }

        [Test]
        public void OneChannel()
        {
            BusTestClass busTestClass = ParallelizationFactory.GetParallelized<BusTestClass>();
            
            busTestClass.TestMethod3(5);

            int result = 0;

            Thread thread =
                new Thread(
                    delegate()
                        {
                            Channel<int> channel = ChannelFactory<int>.GetChannel(busTestClass, "TestChannel3");
                            object[] objects = new Bus(new IChannel[] {channel}).Receive();
                            Expect(objects.Length, EqualTo(1));
                            result = (int) objects[0];
                        });
            thread.Start();
            Thread.Sleep(100);
            thread.Abort();

            Expect(result, EqualTo(-5));

        }

        [Test]
        public void TwoChannels()
        {
            BusTestClass busTestClass = ParallelizationFactory.GetParallelized<BusTestClass>();

            double result = 0;

            busTestClass.TestMethod1(2);
            busTestClass.TestMethod2(3);

            Thread thread =
                new Thread(
                    delegate()
                        {
                            Channel<int> channel1 = ChannelFactory<int>.GetChannel(busTestClass, "TestChannel1");
                            Channel<double> channel2 = ChannelFactory<double>.GetChannel(busTestClass, "TestChannel2");
                            object[] objects = new Bus(new IChannel[] {channel1, channel2}).Receive();
                            Expect(objects.Length, EqualTo(2));
                            result = (int) objects[0] + (double) objects[1];
                        });
            thread.Start();
            Thread.Sleep(100);
            thread.Abort();

            Expect(result, EqualTo(2*2 + 1d/3));
            
        }

        [Test]
        public void ManyValues()
        {
            BusTestClass busTestClass = ParallelizationFactory.GetParallelized<BusTestClass>();

            for (int i = 1; i <= 1000; i++)
            {
                busTestClass.TestMethod1(i);
                busTestClass.TestMethod2(-i);
            }
            Thread.Sleep(100);

            List<int> result1 = new List<int>();
            List<double> result2 = new List<double>();


            Thread thread =
                new Thread(
                    delegate()
                        {
                            Channel<int> channel1 = ChannelFactory<int>.GetChannel(busTestClass,"TestChannel1");
                            Channel<double> channel2 = ChannelFactory<double>.GetChannel(busTestClass,"TestChannel2");
                            var bus = new Bus(new IChannel[] {channel1, channel2});
                            for (int i = 1; i <= 1000; i++)
                            {

                                object[] objects = bus.Receive();
                                Expect(objects.Length, EqualTo(2));
                                result1.Add((int) objects[0]);
                                result2.Add((double) objects[1]);
                            }
                        });
            thread.Start();
            Thread.Sleep(500);
            thread.Abort();


            for (int i = 1; i <= 1000; i++)
                Expect(result1.Contains(i*i));

            for (int i = 1; i <= 1000; i++)
                Expect(result2.Contains(1d/-i));

        }
    }
}
