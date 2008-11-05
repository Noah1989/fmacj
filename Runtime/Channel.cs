using System.Collections.Generic;
using System.Threading;

namespace Fmacj.Runtime
{
    public class Channel<T> : IChannel
    {
        private readonly EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

        public Channel()
		{
		}

        private readonly Queue<T> values = new Queue<T>();

        public void Send(T value)
        {
            lock (values)
                values.Enqueue(value);
            waitHandle.Set();

        }

        public T Receive()
        {
            T result = default(T);
            bool recieved = false;

            // This loop is necessary because another thread possibly steals
            // the last element from the queue before we can make the lock below.
            // If this is the case we have to wait again.
            // Btw: Locking around waitHandle.WaitOne() is not a good idea
            // and causes an OutOfMemoryException.
            while (!recieved)
            {
                waitHandle.WaitOne();
                recieved = TryRecieve(ref result);
            }

            return result;
        }

        private bool TryRecieve(ref T result)
        {
            waitHandle.Reset();

            // Conditions have been created where this is lock is necessary
            // to prevent other threads from stealing the last element from
            // the queue just between the count check and the dequeuing.
            lock (values)
            {
                if (values.Count == 0) return false;
                result = values.Dequeue();
                if (values.Count == 0) return true;
                waitHandle.Set();
            }
            return true;
        }

        public WaitHandle WaitHandle
        {
            get { return waitHandle; }
        }

        public bool TryReceive(out object value)
        {
            T result = default(T);
            bool received = TryRecieve(ref result);
            value = result;
            return received;
        }
    }
}