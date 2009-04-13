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

namespace Fmacj.Runtime
{	
	internal class ChannelEnumerator<T> : IEnumerator<T>, IDisposable
	{		
		private Channel<T> channel;
		private T current;
		
		public ChannelEnumerator(Channel<T> channel)
		{
			if(channel == null)
				throw new ArgumentNullException("channel");
			
			this.channel = channel;
		}
		
		public void Reset()
		{
			throw new NotSupportedException("Reset() is not supported on channel enumerators");
		}
		
		public bool MoveNext()
		{
			current = channel.Receive();
			return true; 
		}
		
		public T Current
		{
			get { return current; }
		}		
		
		object IEnumerator.Current
		{
			get { return current; }
		}
		
		public void Dispose()
		{
			OnDisposed(EventArgs.Empty);
		}

		private void OnDisposed(EventArgs e)
		{
			if(Disposed != null) Disposed(this, e);
		}
		
		public event EventHandler Disposed;
		
	}
}
