using System;

namespace Fmacj.Framework
{
    [AttributeUsage(
        AttributeTargets.Class,
        AllowMultiple = false,
        Inherited = true)
    ]
    public sealed class ParallelizableAttribute : Attribute
    {
    }
}
