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

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public void ThrowDeepException()
        {
            Level1();
        }

        #endregion
    }
}