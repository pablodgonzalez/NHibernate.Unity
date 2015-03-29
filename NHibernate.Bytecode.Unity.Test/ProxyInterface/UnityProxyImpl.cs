using System;
using System.Runtime.CompilerServices;

namespace NHibernate.Bytecode.Unity.Tests.ProxyInterface
{
    public class UnityProxyImpl : IUnityProxy
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void Level1()
        {
            Level2();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void Level2()
        {
            throw new ArgumentException("thrown from Level2");
        }

        #region IUnityProxy Members

        public int Id { get; set; }

        public string Name { get; set; }

        public void ThrowDeepException()
        {
            Level1();
        }

        #endregion
    }
}