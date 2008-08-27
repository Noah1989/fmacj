using System;

namespace Fmacj.Framework
{
    [AttributeUsage(
        AttributeTargets.Method,
        AllowMultiple = false,
        Inherited=true)
    ]
    public sealed class AsynchronousAttribute : Attribute
    {
    }
}