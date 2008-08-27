using System;
namespace Fmacj.Framework
{
    public static class TypeValidator
    {
        public static bool IsParallelizable(Type type)
        {
            if (!typeof(IParallelizable).IsAssignableFrom(type)) return false;
            if (!type.IsClass) return false;
            if (!type.IsAbstract) return false;

            return true;
        }
    }
}