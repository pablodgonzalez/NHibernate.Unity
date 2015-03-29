// ---------------------------------------------------------------
// <autor>Pablo González</autor>
// <web>www.odra.com.ar</web>
// <blog>blog.odra.com.ar</blog>
// <email>pablo.gonzalez@odra.com.ar</email>
// <copyrigth>http://www.odra.com.ar/LGPL-3.0.txt</copyrigth>
// ---------------------------------------------------------------

using System.ComponentModel;
using System.Configuration;
using System.IO;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;

namespace NHibernate.Unity.Configuration
{
    public sealed class SessionFactoryElement : DeserializableConfigurationElement
    {
        [ConfigurationProperty("connectionStringName", IsRequired = true, IsKey = true)]
        public string ConnectionStringName
        {
            get
            {
                return this["connectionStringName"] as string;
            }
        }

        [ConfigurationProperty("file", IsRequired = true, IsKey = true)]
        [TypeConverter(typeof(FileInfoConverter))]
        public FileInfo File
        {
            get
            {
                return this["file"] as FileInfo;
            }
        }

        [ConfigurationProperty("mappings", IsRequired = true)]
        public MappingConfigurationCollection Mappings
        {
            get
            {
                return this["mappings"] as MappingConfigurationCollection;
            }
        }
    }
}
