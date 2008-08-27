namespace Fmacj.Framework
{
    public class InvalidMethodException : ParallelizationException
    {
        public InvalidMethodException()
            : base(
                "A [Future], [Movable], [Asynchronous], [Chord] or [Join] method does not meet the required specifications for parallelization."
                )
        {
        }

        public InvalidMethodException(string typeName, string methodDescription, string reason)
            : base(
                string.Format(
                    "The method '{0}' of '{1}' is invalid due to the following reason: {2}",
                    methodDescription, typeName, reason))
        {
        }
    }
}