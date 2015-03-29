// ---------------------------------------------------------------
// <autor>Pablo González</autor>
// <web>www.odra.com.ar</web>
// <blog>blog.odra.com.ar</blog>
// <email>pablo.gonzalez@odra.com.ar</email>
// <copyrigth>http://www.odra.com.ar/LGPL-3.0.txt</copyrigth>
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ObjectBuilder2;
using NHibernate.Cfg;

namespace NHibernate.Unity
{
    internal sealed class SessionFactoryPolicy : IBuilderPolicy
    {
        private readonly IDictionary<string, ISessionFactory> _sessionFactories = new Dictionary<string, ISessionFactory>();
        private readonly IDictionary<string, Configuration> _configurations = new Dictionary<string, Configuration>();

        internal ISessionFactory this[string connectionStringName]
        {
            get
            {
                return _sessionFactories[connectionStringName] ??
                       (_sessionFactories[connectionStringName] =
                        _configurations[connectionStringName].BuildSessionFactory());
            }
        }

        internal void RegisterSessionFactory(string connectionStringName, Configuration configuration, bool lazyBuildSessionFactory)
        {
            _configurations.Add(connectionStringName, configuration);
            var sessionFactory = !lazyBuildSessionFactory ? configuration.BuildSessionFactory() : default(ISessionFactory);
            _sessionFactories.Add(connectionStringName, sessionFactory);
        }

        internal ISessionFactory Default()
        {
            if (_sessionFactories.Values.SingleOrDefault() == default(ISessionFactory)
                && _configurations.Values.SingleOrDefault() != default(Configuration))
            {
                _sessionFactories[_configurations.Keys.Single()] = _configurations.Values.Single().BuildSessionFactory();
            }
            return _sessionFactories.Values.Single();
        }
    }
}
