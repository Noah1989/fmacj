﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace Fmacj.Runtime
{
    public class Bus
    {
        private readonly IChannel[] channels;
        private readonly Dictionary<IChannel, object> data = new Dictionary<IChannel, object>();
        private readonly Dictionary<IChannel, RegisteredWaitHandle> registeredWaitHandles = new Dictionary<IChannel, RegisteredWaitHandle>();
        private readonly EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        private int receivedCount;
        private bool dataAvailable;
        
        public Bus(IChannel[] channels)
        {
            this.channels = channels;
            Reset();
        }

        private void Reset()
        {
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
            if(channel.TryReceive(out value))
            {
                data[channel] = value;
                receivedCount++;
            }
            else
            {
                RegisterChannel(channel);
            }

            if (receivedCount == channels.Length)
                dataAvailable = true;
                waitHandle.Set();
        }

        public object[] Receive()
        {
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

        public WaitHandle WaitHandle { get { return waitHandle; } }
    }
}
