using System;
using Xunit;
using DependencyInjector.Exceptions;

namespace DependencyInjector.Tests
{
    public class DependenciesConfigurationTests
    {
        [Fact]
        public void Register_ImplInheritedFromDependency_NoExceptionThrown()
        {
            DependenciesConfiguration dependencies = new DependenciesConfiguration();

            dependencies.Register<IRepository, Repository>();
        }

        [Fact]
        public void Register_ImplNotInheritedFromDependency_ExceptionThrown()
        {
            DependenciesConfiguration dependencies = new DependenciesConfiguration();

            Assert.Throws<ConfigurationException>(() => dependencies.Register(typeof(Repository), typeof(IRepository)));
        }

        [Fact]
        public void Register_AsSelf_NoExceptionThrown()
        {
            DependenciesConfiguration dependencies = new DependenciesConfiguration();

            dependencies.Register<Repository, Repository>();
        }

        [Fact]
        public void Register_OpenGenericTypeWithCorrectInheritance_NoExceptionThrown()
        {
            DependenciesConfiguration dependencies = new DependenciesConfiguration();

            dependencies.Register(typeof(IService<>), typeof(ServiceImpl<>));
        }

        [Fact]
        public void Register_OpenGenericTypeWithIncorrectInheritance_ExceptionThrown()
        {
            DependenciesConfiguration dependencies = new DependenciesConfiguration();

            Assert.Throws<ConfigurationException>(() => dependencies.Register(typeof(ServiceImpl<>), typeof(IService<>)));
        }

        [Fact]
        public void Register_ImplIsAbstract_ExceptionThrown()
        {
            DependenciesConfiguration dependencies = new DependenciesConfiguration();

            Assert.Throws<ConfigurationException>(() => dependencies.Register<IRepository, IMySqlRepository>());
        }

        [Fact]
        public void Register_SamedependencyRegisteredSecondTime_ExceptionThrown()
        {
            DependenciesConfiguration dependencies = new DependenciesConfiguration();

            dependencies.Register<IRepository, Repository>();

            Assert.Throws<ConfigurationException>(() => dependencies.Register<IRepository, Repository>());
        }

        [Fact]
        public void Register_ImplWithScalarParamInConstructor_ExceptionThrown()
        {
            DependenciesConfiguration dependencies = new DependenciesConfiguration();

            Assert.Throws<ConfigurationException>(() => dependencies.Register<IRepository, ReposityryWithWrongConstructor>());
        }

        [Fact]
        public void Register_ImplWithDuplicateName_ExceptionThrown()
        {
            DependenciesConfiguration dependencies = new DependenciesConfiguration();

            dependencies.Register<IRepository, Repository>("TestName");

            Assert.Throws<ConfigurationException>(() => dependencies.Register<IRepository, Repository2>("TestName"));
        }
    }

    class ReposityryWithWrongConstructor : IRepository
    {
        public ReposityryWithWrongConstructor(int foo) { }
    }
}
