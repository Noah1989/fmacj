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
        public abstract class BusTestClass : IParallelizable
        {
            [Fork]
            public abstract void TestMethod1(int value);
            [Asynchronous]
            protected void TestMethod1(int value, [Channel("TestChannel1")] out int result)
            {
                result = value*value;
            }

            [Fork]
            public abstract void TestMethod2(int value);
            [Asynchronous]
            protected void TestMethod2(int value, [Channel("TestChannel2")] out double result)
            {
                result = 1/(double)value;
            }

            [Fork]
            public abstract void TestMethod3(int value);
            [Asynchronous]
            protected void TestMethod3(int value, [Channel("TestChannel3")] out int result)
            {
                result = -value;
            }

            public abstract void Dispose();
        }

        [SetUp]
        public void SetUp()
        {
			ConsoleOut.ShowAvailableThreadPoolThreads();
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
                            Channel<int> channel = ChannelResolver<int>.GetChannel(busTestClass, "TestChannel3");
                            object[] objects = new Bus(new IChannel[] {channel}).Receive();
                            Expect(objects.Length, EqualTo(1));
                            result = (int) objects[0];
                        });
            thread.Start();

            ThreadTimeout.Timeout(thread, 10000);

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
                            Channel<int> channel1 = ChannelResolver<int>.GetChannel(busTestClass, "TestChannel1");
                            Channel<double> channel2 = ChannelResolver<double>.GetChannel(busTestClass, "TestChannel2");
                            object[] objects = new Bus(new IChannel[] {channel1, channel2}).Receive();
                            Expect(objects.Length, EqualTo(2));
                            result = (int) objects[0] + (double) objects[1];
                        });
            thread.Start();

            ThreadTimeout.Timeout(thread, 10000);

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
                            Channel<int> channel1 = ChannelResolver<int>.GetChannel(busTestClass,"TestChannel1");
                            Channel<double> channel2 = ChannelResolver<double>.GetChannel(busTestClass,"TestChannel2");
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

            ThreadTimeout.Timeout(thread, 10000);

            for (int i = 1; i <= 1000; i++)
                Expect(result1.Contains(i*i), String.Format("Missing number: {0} ({1} received)", i, result1.Count));

            for (int i = 1; i <= 1000; i++)
                Expect(result2.Contains(1d/-i), String.Format("Missing number: {0} ({1} received)", i, result1.Count));
        }
    }
}
