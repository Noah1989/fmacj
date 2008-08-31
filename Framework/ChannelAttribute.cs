using System;

namespace Fmacj.Framework
{
    [AttributeUsage(
        AttributeTargets.Parameter,
        AllowMultiple = false,
        Inherited = true)
    ]
    public sealed class ChannelAttribute : Attribute
    {
        public ChannelAttribute(string name)
        {
            Name = name; 
        }

        public string Name { get; private set; }
    }
}