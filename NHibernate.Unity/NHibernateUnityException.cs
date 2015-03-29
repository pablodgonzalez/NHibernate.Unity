// ---------------------------------------------------------------
// <autor>Pablo González</autor>
// <web>www.odra.com.ar</web>
// <blog>blog.odra.com.ar</blog>
// <email>pablo.gonzalez@odra.com.ar</email>
// <copyrigth>http://www.odra.com.ar/LGPL-3.0.txt</copyrigth>
// ---------------------------------------------------------------

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace NHibernate.Unity
{
    [Serializable]
    public class NHibernateUnityException : Exception
    {
        public NHibernateUnityException()
        {
        }

        public NHibernateUnityException(string message)
            : base(message)
        {
        }

        public NHibernateUnityException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public NHibernateUnityException(string format, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, format, args))
        {
        }

        public NHibernateUnityException(string format, Exception innerException, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, format, args), innerException)
        {
        }

        protected NHibernateUnityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
