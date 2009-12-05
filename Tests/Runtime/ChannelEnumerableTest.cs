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
using System.Linq;
using Fmacj.Framework;
using Fmacj.Runtime;
using NUnit.Framework;

namespace Fmacj.Tests
{
	[TestFixture]	
	public class ChannelEnumerableTest : AssertionHelper
	{		
		[Test]
		public void Enumerate()
		{
			Channel<int> channel = new Channel<int>();
			
			for (int i = 0; i < 10; i++)
				channel.Send(i);

			IChannelEnumerable<int> enumerable = new ChannelEnumerable<int>(channel);

			int n = 0;
			foreach(int i in enumerable.Take(10))
				Expect(i, EqualTo(n++));
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void EnumerateOnlyOnce()
		{
			Channel<int> channel = new Channel<int>();
			
			for (int i = 0; i < 10; i++)
				channel.Send(i);

			IChannelEnumerable<int> enumerable = new ChannelEnumerable<int>(channel);

			int n = 0;
			foreach(int i in enumerable.Take(5))
				Expect(i, EqualTo(n++));
			
			foreach(int i in enumerable.Take(5))
				Expect(i, EqualTo(n++));
		}
	}
}
