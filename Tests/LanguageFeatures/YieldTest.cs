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
using Fmacj.Emitter;
using Fmacj.Framework;
using NUnit.Framework;

namespace Fmacj.Tests
{
    [TestFixture]
    public class YieldTest : AssertionHelper
    {
        [Parallelizable]
        public abstract class YieldTestClass : IParallelizable
        {
         	[Yield]
			public abstract void YieldTestChannel([Channel("TestChannel")] int value1);

			[Yield]
			public abstract void YieldMultipleChannels([Channel("TestChannel1")] int value1, [Channel("TestChannel2")] int value2);
			
            public abstract void Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            ParallelizationFactory.Clear();
            ParallelizationFactory.Parallelize(typeof(YieldTestClass).Assembly);
        }

        [Test]
        public void Yield()
        {        
			using (YieldTestClass yieldTestClass = ParallelizationFactory.GetParallelized<YieldTestClass>())
			{
				yieldTestClass.YieldTestChannel(23);
				
				int result = 0;

				Thread thread = new Thread(delegate() { result = ChannelResolver<int>.GetChannel(yieldTestClass, "TestChannel").Receive(); });
				thread.Start();
				
				ThreadTimeout.Timeout(thread, 10000);
				
				Expect(result,EqualTo(23));
			}
		}

		[Test]
        public void YieldMultipleChannels()
        {        
			using (YieldTestClass yieldTestClass = ParallelizationFactory.GetParallelized<YieldTestClass>())
			{
				yieldTestClass.YieldMultipleChannels(23, 42);
				
				int result = 0;

				Thread thread = new Thread(delegate() { result = ChannelResolver<int>.GetChannel(yieldTestClass, "TestChannel1").Receive()
															   + ChannelResolver<int>.GetChannel(yieldTestClass, "TestChannel2").Receive(); });
				thread.Start();
				
				ThreadTimeout.Timeout(thread, 10000);
				
				Expect(result,EqualTo(65));
			}
		}
	}
}