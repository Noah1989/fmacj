namespace Fmacj.Framework
{
    public class InconsistentChannelTypeException : ParallelizationException
    {
        public InconsistentChannelTypeException()
            : base(
                "A channel is being used with different channel types."
                )
        {
        }

        public InconsistentChannelTypeException(string channelName, string typeName)
            : base(
                string.Format(
                    "A channel '{0}' of '{1}' is being used with different channel types.",
                    channelName, typeName))
        {
        }
    }
}