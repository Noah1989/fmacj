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
using Fmacj.Core.Emitter;
using Fmacj.Core.Framework;
using Fmacj.Core.Runtime;
using NUnit.Framework;

namespace Fmacj.Tests.Runtime
{
    [TestFixture]
    public class BusTest : AssertionHelper
    {
//        [Parallelizable]
//        public abstract class BusTestClass : IParallelizable
//        {
//            [Fork]
//            public abstract void TestMethod1(int value);
//            [Asynchronous]
//            protected void TestMethod1(int value, [Channel("TestChannel1")] out int result)
//            {
//                result = value*value;
//            }
//
//            [Fork]
//            public abstract void TestMethod2(int value);
//            [Asynchronous]
//            protected void TestMethod2(int value, [Channel("TestChannel2")] out double result)
//            {
//                result = 1/(double)value;
//            }
//
//            [Fork]
//            public abstract void TestMethod3(int value);
//            [Asynchronous]
//            protected void TestMethod3(int value, [Channel("TestChannel3")] out int result)
//            {
//                result = -value;
//            }
//
//            public abstract void Dispose();
//        }
//
//        [SetUp]
//        public void SetUp()
//        {
//            ParallelizationFactory.Clear();
//            ParallelizationFactory.Parallelize(typeof(BusTestClass).Assembly);
//        }

        [Test]
        public void OneChannel()
        {            
			Channel<int> channel = new Channel<int>(); 
			
			channel.Send(5);
			
            int result = 0;

            Bus bus = new Bus(new IChannel[] {channel},
							  new ChannelOptions[] {new ChannelOptions(false)});					
	        object[] objects = bus.Receive();
	        bus.Close();
            Expect(objects.Length, EqualTo(1));
            result = (int) objects[0];

            Expect(result, EqualTo(5));
        }

        [Test]
        public void TwoChannels()
        {

			Channel<int> channel1 = new Channel<int>();
			Channel<double> channel2 = new Channel<double>();
			
			channel1.Send(2);
			channel2.Send(3.5);
			
			double result = 0;
			
            Bus bus = new Bus(new IChannel[] {channel1, channel2},
							  new ChannelOptions[] { 
							  	  new ChannelOptions(false),
								  new ChannelOptions(false)});;
	        object[] objects = bus.Receive();
	        bus.Close();
            Expect(objects.Length, EqualTo(2));
            result = (int) objects[0] + (double) objects[1];
 
            Expect(result, EqualTo(2 + 3.5));
        }

        [Test]
        public void ManyValues()
        {            
			Channel<int> channel1 = new Channel<int>();
            Channel<double> channel2 = new Channel<double>();
                          			
            for (int i = 1; i <= 1000; i++)
            {
                channel1.Send(i);
                channel2.Send(-i*0.5);
            }
            Thread.Sleep(100);

            List<int> result1 = new List<int>();
            List<double> result2 = new List<double>();

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
                      
            for (int i = 1; i <= 1000; i++)
                Expect(result1.Contains(i), String.Format("Missing number: {0} ({1} received)", i, result1.Count));

            for (int i = 1; i <= 1000; i++)
                Expect(result2.Contains(-i*0.5), String.Format("Missing number: {0} ({1} received)", i, result1.Count));
        }
    }
}
