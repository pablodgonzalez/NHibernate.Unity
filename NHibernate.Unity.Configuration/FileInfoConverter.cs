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
    public sealed class FileInfoConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
        {
            return destinationType == typeof(FileInfo) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var stringValue = value as string;

            if (stringValue != null)
            {
                if (!Path.IsPathRooted(stringValue))
                {
                    var executingAssembly = new FileInfo(Assembly.GetExecutingAssembly().Location);
                    return new FileInfo(Path.Combine(executingAssembly.DirectoryName, stringValue));
                }

                return new FileInfo(stringValue);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
        {
            var fileinfo = value as FileInfo;

            return fileinfo == null ? string.Empty : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
