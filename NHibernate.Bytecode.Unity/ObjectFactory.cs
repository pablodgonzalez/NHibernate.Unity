using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Utility;

namespace NHibernate.Bytecode.Unity
{
    public class ObjectFactory : ActivatorObjectsFactory
    {
        private readonly IUnityContainer _container;

        public ObjectFactory(IUnityContainer container)
        {
            Guard.ArgumentNotNull(container, "container");
            _container = container;
        }

        #region Implementation of IObjectsFactory

        /// <summary>
        /// Creates an instance of the specified type.
        /// </summary>
        /// <param name="type">The type of object to create.</param>
        /// <returns>
        /// A reference to the created object.
        /// </returns>
        public new object CreateInstance(System.Type type)
        {
            return _container.IsRegistered(type) ? _container.Resolve(type) : base.CreateInstance(type);
        }

        /// <summary>
        /// Creates an instance of the specified type.
        /// </summary>
        /// <param name="type">The type of object to create.</param><param name="nonPublic">true if a public or nonpublic default constructor can match; false if only a public default constructor can match.</param>
        /// <returns>
        /// A reference to the created object.
        /// </returns>
        public new object CreateInstance(System.Type type, bool nonPublic)
        {
            return nonPublic ? base.CreateInstance(type, true) : CreateInstance(type);
        }

        /// <summary>
        /// Creates an instance of the specified type using the constructor 
        ///             that best matches the specified parameters.
        /// </summary>
        /// <param name="type">The type of object to create.</param><param name="ctorArgs">An array of constructor arguments.</param>
        /// <returns>
        /// A reference to the created object.
        /// </returns>
        public new object CreateInstance(System.Type type, params object[] ctorArgs)
        {
            return _container.Resolve(type, new TypedParameterOverride(ctorArgs));
        }

        #endregion
    }
}
