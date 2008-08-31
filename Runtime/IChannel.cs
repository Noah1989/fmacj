using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Fmacj.Runtime
{
    public interface IChannel
    {
        WaitHandle WaitHandle { get; }
        bool TryReceive(out object value);
    }
}
