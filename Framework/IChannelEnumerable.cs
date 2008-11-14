using System;
using System.Collections.Generic;

namespace Fmacj.Framework
{	
	public interface IChannelEnumerable<T> : IEnumerable<T>
	{
		IEnumerable<T> GetNext(int count);
	}
}
