// ---------------------------------------------------------------
// <autor>Pablo González</autor>
// <web>www.odra.com.ar</web>
// <blog>blog.odra.com.ar</blog>
// <email>pablo.gonzalez@odra.com.ar</email>
// <copyrigth>http://www.odra.com.ar/LGPL-3.0.txt</copyrigth>
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity.InterceptionExtension;
using NHibernate.Intercept;
using NHibernate.Util;

namespace NHibernate.Bytecode.Unity
{
    /// <summary>
    /// Unity don't implement mixin how Castle wherever we can get same result.
    /// </summary>
    [Serializable]
    public class LazyFieldInterceptor : IFieldInterceptorAccessor, IInterceptionBehavior
    {
        private readonly System.Type[] _requiredInterfaces;

        public LazyFieldInterceptor(params System.Type[] requiredInterfaces)
        {
            _requiredInterfaces = requiredInterfaces.Concat(new[] { typeof(IFieldInterceptorAccessor) }).ToArray();
        }

        public IFieldInterceptor FieldInterceptor
        {
            get;
            set;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            if (FieldInterceptor != null)
            {
                if (ReflectHelper.IsPropertyGet((MethodInfo)input.MethodBase))
                {
                    var methodReturn = getNext().Invoke(input, getNext); // get the existing value

                    var result = FieldInterceptor.Intercept(
                        input.Target,
                        ReflectHelper.GetPropertyName((MethodInfo)input.MethodBase),
                        methodReturn.ReturnValue);

                    if (result != AbstractFieldInterceptor.InvokeImplementation)
                    {
                        methodReturn.ReturnValue = result;
                    }

                    return methodReturn;
                }

                if (ReflectHelper.IsPropertySet((MethodInfo)input.MethodBase))
                {
                    FieldInterceptor.MarkDirty();
                    FieldInterceptor.Intercept(input.Target, ReflectHelper.GetPropertyName((MethodInfo)input.MethodBase), null);
                }

                return getNext().Invoke(input, getNext);
            }

            if (input.MethodBase.DeclaringType == typeof(IFieldInterceptorAccessor) && ReflectHelper.IsPropertyGet((MethodInfo)input.MethodBase))
            {
                return input.CreateMethodReturn(FieldInterceptor);
            }

            if (input.MethodBase.DeclaringType == typeof(IFieldInterceptorAccessor) && ReflectHelper.IsPropertySet((MethodInfo)input.MethodBase))
            {
                FieldInterceptor = (IFieldInterceptor)input.Arguments[0];
                return input.CreateMethodReturn(null);
            }

            return getNext().Invoke(input, getNext);
        }

        public IEnumerable<System.Type> GetRequiredInterfaces()
        {
            return _requiredInterfaces;
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}
