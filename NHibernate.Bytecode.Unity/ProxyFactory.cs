// ---------------------------------------------------------------
// <autor>Pablo González</autor>
// <web>www.odra.com.ar</web>
// <blog>blog.odra.com.ar</blog>
// <email>pablo.gonzalez@odra.com.ar</email>
// <copyrigth>http://www.odra.com.ar/LGPL-3.0.txt</copyrigth>
// ---------------------------------------------------------------

using System;
using System.Linq;
using Common.Logging;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using NHibernate.Engine;
using NHibernate.Intercept;
using NHibernate.Proxy;
using Unity.InterceptionExtension.Serialization.Serializable;

namespace NHibernate.Bytecode.Unity
{
    public class ProxyFactory : AbstractProxyFactory
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(ProxyFactory));

        protected readonly IUnityContainer Container;

        public ProxyFactory(IUnityContainer container)
        {
            Container = container;
            Container.AddNewExtension<NHibernateProxyUnityExtension>();
        }

        /// <summary>
        /// Build a proxy using the Unity.Interception library.
        /// </summary>
        /// <param name="id">The value for the Id.</param>
        /// <param name="session">The Session the proxy is in.</param>
        /// <returns>A fully built <c>INHibernateProxy</c>.</returns>
        public override INHibernateProxy GetProxy(object id, ISessionImplementor session)
        {
            try
            {
                var initializer = new LazyInitializer(EntityName, PersistentClass, id, GetIdentifierMethod,
                                                          SetIdentifierMethod, ComponentIdType, session, Interfaces);

                var proxy = Container.Resolve(KeyType, EntityName);

                (proxy as IInterceptingProxy).AddInterceptionBehavior(initializer);

                return (INHibernateProxy)proxy;
            }
            catch (Exception e)
            {
                log.Error("Creating a proxy instance failed", e);
                throw new HibernateException("Creating a proxy instance failed", e);
            }
        }

        public override object GetFieldInterceptionProxy(object instanceToWrap)
        {
            AddAdditionalInterface(typeof(IFieldInterceptorAccessor));

            var proxy = Container.Resolve(KeyType, EntityName);

            (proxy as IInterceptingProxy).AddInterceptionBehavior(new LazyFieldInterceptor(instanceToWrap, Interfaces));

            return proxy;
        }

        public override void PostInstantiate(string entityName, System.Type persistentClass, Iesi.Collections.Generic.ISet<System.Type> interfaces, System.Reflection.MethodInfo getIdentifierMethod, System.Reflection.MethodInfo setIdentifierMethod, Type.IAbstractComponentType componentIdType)
        {
            base.PostInstantiate(entityName, persistentClass, interfaces, getIdentifierMethod, setIdentifierMethod, componentIdType);

            if (!Container.IsRegistered(KeyType, EntityName))
            {
                var injectionMember = interfaces
                  .Select(
                      t => new AdditionalInterface(t))
                  .Cast<InjectionMember>()
                  .Concat(new[] { new SerializableInterceptor<VirtualMethodInterceptor>() }).ToArray();

                Container.RegisterType(
                          KeyType,
                          EntityName,
                          new TransientLifetimeManager(),
                          injectionMember);

            }
            else
            {
                interfaces.ToList().ForEach(AddAdditionalInterface);
            }
        }

        protected System.Type KeyType
        {
            get { return IsClassProxy ? PersistentClass : typeof(object); }
        }

        private void AddAdditionalInterface(System.Type @interface)
        {
            new AdditionalInterface(@interface).AddPolicies(null, KeyType, EntityName, Container.Configure<NHibernateProxyUnityExtension>().Policies);
        }
    }
}