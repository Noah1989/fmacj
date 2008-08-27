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
            _name = name; 
        }

        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }
    }
}