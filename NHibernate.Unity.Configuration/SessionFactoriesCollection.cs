// ---------------------------------------------------------------
// <autor>Pablo González</autor>
// <web>www.odra.com.ar</web>
// <blog>blog.odra.com.ar</blog>
// <email>pablo.gonzalez@odra.com.ar</email>
// <copyrigth>http://www.odra.com.ar/LGPL-3.0.txt</copyrigth>
// ---------------------------------------------------------------

using System.Configuration;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;

namespace NHibernate.Unity.Configuration
{
    [ConfigurationCollection(typeof(SessionFactoryElement), AddItemName = "sessionFactory")]
    public sealed class SessionFactoriesCollection : DeserializableConfigurationElementCollection<SessionFactoryElement>
    {
        protected override object GetElementKey(ConfigurationElement element)
        {
            var nhElement = (SessionFactoryElement)element;
            return nhElement.ConnectionStringName + nhElement.File.FullName;
        }
    }
}
