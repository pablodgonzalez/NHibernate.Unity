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
    [ConfigurationCollection(typeof(MappingConfigurationElement), AddItemName = "mapping")]
    public sealed class MappingConfigurationCollection : DeserializableConfigurationElementCollection<MappingConfigurationElement>
    {
        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as MappingConfigurationElement).GetKey();
        }
    }
}
