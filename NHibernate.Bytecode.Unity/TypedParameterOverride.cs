using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Utility;

namespace NHibernate.Bytecode.Unity
{
    public class TypedParameterOverride : ResolverOverride
    {
        private readonly Queue<InjectionParameterValue> _parameterValues;

        public TypedParameterOverride(IEnumerable<object> parameterValues)
        {
            _parameterValues = new Queue<InjectionParameterValue>();
            foreach (var parameterValue in parameterValues)
            {
                _parameterValues.Enqueue(new InjectionParameter(parameterValue));
            }
        }

        public override IDependencyResolverPolicy GetResolver(IBuilderContext context, System.Type dependencyType)
        {
            Guard.ArgumentNotNull(context, "context");

            var currentOperation = context.CurrentOperation as ConstructorArgumentResolveOperation;

            return currentOperation != null && _parameterValues.Count > 0 ? _parameterValues.Dequeue().GetResolverPolicy(dependencyType) : GetDefault(dependencyType);
        }

        private IDependencyResolverPolicy GetDefault(System.Type t)
        {
            return new InjectionParameter(GetType().GetMethod("GetDefaultGeneric", BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance).MakeGenericMethod(t).Invoke(this, null)).GetResolverPolicy(t);
        }

        private T GetDefaultGeneric<T>()
        {
            return default(T);
        }
    }
}
