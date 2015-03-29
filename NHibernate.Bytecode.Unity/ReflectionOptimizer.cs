using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Utility;
using NHibernate.Properties;
using BaseReflectionOptimizer = NHibernate.Bytecode.Lightweight.ReflectionOptimizer;

namespace NHibernate.Bytecode.Unity
{
    public class ReflectionOptimizer : BaseReflectionOptimizer
    {
        private readonly IUnityContainer _container;

        public ReflectionOptimizer(IUnityContainer container, System.Type mappedType, IGetter[] getters, ISetter[] setters)
            : base(mappedType, getters, setters)
        {
            Guard.ArgumentNotNull(container, "container");
            _container = container;
        }

        public override object CreateInstance()
        {
            return _container.IsRegistered(mappedType) ? _container.Resolve(mappedType) : base.CreateInstance();
        }

        protected override void ThrowExceptionForNoDefaultCtor(System.Type type)
        {
        }
    }
}
