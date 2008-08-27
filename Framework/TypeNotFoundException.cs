namespace Fmacj.Framework
{
    public class TypeNotFoundException : ParallelizationException
    {
        public TypeNotFoundException() : base("The parallelized version of the requested type could not be found.")
        {
        }

        public TypeNotFoundException(string typeName)
            : base(string.Format("The parallelized version of '{0}' could not be found.", typeName))
        {
        }
    }
}