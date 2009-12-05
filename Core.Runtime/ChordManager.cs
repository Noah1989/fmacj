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

namespace Fmacj.Runtime
{
    public static class ChordManager
    {
        public static void RegisterChord(IChannel[] channels, ChannelOptions[] options, WaitOrTimerCallback callback, ref EventHandler onDisposing)
        {
            if(channels.Length==0)
                throw new ArgumentException("There must be at least one channel provided for a chord.", "channels");

            Bus bus = new Bus(channels, options);
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
