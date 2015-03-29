// ---------------------------------------------------------------
// <autor>Pablo González</autor>
// <web>www.odra.com.ar</web>
// <blog>blog.odra.com.ar</blog>
// <email>pablo.gonzalez@odra.com.ar</email>
// <copyrigth>http://www.odra.com.ar/LGPL-3.0.txt</copyrigth>
// ---------------------------------------------------------------

using System.Configuration;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace NHibernate.Unity.Configuration
{
    public class NHibernateUnityExtensionElement : ContainerConfiguringElement
    {
        protected override void ConfigureContainer(IUnityContainer container)
        {
            if (container.Configure<NHibernateUnityExtension>() == null)
            {
                container.AddNewExtension<NHibernateUnityExtension>();
            }

            foreach (var sessionFactory in SessionFactories)
            {
                var factoryConfiguration = new Cfg.Configuration().Configure(sessionFactory.File.FullName);
                factoryConfiguration.SetProperty("connection.connection_string", ConfigurationManager.ConnectionStrings[sessionFactory.ConnectionStringName].ConnectionString);

                foreach (var mapping in sessionFactory.Mappings)
                {
                    mapping.AddMappings(factoryConfiguration);
                }

                container.RegisterInstance(sessionFactory.ConnectionStringName, factoryConfiguration);
            }
        }

        [ConfigurationProperty("sessionFactories", IsRequired = false)]
        public SessionFactoriesCollection SessionFactories
        {
            get
            {
                return this["sessionFactories"] as SessionFactoriesCollection;
            }
        }
    }
}
