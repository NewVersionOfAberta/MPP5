using System;
using System.Collections.Generic;
using Xunit;

namespace DependencyInjector.Tests
{
    public class DependencyProviderTests
    {
        [Fact]
        public void Resolve_TransientLifetime_ReturnsDifferentObjects()
        {
            DependenciesConfiguration dependencies = new DependenciesConfiguration();
            dependencies.Register<IRepository, Repository>(DependencyLifetime.Transient);
            DependencyProvider provider = new DependencyProvider(dependencies);

            IRepository firstResolve = provider.Resolve<IRepository>();
            IRepository secondResolve = provider.Resolve<IRepository>();

            Assert.True(firstResolve != secondResolve);
        }

        [Fact]
        public void Resolve_SingletonLifetime_ReturnsSameObjects()
        {
            DependenciesConfiguration dependencies = new DependenciesConfiguration();
            dependencies.Register<IRepository, Repository>(DependencyLifetime.Singleton);
            DependencyProvider provider = new DependencyProvider(dependencies);

            IRepository firstResolve = provider.Resolve<IRepository>();
            IRepository secondResolve = provider.Resolve<IRepository>();

            Assert.True(firstResolve == secondResolve);
        }

        [Fact]
        public void Resolve_AsSelfRegistration_ReturnsSameObjects()
        {
            DependenciesConfiguration dependencies = new DependenciesConfiguration();
            dependencies.Register<Repository, Repository>(DependencyLifetime.Singleton);
            DependencyProvider provider = new DependencyProvider(dependencies);

            Repository repository = provider.Resolve<Repository>();
        }

        [Fact]
        public void Resolve_ExsistingImplName_ReturnsCorrectImpl()
        {
            DependenciesConfiguration dependencies = new DependenciesConfiguration();
            dependencies.Register<IRepository, Repository>("First");
            dependencies.Register<IRepository, Repository2>("Second");
            DependencyProvider provider = new DependencyProvider(dependencies);

            IRepository repository = provider.Resolve<IRepository>("Second");

            Assert.IsType<Repository2>(repository);
        }

        [Fact]
        public void Resolve_RequestEnumerable_ReturnsAllImplementations()
        {
            DependenciesConfiguration dependencies = new DependenciesConfiguration();
            dependencies.Register<IRepository, Repository>(DependencyLifetime.Singleton, "First");
            dependencies.Register<IRepository, Repository2>(DependencyLifetime.Singleton, "Second");
            DependencyProvider provider = new DependencyProvider(dependencies);

            IRepository firstRepository = provider.Resolve<IRepository>("First");
            IRepository secondRepository = provider.Resolve<IRepository>("Second");
            IEnumerable<IRepository> repositories = provider.Resolve<IEnumerable<IRepository>>();

            Assert.Collection(repositories, 
                repository => Assert.Same(firstRepository, repository), 
                repository => Assert.Same(secondRepository, repository)
                );
        }

        [Fact]
        public void Resolve_NameAttributeInConstructor_ChoosesCorrectImpl()
        {
            DependenciesConfiguration dependencies = new DependenciesConfiguration();
            dependencies.Register<IRepository, Repository>(DependencyLifetime.Singleton, "First");
            dependencies.Register<IRepository, Repository2>(DependencyLifetime.Singleton, "Second");
            dependencies.Register<IProvider, ProviderWithConstructor>();
            DependencyProvider provider = new DependencyProvider(dependencies);

            IProvider repository = provider.Resolve<IProvider>();
            IRepository secondRepository = provider.Resolve<IRepository>("Second");

            Assert.Same(secondRepository, ((ProviderWithConstructor)repository).Repository);
        }

        [Fact]
        public void Resolve_EnumerableInConstructor_ChoosesAllImplementations()
        {
            DependenciesConfiguration dependencies = new DependenciesConfiguration();
            dependencies.Register<IRepository, Repository>(DependencyLifetime.Singleton, "First");
            dependencies.Register<IRepository, Repository2>(DependencyLifetime.Singleton, "Second");
            dependencies.Register<IProvider, ProviderWithEnumerableConstructor>();
            DependencyProvider provider = new DependencyProvider(dependencies);

            IProvider repository = provider.Resolve<IProvider>();
            IRepository firstRepository = provider.Resolve<IRepository>("First");
            IRepository secondRepository = provider.Resolve<IRepository>("Second");

            IEnumerable<IRepository> repositories = ((ProviderWithEnumerableConstructor) repository).Repositories;
            Assert.Collection(repositories,
                repository => Assert.Same(firstRepository, repository),
                repository => Assert.Same(secondRepository, repository)
                );
        }

        [Fact]
        public void Resolve_Generic_ReturnsCorrectType()
        {
            DependenciesConfiguration dependencies = new DependenciesConfiguration();
            dependencies.Register<IRepository, Repository>();
            dependencies.Register<IService<IRepository>, ServiceImpl<IRepository>>();
            DependencyProvider provider = new DependencyProvider(dependencies);

            IService<IRepository> service = provider.Resolve<IService<IRepository>>();

            Assert.IsType<ServiceImpl<IRepository>>(service);
        }

        [Fact]
        public void Resolve_OpenGeneric_ReturnsCorrectType()
        {
            DependenciesConfiguration dependencies = new DependenciesConfiguration();
            dependencies.Register<IRepository, Repository>();
            dependencies.Register(typeof(IService<>), typeof(ServiceImpl<>));
            DependencyProvider provider = new DependencyProvider(dependencies);

            IService<IRepository> service = provider.Resolve<IService<IRepository>>();

            Assert.IsType<ServiceImpl<IRepository>>(service);
        }
    }

    interface IRepository { }

    interface IMySqlRepository : IRepository { }

    class Repository : IRepository { }
    class Repository2 : IRepository { }

    interface IProvider { }

    class ProviderWithConstructor : IProvider
    {
        public IRepository Repository
        { get; }

        public ProviderWithConstructor([ImplementationName("Second")] IRepository repository)
        {
            Repository = repository;
        }
    }

    class ProviderWithEnumerableConstructor : IProvider
    {
        public IEnumerable<IRepository> Repositories
        { get; }

        public ProviderWithEnumerableConstructor(IEnumerable<IRepository> repositories)
        {
            Repositories = repositories;
        }
    }

    interface IService<TRepository> where TRepository : IRepository
    {
    }

    class ServiceImpl<TRepository> : IService<TRepository> where TRepository : IRepository
    {
        public ServiceImpl(TRepository repository)
        { }
    }
}
