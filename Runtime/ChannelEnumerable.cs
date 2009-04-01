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
using System.Collections;
using System.Collections.Generic;
using Fmacj.Framework;

namespace Fmacj.Runtime
{	
	public class ChannelEnumerable<T> : IChannelEnumerable<T>
	{
		private bool used = false;
		
		private Channel<T> channel;
				
		public ChannelEnumerable(Channel<T> channel)
		{
			this.channel = channel;
		}

		public IEnumerator<T> GetEnumerator()
		{
			if (used) throw new InvalidOperationException("ChannelEnumerable can only be enumerated once.");
			used = true;
			return new ChannelEnumerator<T>(channel);
		}		
		
		IEnumerator IEnumerable.GetEnumerator()
		{			
			return GetEnumerator();			
		}
	}
}
