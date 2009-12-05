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
