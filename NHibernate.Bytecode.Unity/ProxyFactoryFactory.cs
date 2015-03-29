// ---------------------------------------------------------------
// <autor>Pablo González</autor>
// <web>www.odra.com.ar</web>
// <blog>blog.odra.com.ar</blog>
// <email>pablo.gonzalez@odra.com.ar</email>
// <copyrigth>http://www.odra.com.ar/LGPL-3.0.txt</copyrigth>
// ---------------------------------------------------------------

using Microsoft.Practices.Unity;
using NHibernate.Proxy;

namespace NHibernate.Bytecode.Unity
{
    public class ProxyFactoryFactory : IProxyFactoryFactory
    {
        private readonly IUnityContainer _container;


        public ProxyFactoryFactory()
            : this(new UnityContainer())
        {
        }

        public ProxyFactoryFactory(IUnityContainer container)
        {
            _container = container;
        }

        #region IProxyFactoryFactory Members

        public IProxyFactory BuildProxyFactory()
        {
            return new ProxyFactory(_container);
        }

        public IProxyValidator ProxyValidator
        {
            get { return new DynProxyTypeValidator(); }
        }

        public bool IsInstrumented(System.Type entityClass)
        {
            return true;
        }

        public bool IsProxy(object entity)
        {
            return entity is INHibernateProxy;
        }

        #endregion
    }
}