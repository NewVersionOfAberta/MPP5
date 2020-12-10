using System;

namespace DependencyInjector
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ImplementationNameAttribute : Attribute
    {
        public object Name
        { get; }

        public ImplementationNameAttribute(object name)
        {
            Name = name;
        }
    }
}
