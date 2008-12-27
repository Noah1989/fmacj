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
            public abstract void Bar(int val);

            [Fork]
            public abstract void Baz(int val);

            [Asynchronous]
            protected void Bar(int val, [Channel("bar")] out int result)
            {				
                result = val*val;				
            }

            [Asynchronous]
            protected void Baz(int val, [Channel("baz")] out double result)
            {				
                result = 1/(double)val;				
            }

            [Chord]
            protected double Sum([Channel("bar")] int bar, [Channel("baz")] double baz)
            {				
                return bar + baz;				
            }

            [Join]
            public abstract double Sum();

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
            using (JoinTestClass foo = ParallelizationFactory.GetParallelized<JoinTestClass>())
			{
				foo.Bar(2);
				foo.Baz(3);
				
				double result = 0;
				
				Thread thread = new Thread(delegate() { result = foo.Sum(); });
				thread.Start();
				ThreadTimeout.Timeout(thread, 10000);
				
				Expect(result, EqualTo(2*2 + 1.0/3));            
			}
		}
	}
}