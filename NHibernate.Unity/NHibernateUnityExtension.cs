// ---------------------------------------------------------------
// <autor>Pablo González</autor>
// <web>www.odra.com.ar</web>
// <blog>blog.odra.com.ar</blog>
// <email>pablo.gonzalez@odra.com.ar</email>
// <copyrigth>http://www.odra.com.ar/LGPL-3.0.txt</copyrigth>
// ---------------------------------------------------------------

using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Microsoft.Practices.Unity.ObjectBuilder;
using NHibernate.Cfg;

namespace NHibernate.Unity
{
    public class NHibernateUnityExtension : UnityContainerExtension
    {
        private static IUnityContainer _container;
        private static ExtensionContext _context;
        private readonly bool _lazyBuildSessionfactory;

        [InjectionConstructor]
        public NHibernateUnityExtension()
            : this(false)
        {
        }

        public NHibernateUnityExtension(bool lazyBuildSessionfactory)
        {
            _lazyBuildSessionfactory = lazyBuildSessionfactory;
        }

        protected override void Initialize()
        {
            if (_container != null)
            {
                _context.Registering -= OnContextRegistering;
                _context.RegisteringInstance -= OnContextRegisteringInstance;
                _context.Policies.ClearDefault<SessionFactoryPolicy>();
            }
            _container = Container;
            _context = Context;
            _context.Registering += OnContextRegistering;
            _context.RegisteringInstance += OnContextRegisteringInstance;
            _context.Strategies.AddNew<SessionFactoryStrategy>(UnityBuildStage.PreCreation);
            _context.Policies.SetDefault(new SessionFactoryPolicy(_lazyBuildSessionfactory));
            if (_container.Configure<Interception>() == null)
            {
                _container.AddNewExtension<Interception>();
            }
        }

        private static void OnContextRegisteringInstance(object sender, RegisterInstanceEventArgs e)
        {
            if (!typeof(Configuration).IsAssignableFrom(e.RegisteredType)) return;

            e.LifetimeManager = new ContainerControlledLifetimeManager();
            _context.Policies.Get<SessionFactoryPolicy>(null).RegisterSessionFactory(e.Name, (Configuration)e.Instance);
        }

        private static void OnContextRegistering(object sender, RegisterEventArgs e)
        {
            if (typeof(ISessionFactory).IsAssignableFrom(e.TypeFrom) || typeof(ISessionFactory).IsAssignableFrom(e.TypeTo))
            {
                throw new NHibernateUnityException("The type {0} cannot be registered on unity.", typeof(ISessionFactory).AssemblyQualifiedName);
            }
        }

        internal static IUnityContainer CurrentContainer
        {
            get
            {
                InitializeDefaultIfNot();
                return _container;
            }
        }

        internal static IPolicyList CurrrentPolicies
        {
            get
            {
                InitializeDefaultIfNot();
                return _context.Policies;
            }
        }

        private static void InitializeDefaultIfNot()
        {
            if (_container == null)
            {
                new UnityContainer().AddNewExtension<NHibernateUnityExtension>();
            }
        }
    }
}
