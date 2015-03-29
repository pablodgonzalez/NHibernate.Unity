// ---------------------------------------------------------------
// <autor>Pablo González</autor>
// <web>www.odra.com.ar</web>
// <blog>blog.odra.com.ar</blog>
// <email>pablo.gonzalez@odra.com.ar</email>
// <copyrigth>http://www.odra.com.ar/LGPL-3.0.txt</copyrigth>
// ---------------------------------------------------------------

using Microsoft.Practices.Unity.Configuration;

namespace NHibernate.Unity.Configuration
{
    public class NHibernateUnityConfigurationExtension : SectionExtension
    {
        public override void AddExtensions(SectionExtensionContext context)
        {
            context.AddAlias<NHibernateUnityExtension>("NHibernate");
            context.AddElement<NHibernateUnityExtensionElement>("NHibernate");
        }
    }
}
