using System;
using System.Collections;
using System.Collections.Generic;

namespace Fmacj.Runtime
{	
	internal class ChannelEnumerator<T> : IEnumerator<T>
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
		}

	}
}
