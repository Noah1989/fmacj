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
            public abstract void Dispose();
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
