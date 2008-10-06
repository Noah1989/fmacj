using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Fmacj.Framework;
using Fmacj.Emitter;
using NUnit.Framework;

namespace Fmacj.Tests
{
    [Ignore]
    [TestFixture]
    public class FooTest : AssertionHelper
    {
        [Parallelizable]
        public abstract class Foo : IParallelizable
        {
            [Fork]
            public abstract void Bar(int val);

            [Fork]
            public abstract void Baz(int val);

            [Movable]
            protected void Bar(int val, [Channel("Bar")] out int result)
            {
                result = val*val;
            }

            [Asynchronous]
            protected void Baz(int val, [Channel("Baz")] out double result)
            {
                result = 1/(double)val;
            }

            [Chord]
            protected double Sum([Channel("Bar")] int bar, [Channel("Baz")] double baz)
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
            ParallelizationFactory.Parallelize(typeof(Foo).Assembly);
        }

        [Test]
        public void Test()
        {
            Foo foo = ParallelizationFactory.GetParallelized<Foo>();
            foo.Bar(2);
            foo.Baz(3);
            Expect(foo.Sum(), EqualTo(2*2 + 1/3));
        }
    }
}
