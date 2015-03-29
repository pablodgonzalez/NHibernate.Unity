// ---------------------------------------------------------------
// <autor>Pablo González</autor>
// <web>www.odra.com.ar</web>
// <blog>blog.odra.com.ar</blog>
// <email>pablo.gonzalez@odra.com.ar</email>
// <copyrigth>http://www.odra.com.ar/LGPL-3.0.txt</copyrigth>
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;
using Common.Logging;
using Iesi.Collections.Generic;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using NHibernate.Engine;
using NHibernate.Intercept;
using NHibernate.Proxy;
using NHibernate.Type;
using NHibernate.Unity;
using Unity.InterceptionExtension.Serialization.Serializable;

namespace NHibernate.Bytecode.Unity
{
    public class ProxyFactory : AbstractProxyFactory
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(ProxyFactory));

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

                var proxy = NHibernateUnityExtension.CurrentContainer.Resolve(KeyType, EntityName);

                (proxy as IInterceptingProxy).AddInterceptionBehavior(initializer);

                return (INHibernateProxy)proxy;
            }
            catch (Exception e)
            {
                log.Error("Creating a proxy instance failed", e);
                throw new HibernateException("Creating a proxy instance failed", e);
            }
        }

        public override object GetFieldInterceptionProxy()
        {
            AddAdditionalInterface(typeof(IFieldInterceptorAccessor));

            var proxy = NHibernateUnityExtension.CurrentContainer.Resolve(KeyType, EntityName);

            (proxy as IInterceptingProxy).AddInterceptionBehavior(new LazyFieldInterceptor(Interfaces));

            return proxy;
        }

        public override void PostInstantiate(string entityName, System.Type persistentClass, ISet<System.Type> interfaces, MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod, IAbstractComponentType componentIdType)
        {
            base.PostInstantiate(entityName, persistentClass, interfaces, getIdentifierMethod, setIdentifierMethod, componentIdType);

            if (!NHibernateUnityExtension.CurrentContainer.IsRegistered(KeyType, EntityName))
            {
                var injectionMember = interfaces
                  .Select(
                      t => new AdditionalInterface(t))
                  .Cast<InjectionMember>()
                  .Concat(new[] { new SerializableInterceptor<VirtualMethodInterceptor>() }).ToArray();

                NHibernateUnityExtension.CurrentContainer.RegisterType(
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

        private System.Type KeyType
        {
            get { return IsClassProxy ? PersistentClass : typeof(object); }
        }

        private void AddAdditionalInterface(System.Type @interface)
        {
            new AdditionalInterface(@interface).AddPolicies(null, KeyType, EntityName, NHibernateUnityExtension.CurrrentPolicies);
        }
    }
}