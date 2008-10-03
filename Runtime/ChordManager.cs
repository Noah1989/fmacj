﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Fmacj.Runtime
{
    public static class ChordManager
    {
        public static void RegisterChord(IChannel[] channels, WaitOrTimerCallback callback)
        {
            if(channels.Length==0)
                throw new ArgumentException("There must be at least one channel provided for a chord.", "channels");

            Bus bus = new Bus(channels);
            ThreadPool.RegisterWaitForSingleObject(bus.WaitHandle, callback, bus, -1, false);
        }
        
    }
}
