using System;
using System.Reflection;
using System.Runtime.Serialization;
using Fmacj.Emitter;
using Fmacj.Framework;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Fmacj.Tests
{
    [TestFixture]
    public class ParallelizationFactoryTest : AssertionHelper
    {
        [Parallelizable]
        public abstract class ParallelizationFactoryTestClass : IParallelizable
        {
        }
        
        [SetUp]
        public void SetUp()
        {
            ParallelizationFactory.Clear();
        }

        [Test]
        [ExpectedException(ExceptionType=typeof(InvalidOperationException))]
        public void NotParallelizedAssembly()
        {
            ParallelizationFactory.GetParallelized<ParallelizationFactoryTestClass>();
        }

        [Test]
        [ExpectedException(ExceptionType = typeof(InvalidOperationException))]
        public void MultipleParallelizedAssembly()
        {
            ParallelizationFactory.Parallelize(typeof(ParallelizationFactoryTestClass).Assembly);
            ParallelizationFactory.Parallelize(typeof(ParallelizationFactoryTestClass).Assembly);
        }

        [Test]
        public void MinimalClass()
        {
            ParallelizationFactory.Parallelize(typeof(ParallelizationFactoryTestClass).Assembly);
            Expect(ParallelizationFactory.GetParallelized<ParallelizationFactoryTestClass>(), Is.Not.Null);
        }
    }
}
