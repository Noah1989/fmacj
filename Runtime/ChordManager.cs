using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Fmacj.Runtime
{
    public static class ChordManager
    {
        public static void RegisterChord(IChannel[] channels, WaitOrTimerCallback callback, ref EventHandler onDisposing)
        {
            if(channels.Length==0)
                throw new ArgumentException("There must be at least one channel provided for a chord.", "channels");

            Bus bus = new Bus(channels);
            RegisterBus(bus, callback);

            onDisposing += delegate { UnregisterBus(bus); };
        }

        private static void UnregisterBus(Bus bus)
        {
            bus.Close();
        }

        public static void RegisterBus(Bus bus, WaitOrTimerCallback callback)
        {
            if (bus.IsClosed)
                throw new InvalidOperationException("Cannot register Bus. Bus is Closed.");

			if (bus.RegisteredWaitHandle != null)
				bus.RegisteredWaitHandle.Unregister(null);
			
			
            bus.RegisteredWaitHandle = ThreadPool.RegisterWaitForSingleObject(bus.WaitHandle, callback, bus, -1, true);
        }
    }
}
