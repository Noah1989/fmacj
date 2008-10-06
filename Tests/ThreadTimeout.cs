using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Fmacj.Tests
{
    internal static class ThreadTimeout
    {
        public static bool Timeout(Thread thread, int timeout)
        {
            bool result = true;
            Thread timeoutThread =
                new Thread(
                    delegate()
                        {
                            Thread.Sleep(timeout);
                            if (thread.ThreadState != ThreadState.Stopped)
                            {
                                result = false;
                                thread.Abort();
                            }
                        });
            timeoutThread.Start();
            thread.Join();
            return result;
        }
    }
}
