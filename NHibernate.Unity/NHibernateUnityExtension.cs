// ---------------------------------------------------------------
// <autor>Pablo González</autor>
// <web>www.odra.com.ar</web>
// <blog>blog.odra.com.ar</blog>
// <email>pablo.gonzalez@odra.com.ar</email>
// <copyrigth>http://www.odra.com.ar/LGPL-3.0.txt</copyrigth>
// ---------------------------------------------------------------

using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;
using NHibernate.Bytecode.Unity;
using NHibernate.Cfg;

namespace NHibernate.Unity
{
    public class NHibernateUnityExtension : UnityContainerExtension
    {
        private readonly bool _lazyBuildSessionfactory;
        private readonly bool _useBytecodeProvider;
        private readonly bool _useProxyFactory;

        [InjectionConstructor]
        public NHibernateUnityExtension()
            : this(false, true, true)
        {
        }

        public NHibernateUnityExtension(bool lazyBuildSessionfactory, bool useBytecodeProvider, bool useProxyFactory)
        {
            _lazyBuildSessionfactory = lazyBuildSessionfactory;
            _useBytecodeProvider = useBytecodeProvider;
            _useProxyFactory = useProxyFactory;
        }

        protected override void Initialize()
        {
            if (_useBytecodeProvider)
            {
                Environment.BytecodeProvider = new BytecodeProvider(Container);
            }
            Context.Registering += OnContextRegistering;
            Context.RegisteringInstance += OnContextRegisteringInstance;
            Context.Strategies.AddNew<SessionFactoryStrategy>(UnityBuildStage.PreCreation);
            Context.Policies.SetDefault(new SessionFactoryPolicy());
        }

        private void OnContextRegisteringInstance(object sender, RegisterInstanceEventArgs e)
        {
            if (!typeof(Configuration).IsAssignableFrom(e.RegisteredType)) return;

            e.LifetimeManager = new ContainerControlledLifetimeManager();
            var configuration = (Configuration)e.Instance;
            if (!_useBytecodeProvider && _useProxyFactory)
            {
                configuration.Proxy(p => p.ProxyFactoryFactory<ProxyFactoryFactory>());
            }
            Context.Policies.Get<SessionFactoryPolicy>(null).RegisterSessionFactory(e.Name, (Configuration)e.Instance, _lazyBuildSessionfactory);
        }

        private void OnContextRegistering(object sender, RegisterEventArgs e)
        {
            if (typeof(ISessionFactory).IsAssignableFrom(e.TypeFrom) || typeof(ISessionFactory).IsAssignableFrom(e.TypeTo))
            {
                throw new NHibernateUnityException("The type {0} cannot be registered on unity.", typeof(ISessionFactory).AssemblyQualifiedName);
            }
        }
    }
}
