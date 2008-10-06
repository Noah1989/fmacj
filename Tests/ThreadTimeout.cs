using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Fmacj.Tests
{
    internal static class ThreadTimeout
    {
        public static void Timeout(Thread thread, int timeout)
        {
            Thread timeoutThread =
                new Thread(
                    delegate()
                    {
                        Thread.Sleep(timeout);
                        thread.Abort();
                    });
            timeoutThread.Start();
            thread.Join();
            timeoutThread.Abort();
        }
    }
}
