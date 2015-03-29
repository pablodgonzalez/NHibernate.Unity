// ---------------------------------------------------------------
// <autor>Pablo González</autor>
// <web>www.odra.com.ar</web>
// <blog>blog.odra.com.ar</blog>
// <email>pablo.gonzalez@odra.com.ar</email>
// <copyrigth>http://www.odra.com.ar/LGPL-3.0.txt</copyrigth>
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.Unity.InterceptionExtension;
using NHibernate.Engine;
using NHibernate.Proxy;
using NHibernate.Proxy.Poco;
using NHibernate.Type;

namespace NHibernate.Bytecode.Unity
{
    /// <summary>
    /// A <see cref="ILazyInitializer"/> for use with the Unity Dynamic Class Generator.
    /// </summary>
    [Serializable]
    public class LazyInitializer : BasicLazyInitializer, IInterceptionBehavior
    {

        private static readonly MethodInfo Exception_InternalPreserveStackTrace =
    typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);

        private readonly System.Type[] _requiredInterfaces;

        #region Instance

        /// <summary>
        /// Initializes a new <see cref="LazyInitializer"/> object.
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="persistentClass">The Class to Proxy.</param>
        /// <param name="id">The Id of the Object we are Proxying.</param>
        /// <param name="getIdentifierMethod"></param>
        /// <param name="setIdentifierMethod"></param>
        /// <param name="componentIdType"></param>
        /// <param name="session">The ISession this Proxy is in.</param>
        /// <param name="requiredInterfaces">additional interfaces to intercepting.</param>
        public LazyInitializer(string entityName, System.Type persistentClass, object id,
                                     MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod,
                                     IAbstractComponentType componentIdType, ISessionImplementor session,
            System.Type[] requiredInterfaces)
            : base(entityName, persistentClass, id, getIdentifierMethod, setIdentifierMethod, componentIdType, session)
        {
            _requiredInterfaces = requiredInterfaces;
        }

        #endregion

        #region IInterceptionBehavior Members


        public IEnumerable<System.Type> GetRequiredInterfaces()
        {
            return _requiredInterfaces;
        }

        public virtual IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var method = input.MethodBase.ReflectedType.GetMethod(input.MethodBase.Name);
            var arguments = new object[input.Arguments.Count];

            for (var i = 0; i < input.Arguments.Count; i++)
            {
                arguments[i] = input.Arguments[i];
            }

            try
            {
                var returnValue = base.Invoke(method, arguments, input.Target);

                // Avoid invoking the actual implementation, if possible
                return input.CreateMethodReturn(returnValue != InvokeImplementation ? returnValue : method.Invoke(GetImplementation(), arguments), null);
            }
            catch (TargetInvocationException tie)
            {
                // Propagate the inner exception so that the proxy throws the same exception as
                // the real object would 
                Exception_InternalPreserveStackTrace.Invoke(tie.InnerException, new Object[] { });
                throw tie.InnerException;
            }
        }

        public bool WillExecute
        {
            get { return true; }
        }

        #endregion
    }
}