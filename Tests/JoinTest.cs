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
        }

        [SetUp]
        public void SetUp()
        {
            ParallelizationFactory.Clear();
            ParallelizationFactory.Parallelize(typeof(JoinTestClass).Assembly);
        }

        [Test]
        public void Test()
        {
            JoinTestClass foo = ParallelizationFactory.GetParallelized<JoinTestClass>();
            
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
