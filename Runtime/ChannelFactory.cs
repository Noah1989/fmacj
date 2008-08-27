using System.Collections.Generic;
using Fmacj.Framework;

namespace Fmacj.Runtime
{
    public static class ChannelFactory<T>
    {
        private static readonly Dictionary<IParallelizable, Dictionary<string, Channel<T>>> channels =
            new Dictionary<IParallelizable, Dictionary<string, Channel<T>>>();

        public static Channel<T> GetChannel(IParallelizable instance, string name)
        {

            Dictionary<string, Channel<T>> instanceChannels;
            
            lock(channels)
                if (!channels.TryGetValue(instance, out instanceChannels))
                {
                    instanceChannels = new Dictionary<string, Channel<T>>();
                    channels.Add(instance, instanceChannels);
                }

            Channel<T> result;

            lock(instanceChannels)
                if (!instanceChannels.TryGetValue(name, out result))
                {
                    result = new Channel<T>(name);
                    instanceChannels.Add(name, result);
                }

            return result;
        }

    }
}
