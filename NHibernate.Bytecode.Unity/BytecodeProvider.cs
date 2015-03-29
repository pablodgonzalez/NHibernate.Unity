using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Utility;
using NHibernate.Properties;

namespace NHibernate.Bytecode.Unity
{
    public class BytecodeProvider : AbstractBytecodeProvider
    {
        private readonly IUnityContainer _container;

        public BytecodeProvider()
            : this(new UnityContainer())
        {
        }

        public BytecodeProvider(IUnityContainer container)
        {
            Guard.ArgumentNotNull(container, "container");
            _container = container;
        }

        #region Implementation of IBytecodeProvider

        /// <summary>
        /// Retrieve the <see cref="T:NHibernate.Bytecode.IReflectionOptimizer"/> delegate for this provider
        ///             capable of generating reflection optimization components.
        /// </summary>
        /// <param name="clazz">The class to be reflected upon.</param><param name="getters">All property getters to be accessed via reflection.</param><param name="setters">All property setters to be accessed via reflection.</param>
        /// <returns>
        /// The reflection optimization delegate.
        /// </returns>
        public override IReflectionOptimizer GetReflectionOptimizer(System.Type clazz, IGetter[] getters, ISetter[] setters)
        {
            return new ReflectionOptimizer(_container, clazz, getters, setters);
        }

        /// <summary>
        /// The specific factory for this provider capable of
        ///             generating run-time proxies for lazy-loading purposes.
        /// </summary>
        public override IProxyFactoryFactory ProxyFactoryFactory
        {
            get { return new ProxyFactoryFactory(_container); }
        }

        /// <summary>
        /// NHibernate's object instaciator.
        /// </summary>
        /// <remarks>
        /// For entities <see cref="T:NHibernate.Bytecode.IReflectionOptimizer"/> and its implementations.
        /// </remarks>
        public override IObjectsFactory ObjectsFactory
        {
            get { return new ObjectFactory(_container); }
        }

        #endregion
    }
}
