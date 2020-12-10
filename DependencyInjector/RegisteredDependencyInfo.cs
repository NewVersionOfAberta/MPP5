using System;

namespace DependencyInjector
{
    public enum DependencyLifetime
    {
        Transient,
        Singleton
    }

    class RegisteredDependencyInfo
    {
        public Type DependencyType
        { get; }

        public Type ImplementationType
        { get; }

        public DependencyLifetime Lifetime
        { get; }

        public object? Name
        { get; }

        public RegisteredDependencyInfo(Type dependencyType, Type implementationType, DependencyLifetime lifetime, object? name)
        {
            DependencyType = dependencyType;
            ImplementationType = implementationType;
            Lifetime = lifetime;
            Name = name;
        }

        public RegisteredDependencyInfo(Type dependencyType, Type implementationType, DependencyLifetime lifetime)
        {
            DependencyType = dependencyType;
            ImplementationType = implementationType;
            Lifetime = lifetime;
        }
    }
}
