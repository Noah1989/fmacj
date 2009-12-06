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
using System.Collections.Specialized;
using System.Threading;
using Fmacj.Core.Framework;

namespace Fmacj.Core.Runtime
{
    public class Bus
    {
        private readonly IChannel[] channels;
        private readonly OrderedDictionary data = new OrderedDictionary();
        private readonly Dictionary<IChannel, RegisteredWaitHandle> registeredWaitHandles = new Dictionary<IChannel, RegisteredWaitHandle>();
        private readonly Dictionary<IChannel, ChannelOptions> channelOptions = new Dictionary<IChannel, ChannelOptions>();
		private readonly Dictionary<IChannel, IChannelEnumerable> channelEnumerables = new Dictionary<IChannel, IChannelEnumerable>();

		
		private readonly EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        private readonly EventWaitHandle publicWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
		
		
		private RegisteredWaitHandle registeredWaitHandle;
		
        private int receivedCount;
        private bool dataAvailable;
        private bool isClosed;

        public Bus(IChannel[] channels, ChannelOptions[] options)
        {
			if(channels.Length != options.Length)
				throw new ArgumentException("The number of channels must be equal to the number of channel options provided.");
			
			for(int i = 0; i < channels.Length; i++)
				channelOptions.Add(channels[i], options[i]);
			
            isClosed = false;
            this.channels = channels;
            Reset();
        }

        private void Reset()
        {
            if (isClosed)
                throw new InvalidOperationException("Cannot reset Bus. Bus is closed.");

            dataAvailable = false;
            data.Clear();
            registeredWaitHandles.Clear();						
            receivedCount = 0;	

            foreach (IChannel channel in channels)
            {
                data.Add(channel, null);
                RegisterChannel(channel);
            }
        }

        private void RegisterChannel(IChannel channel)
        {
            lock (registeredWaitHandles)
            {
				if (isClosed) return;					
				
				registeredWaitHandles.Add(channel,
                                          ThreadPool.RegisterWaitForSingleObject(channel.WaitHandle, ChannelCallback,
                                                                                 channel, -1, true));
            }
        }

        private void ChannelCallback(object state, bool timedOut)
        {			
            IChannel channel = state as IChannel;
            if (channel == null) throw new ArgumentException("The given object must be an IChannel.", "state");

            lock (registeredWaitHandles)
            {
                registeredWaitHandles[channel].Unregister(null);
                registeredWaitHandles.Remove(channel);
            }

            object value;
			if(channelOptions[channel].Enumerable)
			{
				if(channelEnumerables.ContainsKey(channel))
				{
					channelEnumerables[channel].WaitForRelease();
					channelEnumerables.Remove(channel);
				}
				IChannelEnumerable channelEnumerable = channel.GetEnumerable();
				data[channel] = channelEnumerable;
				channelEnumerables.Add(channel, channelEnumerable);
			}
			else if(channel.TryReceive(out value))
            {
                data[channel] = value;                
            }
            else
            {
                RegisterChannel(channel);
				return;
            }
			
			Interlocked.Increment(ref receivedCount);

            // lock here because
            // setting dataAvailable
            lock (data)
            {
                if (receivedCount == channels.Length)
				{
                    dataAvailable = true;
                	waitHandle.Set();
                	publicWaitHandle.Set();
				}
            }
        }

        public object[] Receive()
        {
            if (isClosed)
                throw new InvalidOperationException("Cannot receive from Bus. Bus is closed.");

            object[] result = null;
            bool recieved = false;

            while (!recieved)
            {
                waitHandle.WaitOne();
                
                recieved = TryRecieve(ref result);
            }

            return result;
        }

        private bool TryRecieve(ref object[] result)
        {
            waitHandle.Reset();
            
            lock (data)
            {
                if (!dataAvailable) return false;
                result = new object[data.Count];
                data.Values.CopyTo(result, 0);
                Reset();
            }
            return true;
        }

        public WaitHandle WaitHandle { get { return publicWaitHandle; } }
		
		internal RegisteredWaitHandle RegisteredWaitHandle 
		{
			get { return registeredWaitHandle; } 
			set { registeredWaitHandle = value; }
		}         

        public void Close()
        {
            if (isClosed)
                throw new InvalidOperationException("Cannot close Bus. Bus is already closed.");

			lock (registeredWaitHandles)
            {
				foreach (RegisteredWaitHandle handle in registeredWaitHandles.Values)
					handle.Unregister(null);
				
				if(registeredWaitHandle != null)
					registeredWaitHandle.Unregister(null);
				
				isClosed = true;
			}
        }

        public bool IsClosed
        {
            get { return isClosed; }
        }
    }
}
