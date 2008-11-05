using System.Reflection;
using Fmacj.Framework;
using Fmacj.Runtime;

namespace Fmacj.Tests
{
    public static class ChannelResolver<T>
    {
        public static Channel<T> GetChannel(IParallelizable instance, string name)
        {			
            return (Channel<T>) instance.GetType().GetField(name+"Channel", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(instance);
        }

    }
}