namespace Fmacj.Framework
{
    public class InvalidTypeException : ParallelizationException
    {
        public InvalidTypeException()
            : base(
                "A type is marked as [Parallelizable] but does not implement IParallelizable or is not an abstract class."
                )
        {
        }

        public InvalidTypeException(string typeName)
            : base(
                string.Format(
                    "The type '{0}' is marked as [Parallelizable] but does not implement IParallelizable or is not an abstract class.",
                    typeName))
        {
        }
    }
}