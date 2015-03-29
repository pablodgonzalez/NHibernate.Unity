// ---------------------------------------------------------------
// <autor>Pablo González</autor>
// <web>www.odra.com.ar</web>
// <blog>blog.odra.com.ar</blog>
// <email>pablo.gonzalez@odra.com.ar</email>
// <copyrigth>http://www.odra.com.ar/LGPL-3.0.txt</copyrigth>
// ---------------------------------------------------------------

using Microsoft.Practices.ObjectBuilder2;
// ---------------------------------------------------------------
// <autor>Pablo González</autor>
// <web>www.odra.com.ar</web>
// <blog>blog.odra.com.ar</blog>
// <email>pablo.gonzalez@odra.com.ar</email>
// <copyrigth>http://www.odra.com.ar/LGPL-3.0.txt</copyrigth>
// ---------------------------------------------------------------

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace NHibernate.Bytecode.Unity
{
    /// <summary>
    /// The objective of this class is to obtain container policies
    /// </summary>
    internal class NHibernateProxyUnityExtension : UnityContainerExtension
    {
        internal IPolicyList Policies
        {
            get
            {
                return Context.Policies;
            }
        }

        protected override void Initialize()
        {
            if (Container.Configure<Interception>() == null)
            {
                Container.AddNewExtension<Interception>();
            }
        }
    }
}
