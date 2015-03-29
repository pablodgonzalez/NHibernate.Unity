// ---------------------------------------------------------------
// <autor>Pablo González</autor>
// <web>www.odra.com.ar</web>
// <blog>blog.odra.com.ar</blog>
// <email>pablo.gonzalez@odra.com.ar</email>
// <copyrigth>http://www.odra.com.ar/LGPL-3.0.txt</copyrigth>
// ---------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Reflection;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;

namespace NHibernate.Unity.Configuration
{
    public sealed class MappingConfigurationElement : DeserializableConfigurationElement
    {
        [ConfigurationProperty("assembly")]
        [TypeConverter(typeof(AssemblyConverter))]
        public Assembly Assembly
        {
            get
            {
                return this["assembly"] as Assembly;
            }
        }

        [ConfigurationProperty("directory")]
        [TypeConverter(typeof(DirectoryInfoConverter))]
        public DirectoryInfo Directory
        {
            get
            {
                return this["directory"] as DirectoryInfo;
            }
        }

        [ConfigurationProperty("file")]
        [TypeConverter(typeof(FileInfoConverter))]
        public FileInfo File
        {
            get
            {
                return this["file"] as FileInfo;
            }
        }

        [ConfigurationProperty("uri")]
        [TypeConverter(typeof(UriTypeConverter))]
        public Uri Uri
        {
            get
            {
                return this["uri"] as Uri;
            }
        }

        [ConfigurationProperty("class")]
        [TypeConverter(typeof(TypeNameConverter))]
        public System.Type Type
        {
            get
            {
                return this["class"] as System.Type;
            }
        }

        [ConfigurationProperty("resource")]
        public string Resource
        {
            get
            {
                return this["resource"].ToString();
            }
        }

        internal void AddMappings(Cfg.Configuration factoryConfiguration)
        {
            if (File != null && File.Exists)
            {
                factoryConfiguration.AddFile(File);
            }

            if (Uri != null)
            {
                factoryConfiguration.AddUrl(Uri);
            }

            if (Assembly != null && string.IsNullOrEmpty(Resource))
            {
                factoryConfiguration.AddAssembly(Assembly);
            }

            if (Directory != null && Directory.Exists)
            {
                factoryConfiguration.AddDirectory(Directory);
            }

            if (Type != null)
            {
                factoryConfiguration.AddClass(Type);
            }

            if (Assembly != null && !string.IsNullOrEmpty(Resource))
            {
                factoryConfiguration.AddResource(string.Format("{0}.{1}", Assembly.GetName().Name, Resource), Assembly);
            }
        }

        internal object GetKey()
        {
            return Guid.NewGuid();
        }
    }
}
