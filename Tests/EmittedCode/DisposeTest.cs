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
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using Fmacj.Core.Emitter;
using Fmacj.Core.Framework;
using Fmacj.Core.Runtime;
using NUnit.Framework;

namespace Fmacj.Tests.EmittedCode
{
    [TestFixture]
	public class DisposeTest : AssertionHelper
	{		
		[Parallelizable]
        public abstract class DisposeTestClass : IParallelizable
        {
			public bool DisposeCalled { get; private set; }
			
            public virtual void Dispose()
			{
				DisposeCalled = true;
			}			
        }

		[SetUp]
        public void SetUp()
        {
            ParallelizationFactory.Clear();
            ParallelizationFactory.Parallelize(typeof(DisposeTestClass).Assembly);
		}

		[Test]
		public void CallsBaseDispose()
		{
			DisposeTestClass disposeTestClass = ParallelizationFactory.GetParallelized<DisposeTestClass>();
			disposeTestClass.Dispose();
			Expect(disposeTestClass.DisposeCalled);
		}
	}
}