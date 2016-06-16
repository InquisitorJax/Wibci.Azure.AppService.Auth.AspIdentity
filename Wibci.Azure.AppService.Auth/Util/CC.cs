namespace Wibci.Azure.AppService.Auth.Util
{
    public class CC
    {
        private static IDependencyContainer _container;

        public static IDependencyContainer IoC
        {
            get
            {
                if (_container == null)
                    _container = new DefaultDependencyContainer();
                return _container;
            }
        }

        public static void RegisterIocContainer(IDependencyContainer container)
        {
            _container = container;
        }
    }
}