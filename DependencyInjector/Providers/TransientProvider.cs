using System;
using System.Linq;
using System.Reflection;

namespace DependencyInjector.Providers
{
    class TransientProvider : IImplementationProvider
    {
        private readonly DependencyProvider dependencyProvider;
        private readonly Type implementationType;
        private readonly ConstructorInfo suitableConstructor;

        public TransientProvider(DependencyProvider dependencyProvider, Type implementationType)
        {
            this.dependencyProvider = dependencyProvider;
            this.implementationType = implementationType;

            suitableConstructor = SelectLowestParamsCountConstructor(
                implementationType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance));
        }

        private ConstructorInfo SelectLowestParamsCountConstructor(ConstructorInfo[] constructorInfos)
        {
            return constructorInfos.OrderBy(constructor => constructor.GetParameters().Length).First();
        }

        public object ProvideImplementation()
        {
            return CreateObjectInstance(suitableConstructor);
        }

        private object CreateObjectInstance(ConstructorInfo suitableConstructor)
        {
            ParameterInfo[] parameterInfos = suitableConstructor.GetParameters();
            object[] parameters = GetParametersByInfos(parameterInfos);
            return suitableConstructor.Invoke(parameters);
        }

        private object[] GetParametersByInfos(ParameterInfo[] parameterInfos)
        {
            object[] parameters = new object[parameterInfos.Length];
            for (int i = 0; i < parameterInfos.Length; i++)
            {
                object? name = TryGetImplementationName(parameterInfos[i]);
                parameters[i] = dependencyProvider.Resolve(parameterInfos[i].ParameterType, name);
            }
            return parameters;
        }

        private object? TryGetImplementationName(ParameterInfo parameterInfo)
        {
            ImplementationNameAttribute implementationName = parameterInfo.GetCustomAttribute<ImplementationNameAttribute>();
            if (implementationName != null)
                return implementationName.Name;
            return null;
        }
    }
}
