// ---------------------------------------------------------------
// <autor>Pablo González</autor>
// <web>www.odra.com.ar</web>
// <blog>blog.odra.com.ar</blog>
// <email>pablo.gonzalez@odra.com.ar</email>
// <copyrigth>http://www.odra.com.ar/LGPL-3.0.txt</copyrigth>
// ---------------------------------------------------------------

using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace NHibernate.Unity.Configuration
{
    public sealed class DirectoryInfoConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
        {
            return destinationType == typeof(DirectoryInfo) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var stringValue = value as string;

            if (stringValue != null)
            {
                if (!Path.IsPathRooted(stringValue))
                {
                    var testAssembly = new DirectoryInfo(Assembly.GetExecutingAssembly().Location);
                    return new DirectoryInfo(Path.Combine(testAssembly.FullName, stringValue));
                }

                return new DirectoryInfo(stringValue);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
        {
            var directoryinfo = value as DirectoryInfo;

            if (directoryinfo == null)
            {
                return string.Empty;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
