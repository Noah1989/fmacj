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
    public class ChannelTest : AssertionHelper
    {
        [Parallelizable]
        public abstract class ChannelTestClass : IParallelizable
        {
            [Fork]
            public abstract void ChannelTestMethod(int value);
            
            [Asynchronous]
            protected void ChannelTestMethod(int value, [Channel("TestChannel")] out int result)
            {
                result = value * value;
            }

            [Fork]
            public abstract void TwoChannelTestMethod(int value);

            [Asynchronous]
            protected void TwoChannelTestMethod(int value, [Channel("TestChannel1")] out int result1, [Channel("TestChannel2")] out string result2)
            {
                result1 = value * value;
                result2 = string.Format("->{0}<-", value);
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
            ParallelizationFactory.Parallelize(typeof(ChannelTestClass).Assembly);
        }

        [Test]
        public void OneChannel()
        {
            ChannelTestClass channelTestClass = ParallelizationFactory.GetParallelized<ChannelTestClass>();
            channelTestClass.ChannelTestMethod(5);

            int result = 0;

            Thread thread = new Thread(delegate() { result = ChannelFactory<int>.GetChannel(channelTestClass, "TestChannel").Receive(); });
            thread.Start();

            ThreadTimeout.Timeout(thread, 10000);
            
            Expect(result,EqualTo(25));

        }

        [Test]
        public void TwoChannels()
        {
            ChannelTestClass channelTestClass = ParallelizationFactory.GetParallelized<ChannelTestClass>();
            channelTestClass.TwoChannelTestMethod(23);

            int result1 = 0;
            string result2 = "";

            Thread thread = new Thread(delegate()
                                           {
                                               result1 = ChannelFactory<int>.GetChannel(channelTestClass, "TestChannel1").Receive();
                                               result2 = ChannelFactory<string>.GetChannel(channelTestClass, "TestChannel2").Receive();
                                           });
            thread.Start();

            ThreadTimeout.Timeout(thread, 10000);

            Expect(result1, EqualTo(529));
            Expect(result2, EqualTo("->23<-"));
        }

        [Test]
        public void InstanceChannels()
        {
            List<Thread> threads = new List<Thread>();
            bool[] successArray = new bool[100];
            for (int i = 1; i <= 100; i++)
            {
                int i1 = i;
                Thread thread = new Thread(delegate()
                                               {
                                                   TestForInstance(ParallelizationFactory.GetParallelized<ChannelTestClass>(), i1);
                                                   successArray[i1 - 1] = true;
                                               });
                threads.Add(thread);
                thread.Start();
            }

            foreach(Thread thread in threads)
            {
                if (!ThreadTimeout.Timeout(thread, 10000))
                    throw new TimeoutException();
            }

            foreach(bool success in successArray)
            {
                Expect(success);
            }
        }

        private readonly Random random = new Random();

        private void TestForInstance(ChannelTestClass instance, int value)
        {
            Thread.Sleep(random.Next(200));
            instance.ChannelTestMethod(value);

            int result = ChannelFactory<int>.GetChannel(instance, "TestChannel").Receive();
            Expect(result, EqualTo(value*value));
        }

        [Test]
        public void ManyValues()
        {
            ChannelTestClass channelTestClass = ParallelizationFactory.GetParallelized<ChannelTestClass>();

            for (int i = 1; i <= 1000; i++)
                channelTestClass.ChannelTestMethod(i);

            Thread.Sleep(100);

            List<int> result = new List<int>();

            Thread thread =
                new Thread(
                    delegate()
                        {
                            Channel<int> channel = ChannelFactory<int>.GetChannel(channelTestClass, "TestChannel");
                            for (int i = 1; i <= 1000; i++)
                            {
                                result.Add(channel.Receive());
                            }
                        });
            thread.Start();

            ThreadTimeout.Timeout(thread, 10000);

            for (int i = 1; i <= 1000; i++)
                Expect(result.Contains(i*i), String.Format("Missing number: {0}", i));
        }
    }
}
