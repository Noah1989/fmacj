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

            public abstract void Dispose();
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
			using (ChannelTestClass channelTestClass = ParallelizationFactory.GetParallelized<ChannelTestClass>())
			{
				channelTestClass.ChannelTestMethod(5);
				
				int result = 0;

				Thread thread = new Thread(delegate() { result = ChannelResolver<int>.GetChannel(channelTestClass, "TestChannel").Receive(); });
				thread.Start();
				
				ThreadTimeout.Timeout(thread, 10000);
				
				Expect(result,EqualTo(25));
			}
        }

        [Test]
        public void TwoChannels()
        {
            using (ChannelTestClass channelTestClass = ParallelizationFactory.GetParallelized<ChannelTestClass>())
			{
				channelTestClass.TwoChannelTestMethod(23);

				int result1 = 0;
				string result2 = "";
				
				Thread thread = new Thread(delegate()
				                           {
					result1 = ChannelResolver<int>.GetChannel(channelTestClass, "TestChannel1").Receive();
                                               result2 = ChannelResolver<string>.GetChannel(channelTestClass, "TestChannel2").Receive();
				});
				thread.Start();

				ThreadTimeout.Timeout(thread, 10000);

				Expect(result1, EqualTo(529));
				Expect(result2, EqualTo("->23<-"));
			}
        }

        [Test]
        public void InstanceChannels()
        {
            List<Thread> threads = new List<Thread>();
            bool[] successArray = new bool[20];
            for (int i = 1; i <= 20; i++)
            {
                int i1 = i;
                Thread thread = new Thread(delegate()
                                               {
					                               using (ChannelTestClass instance = ParallelizationFactory.GetParallelized<ChannelTestClass>())
						                               TestForInstance(instance, i1);
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

            int result = ChannelResolver<int>.GetChannel(instance, "TestChannel").Receive();
            Expect(result, EqualTo(value*value));
        }

        [Test]
        public void ManyValues()
        {
            using (ChannelTestClass channelTestClass = ParallelizationFactory.GetParallelized<ChannelTestClass>())
			{            
				for (int i = 1; i <= 1000; i++)
					channelTestClass.ChannelTestMethod(i);
				
				Thread.Sleep(100);
				
				List<int> result = new List<int>();
				
				Thread thread =
					new Thread(
                    delegate()
					           {
                            Channel<int> channel = ChannelResolver<int>.GetChannel(channelTestClass, "TestChannel");
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
}