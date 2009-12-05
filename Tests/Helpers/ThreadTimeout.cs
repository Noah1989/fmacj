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
