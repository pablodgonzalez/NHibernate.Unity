//---------------------------------------------------------------
// <autor>Pablo Gonzalez</autor>
// <blog>blog.odra.com.ar</blog>
// <email>pgonzalez@odra.com.ar</email>
//---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Iesi.Collections.Generic;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Proxy;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Bytecode.Unity.Tests.ProxyInterface
{
    [TestFixture]
    public class CustomProxyFixture : TestCase
    {
        protected override Cfg.Configuration Cfg
        {
            get { return Container.Resolve<Cfg.Configuration>("CustomTestSource"); }
        }

        protected override ISessionFactoryImplementor Sessions
        {
            get { return (ISessionFactoryImplementor)Container.Resolve<ISessionFactory>("CustomTestSource"); }
        }

        protected override void Configure(Configuration configuration)
        {
            base.Configure(configuration);
            configuration.Proxy(p => p.ProxyFactoryFactory<CustomProxyFactoryFactory>());
        }

        [Test]
        public void CanImplementNotifyPropertyChanged()
        {
            using (ISession s = OpenSession())
            {
                s.Save(new Blog("blah"));
                s.Flush();
            }

            using (ISession s = OpenSession())
            {
                var blog = (Blog)s.Load(typeof(Blog), 1);
                var propertyChanged = (INotifyPropertyChanged)blog;
                string propChanged = null;
                propertyChanged.PropertyChanged +=
                    delegate(object sender, PropertyChangedEventArgs e) { propChanged = e.PropertyName; };

                blog.BlogName = "foo";
                Assert.AreEqual("BlogName", propChanged);
            }

            using (ISession s = OpenSession())
            {
                s.Delete("from Blog");
                s.Flush();
            }
        }
    }

    public class CustomProxyFactoryFactory : IProxyFactoryFactory
    {
        #region IProxyFactoryFactory Members

        public IProxyFactory BuildProxyFactory()
        {
            return new DataBindingProxyFactory();
        }

        public IProxyValidator ProxyValidator
        {
            get { return new DynProxyTypeValidator(); }
        }

        public bool IsInstrumented(System.Type entityClass)
        {
            return false;
        }

        public bool IsProxy(object entity)
        {
            return entity is IInterceptingProxy;
        }

        #endregion
    }

    public class DataBindingProxyFactory : ProxyFactory
    {
        public DataBindingProxyFactory()
            : base(new UnityContainer())
        {
        }

        public override INHibernateProxy GetProxy(object id, ISessionImplementor session)
        {
            try
            {
                var list = new List<System.Type>(Interfaces) { typeof(INotifyPropertyChanged) };
                System.Type[] interfaces = list.ToArray();

                var initializer = new DataBindingInterceptor(EntityName, PersistentClass, id, GetIdentifierMethod,
                                                             SetIdentifierMethod, ComponentIdType, session, interfaces);

                var generatedProxy = Container.Resolve(KeyType, EntityName);

                (generatedProxy as IInterceptingProxy).AddInterceptionBehavior(initializer);

                return (INHibernateProxy)generatedProxy;
            }
            catch (Exception e)
            {
                log.Error("Creating a proxy instance failed", e);
                throw new HibernateException("Creating a proxy instance failed", e);
            }
        }
        public override void PostInstantiate(string entityName, System.Type persistentClass, Iesi.Collections.Generic.ISet<System.Type> interfaces, MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod, IAbstractComponentType componentIdType)
        {
            var interfacesplus = new OrderedSet<System.Type>(interfaces) { typeof(INotifyPropertyChanged) };
            base.PostInstantiate(entityName, persistentClass, interfacesplus, getIdentifierMethod, setIdentifierMethod, componentIdType);
        }
    }

    public class DataBindingInterceptor : LazyInitializer
    {
        private PropertyChangedEventHandler _subscribers = delegate { };

        public DataBindingInterceptor(string entityName, System.Type persistentClass, object id,
                                      MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod,
                                      IAbstractComponentType componentIdType, ISessionImplementor session,
                                      System.Type[] requiredInterfaces)
            : base(
                entityName, persistentClass, id, getIdentifierMethod, setIdentifierMethod, componentIdType, session,
                requiredInterfaces)
        {
        }

        public override IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            if (input.MethodBase.DeclaringType == typeof(INotifyPropertyChanged))
            {
                var propertyChangedEventHandler = (PropertyChangedEventHandler)input.Arguments[0];
                if (input.MethodBase.Name.StartsWith("add_"))
                {
                    _subscribers += propertyChangedEventHandler;
                }
                else
                {
                    _subscribers -= propertyChangedEventHandler;
                }
                return new VirtualMethodReturn(input, null, null);
            }

            IMethodReturn result = base.Invoke(input, getNext);

            if (input.MethodBase.Name.StartsWith("set_"))
            {
                _subscribers(this, new PropertyChangedEventArgs(input.MethodBase.Name.Substring(4)));
            }

            return result;
        }
    }
}