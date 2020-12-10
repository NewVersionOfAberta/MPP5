using System;

namespace DependencyInjector.Exceptions
{
    public class DependencyNotRegisteredException : Exception
    {
        private readonly static string MESSAGE_FORMAT = "Dependency of type {0} is not registered";
        private readonly static string MESSAGE_WITH_OBJECT_FORMAT = "Dependency of type {0} with name {1} is not registered";
        private readonly string message;

        public override string Message
        { 
            get { return message; } 
        }

        public DependencyNotRegisteredException(Type dependencyType)
        {
            message = string.Format(MESSAGE_FORMAT, dependencyType.Name);
        }

        public DependencyNotRegisteredException(Type dependencyType, object name)
        {
            message = string.Format(MESSAGE_WITH_OBJECT_FORMAT, dependencyType.Name, name);
        }
    }
}
