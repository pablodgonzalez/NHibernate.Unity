// ---------------------------------------------------------------
// <autor>Pablo González</autor>
// <web>www.odra.com.ar</web>
// <blog>blog.odra.com.ar</blog>
// <email>pablo.gonzalez@odra.com.ar</email>
// <copyrigth>http://www.odra.com.ar/LGPL-3.0.txt</copyrigth>
// ---------------------------------------------------------------

using Microsoft.Practices.ObjectBuilder2;

namespace NHibernate.Unity
{
    internal class SessionFactoryStrategy : IBuilderStrategy
    {
        public void PostBuildUp(IBuilderContext context)
        {
        }

        public void PostTearDown(IBuilderContext context)
        {
        }

        public void PreBuildUp(IBuilderContext context)
        {
            if (typeof(ISessionFactory).IsAssignableFrom(context.BuildKey.Type))
            {
                var policy = context.Policies.Get<SessionFactoryPolicy>(context.BuildKey);
                if (policy == null)
                {
                    throw new NHibernateUnityException("The container is not the current container for NHibernate.");
                }

                context.Existing = string.IsNullOrEmpty(context.BuildKey.Name)
                                       ? policy.Default()
                                       : policy[context.BuildKey.Name];
            }
        }

        public void PreTearDown(IBuilderContext context)
        {
        }
    }
}
