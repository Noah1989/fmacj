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
                            Bus bus = new Bus(new IChannel[] {channel},
											  new ChannelOptions[] {
												  new ChannelOptions(false)});					
					        object[] objects = bus.Receive();
					        bus.Close();
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
                            Bus bus = new Bus(new IChannel[] {channel1, channel2},
											  new ChannelOptions[] { 
											  	  new ChannelOptions(false),
												  new ChannelOptions(false)});;
					        object[] objects = bus.Receive();
					        bus.Close();
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
                            var bus = new Bus(new IChannel[] {channel1, channel2},
											  new ChannelOptions[] { 
											  	  new ChannelOptions(false),
												  new ChannelOptions(false)});
					
                            for (int i = 1; i <= 1000; i++)
                            {

                                object[] objects = bus.Receive();
                                Expect(objects.Length, EqualTo(2));
                                result1.Add((int) objects[0]);
                                result2.Add((double) objects[1]);
                            }
					        bus.Close();
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
