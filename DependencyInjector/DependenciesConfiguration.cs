using DependencyInjector.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DependencyInjector
{
    public class DependenciesConfiguration
    {
        private readonly static string NO_SUITABLE_CONSTRUCTOR_MESSAGE_FORMAT = "No constructor that contains only classes and interfaces as parameters found for tpye {0}";
        private readonly static string INHERITANCE_ERROR_MESSAGE_FORMAT = "Type {0} should inherite {1}";
        private readonly static string IMPL_ALREADY_REGISTERED_MESSAGE_FORMAT = "Implementation {0} is already registered";
        private readonly static string ABSTRACT_IMPL_MESSAGE_FORMAT = "Implementation {0} is an abstract class";

        private const DependencyLifetime defaultLifetime = DependencyLifetime.Transient;

        internal Dictionary<Type, List<RegisteredDependencyInfo>> RegisteredDependencies
        { get; }

        public DependenciesConfiguration()
        {
            RegisteredDependencies = new Dictionary<Type, List<RegisteredDependencyInfo>>();
        }

        public void Register<TDependency, TImplementation>(DependencyLifetime lifetime = defaultLifetime) where TImplementation : TDependency
        {
            InnerRegister(typeof(TDependency), typeof(TImplementation), lifetime, null);
        }

        public void Register<TDependency, TImplementation>(object name) where TImplementation : TDependency
        {
            InnerRegister(typeof(TDependency), typeof(TImplementation), defaultLifetime, name);
        }

        public void Register<TDependency, TImplementation>(DependencyLifetime lifetime, object name) where TImplementation : TDependency
        {
            InnerRegister(typeof(TDependency), typeof(TImplementation), lifetime, name);
        }

        public void Register(Type dependency, Type implementation, DependencyLifetime lifetime = defaultLifetime)
        {
            ValidateInharitance(dependency, implementation);
            InnerRegister(dependency, implementation, lifetime, null);
        }

        public void Register(Type dependency, Type implementation, object name)
        {
            ValidateInharitance(dependency, implementation);
            InnerRegister(dependency, implementation, defaultLifetime, name);
        }

        public void Register(Type dependency, Type implementation, DependencyLifetime lifetime, object name)
        {
            ValidateInharitance(dependency, implementation);
            InnerRegister(dependency, implementation, lifetime, name);
        }

        private void ValidateInharitance(Type dependency, Type implementation)
        {
            if ((!dependency.IsGenericTypeDefinition && dependency.IsAssignableFrom(implementation)) || !dependency.OpenIsAssignableFrom(implementation))
            {
                throw new ConfigurationException(string.Format(INHERITANCE_ERROR_MESSAGE_FORMAT, implementation.Name, dependency.Name));
            }
        }

        private void InnerRegister(Type dependency, Type implementation, DependencyLifetime lifetime, object? name)
        {
            ValidateAbstract(implementation);
            ValidateConstructors(implementation);

            RegisteredDependencyInfo dependencyInfo = new RegisteredDependencyInfo(dependency, implementation, lifetime, name);
            if (!RegisteredDependencies.ContainsKey(dependency))
            {
                RegisteredDependencies.Add(dependency, new List<RegisteredDependencyInfo>() { dependencyInfo });
            }
            else
            {
                List<RegisteredDependencyInfo> dependencyInfos = RegisteredDependencies[dependency];
                ValidateImplementationDuplication(dependencyInfos, implementation, dependencyInfo.Name);
                dependencyInfos.Add(dependencyInfo);
            }
        }

        private void ValidateAbstract(Type implementation)
        {
            if (implementation.IsAbstract)
            {
                throw new ConfigurationException(string.Format(ABSTRACT_IMPL_MESSAGE_FORMAT, implementation.Name));
            }
        }

        private void ValidateImplementationDuplication(List<RegisteredDependencyInfo> dependencyInfos, Type implementation, object implementationName)
        {
            if (dependencyInfos.Select(dependency => dependency.ImplementationType).Any(type => type.Equals(implementation))
                || (implementation.Name != null && 
                dependencyInfos.Select(dependency => dependency.Name)
                .Where(name => name != null)
                .Any(name => name.Equals(implementationName)))
                )
            {
                throw new ConfigurationException(string.Format(IMPL_ALREADY_REGISTERED_MESSAGE_FORMAT, implementation.Name));
            }
        }

        private void ValidateConstructors(Type implementation)
        {
            if (!ContainsSuitableConstructor(implementation))
            {
                throw new ConfigurationException(String.Format(NO_SUITABLE_CONSTRUCTOR_MESSAGE_FORMAT, implementation.Name));
            }
        }

        private bool ContainsSuitableConstructor(Type implementationType)
        {
            ConstructorInfo suitableConstructor = implementationType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).
                        OrderBy(constructor => constructor.GetParameters().Length).First();
            return suitableConstructor.GetParameters().All(parameter => 
                    parameter.ParameterType.IsInterface || parameter.ParameterType.IsClass);
        }
    }
}
