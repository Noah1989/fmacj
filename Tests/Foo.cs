﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Fmacj.Framework;
using Fmacj.Emitter;
using NUnit.Framework;

namespace Fmacj.Tests
{
    [TestFixture]
    public class FooTest : AssertionHelper
    {
        [Serializable]
        [Parallelizable]
        public abstract class Foo : IParallelizable
        {
            protected Foo()
            {
            }

            protected Foo(SerializationInfo info, StreamingContext context)
            {
            }

            [Future]
            public abstract void Bar(int val);

            [Future]
            public abstract void Baz(int val);

            [Movable]
            protected void Bar(int val, [Channel("Bar")] out int result)
            {
                result = val*val;
            }

            [Asynchronous]
            protected void Baz(int val, [Channel("Baz")] out int result)
            {
                result = 1/val;
            }

            [Chord]
            protected int Sum([Channel("Bar")] int bar, [Channel("Baz")] int baz)
            {
                return bar + baz;
            }

            [Join]
            public abstract int Sum();

            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
            public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                // no data.
            }

            public object Clone()
            {
                return GetType().GetConstructor(new Type[] {}).Invoke(new object[] {});
            }
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