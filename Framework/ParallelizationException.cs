using System;

namespace Fmacj.Framework
{
    public class ParallelizationException : Exception
    {
        public ParallelizationException() : base("There has been an error during parallelization")
        {
        }

        public ParallelizationException(string message) : base(message)
        {
        }

        public ParallelizationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}