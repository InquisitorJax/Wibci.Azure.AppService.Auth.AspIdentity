using System;

namespace Wibci.Azure.AppService.Auth.Util
{
    public interface IDependencyContainer
    {
        T Resolve<T>();
    }

    internal class DefaultDependencyContainer : IDependencyContainer
    {
        public T Resolve<T>()
        {
            //TODO: Implement Autofac resolve
            throw new NotImplementedException();
        }
    }
}