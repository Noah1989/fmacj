using System.Runtime.Serialization;
using Fmacj.Framework;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Fmacj.Tests
{
    [TestFixture]
    public class TypeValidatorTest : AssertionHelper
    {
        [Test]
        public void AbstractClass()
        {
            Expect(TypeValidator.IsParallelizable(typeof(AbstractTestClass)), Is.True);
        }
        private abstract class AbstractTestClass : IParallelizable
        {
            public abstract void Dispose();
        }

        [Test]
        public void NonAbstractClass()
        {
            Expect(TypeValidator.IsParallelizable(typeof(NonAbstractTestClass)), Is.False);
        }
        private class NonAbstractTestClass : IParallelizable
        {
            public void Dispose()
            {
                throw new System.NotImplementedException();
            }
        }

        [Test]
        public void NonImplementingAbstractClass()
        {
            Expect(TypeValidator.IsParallelizable(typeof(NonImplementingAbstractTestClass)), Is.False);
        }        
        private abstract class NonImplementingAbstractTestClass {}
        
        [Test]
        public void Struct()
        {
            Expect(TypeValidator.IsParallelizable(typeof(TestStruct)), Is.False);
        }
        private struct TestStruct : IParallelizable
        {
            public void Dispose()
            {
                throw new System.NotImplementedException();
            }
        }

        [Test]
        public void Interface()
        {
            Expect(TypeValidator.IsParallelizable(typeof (ITestInterface)), Is.False);
        }
        private interface ITestInterface : IParallelizable {}
    }
}
