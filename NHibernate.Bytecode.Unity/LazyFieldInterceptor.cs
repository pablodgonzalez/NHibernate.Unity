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
        private readonly object _target;
        private readonly System.Type[] _requiredInterfaces;

        public LazyFieldInterceptor(object target, params System.Type[] requiredInterfaces)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            _target = target;
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
                    if (input.MethodBase.DeclaringType == typeof(IFieldInterceptorAccessor) && "get_FieldInterceptor".Equals(input.MethodBase.Name))
                    {
                        return input.CreateMethodReturn(FieldInterceptor);
                    }

                    var propValue = input.MethodBase.Invoke(_target, input.Arguments.Cast<object>().ToArray());

                    var result = FieldInterceptor.Intercept(input.Target, ReflectHelper.GetPropertyName((MethodInfo)input.MethodBase), propValue);

                    if (result != AbstractFieldInterceptor.InvokeImplementation)
                    {
                        return input.CreateMethodReturn(result);
                    }
                }
                else if (ReflectHelper.IsPropertySet((MethodInfo)input.MethodBase))
                {
                    if (input.MethodBase.DeclaringType == typeof(IFieldInterceptorAccessor) && "set_FieldInterceptor".Equals(input.MethodBase.Name))
                    {
                        FieldInterceptor = (IFieldInterceptor)input.Arguments[0];
                        return null;
                    }
                    FieldInterceptor.MarkDirty();
                    FieldInterceptor.Intercept(input.Target, ReflectHelper.GetPropertyName((MethodInfo)input.MethodBase), input.Arguments[0]);
                }
            }
            else if (input.MethodBase.DeclaringType == typeof(IFieldInterceptorAccessor) && ReflectHelper.IsPropertySet((MethodInfo)input.MethodBase) && "set_FieldInterceptor".Equals(input.MethodBase.Name))
            {
                FieldInterceptor = (IFieldInterceptor)input.Arguments[0];
                return input.CreateMethodReturn(null);
            }

            return input.CreateMethodReturn(input.MethodBase.Invoke(_target, input.Arguments.Cast<object>().ToArray()));
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
