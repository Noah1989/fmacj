using System.Collections.Generic;
using System.Threading;

namespace Fmacj.Runtime
{
    public class Channel<T> 
    {
        private readonly EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        public string Name
        {
            get; private set;
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
            waitHandle.WaitOne();
            
            T result;

            lock (values)
            {
                result = values.Dequeue();

                if (values.Count > 0)
                    waitHandle.Set();
            }
            return result;
        }
    }
}