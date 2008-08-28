using System.Collections.Generic;
using System.Threading;

namespace Fmacj.Runtime
{
    public class Channel<T>
    {
        private readonly EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        public string Name
        {
            get;
            private set;
        }

        internal Channel(string name)
        {
            Name = name;
        }

        private readonly Queue<T> values = new Queue<T>();

        public void Send(T value)
        {

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

                // Conditions have been created where this is lock is necessary
                // to prevent other threads from stealing the last element from
                // the queue just between the count check and the dequeuing.
                lock (values)
                {
                    if (values.Count == 0) continue;
                    result = values.Dequeue();
                    recieved = true;
                    if (values.Count == 0) continue;
                    waitHandle.Set();
                }
            }
            return result;
        }
    }
}