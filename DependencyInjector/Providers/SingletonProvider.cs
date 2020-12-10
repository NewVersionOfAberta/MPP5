using System;

namespace DependencyInjector.Providers
{
    class SingletonProvider : IImplementationProvider
    {
        private readonly IImplementationProvider implementationProvider;
        private readonly object locker;
        private volatile object? instance;

        public SingletonProvider(IImplementationProvider implementationProvider)
        {
            this.implementationProvider = implementationProvider;
            locker = new Object();
        }

        public object ProvideImplementation()
        {
            if (instance == null)
            {
                lock(locker)
                {
                    if (instance == null)
                    {
                        instance = implementationProvider.ProvideImplementation();
                    }
                }
            }
            return instance;
        }
    }
}
