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
using System.Threading;
using Fmacj.Framework;
using Fmacj.Emitter;
using NUnit.Framework;

namespace Fmacj.Tests
{
    [TestFixture]
    public class JoinTest : AssertionHelper
    {
        [Parallelizable]
        public abstract class JoinTestClass : IParallelizable
        {
			[Fork]
            public abstract void TestMethod1(int val);
            [Asynchronous]
            protected void TestMethod1(int val, [Channel("TestChannel1")] out int result)
            {				
                result = val*val;				
            }
            
            [Fork]
            public abstract void TestMethod2(int val);
            [Asynchronous]
            protected void TestMethod2(int val, [Channel("TestChannel2")] out double result)
            {				
                result = 1/(double)val;				
            } 

            [Chord]
            protected double SimpleJoin([Channel("TestChannel1")] int bar, [Channel("TestChannel2")] double baz)
            {				
                return bar + baz;				
            }
            [Join]
            public abstract double SimpleJoin();


 			[Fork]
            public abstract void TestMethod3(int value);
            [Asynchronous]
            protected void TestMethod3(int value, [Channel("TestChannel3")] out int result)
            {				
                result = value ;
            }

			[Fork]
            public abstract void TestMethod4(double value);
            [Asynchronous]
            protected void TestMethod4(double value, [Channel("TestChannel4")] out double result)
            {
                result = value;
            }

            [Chord]
            protected string OutChannelChordJoin([Channel("TestChannel3")] int value1, [Channel("TestChannel4")] double value2, 
            							      [Channel("OutChannel1")] out double result1, [Channel("OutChannel2")] out string result2)
            {
                result1 = value1 / value2;
                result2 = "Test";
                return result1.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
            [Join]
            public abstract string OutChannelChordJoin();

			
			[Fork]
            public abstract void TestMethod5(int val);
            [Asynchronous]
            protected void TestMethod5(int val, [Channel("TestChannel5")] out double result)
            {					
                result = -val;								
            } 

            [Chord]
            protected double ParameterJoin(double val1, [Channel("TestChannel5")] double val2)
            {				
                return val1 + val2;				
            }
            [Join]
            public abstract double ParameterJoin(double val1);


            public abstract void Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            ParallelizationFactory.Clear();
            ParallelizationFactory.Parallelize(typeof(JoinTestClass).Assembly);
        }

        [Test]
        public void ForkChordAndJoin()
        {
            using (JoinTestClass joinTestClass = ParallelizationFactory.GetParallelized<JoinTestClass>())
			{
				joinTestClass.TestMethod1(2);
				joinTestClass.TestMethod2(3);
				
				double result = 0;
				
				Thread thread = new Thread(delegate() { result = joinTestClass.SimpleJoin(); });
				thread.Start();
				ThreadTimeout.Timeout(thread, 10000);
				
				Expect(result, EqualTo(2*2 + 1.0/3));            
			}
		}

        [Test]
        public void JoinWithOutChannelChord()
        {
            using (JoinTestClass joinTestClass = ParallelizationFactory.GetParallelized<JoinTestClass>())
			{
				joinTestClass.TestMethod3(23);
				joinTestClass.TestMethod4(5);
				
				string result = "";
				double result1 = 0;
				string result2 = "";

				Thread thread = new Thread(delegate() { result = joinTestClass.OutChannelChordJoin();
														result1 = ChannelResolver<double>.GetChannel(joinTestClass, "OutChannel1").Receive();
														result2 = ChannelResolver<string>.GetChannel(joinTestClass, "OutChannel2").Receive(); });
				thread.Start();
				
				ThreadTimeout.Timeout(thread, 10000);

				Expect(result, EqualTo("4.6"));  
				Expect(result1,EqualTo(4.6));
				Expect(result2,EqualTo("Test"));
			}
		}
		
		[Test]
        public void ParameterJoin()
        {
            using (JoinTestClass joinTestClass = ParallelizationFactory.GetParallelized<JoinTestClass>())
			{
				joinTestClass.TestMethod5(2);				
				
				double result = 0;
				
				Thread thread = new Thread(delegate() { result = joinTestClass.ParameterJoin(5); });
				thread.Start();
				ThreadTimeout.Timeout(thread, 10000);
				Expect(result, EqualTo(3));            
			}
		}
	}
}
