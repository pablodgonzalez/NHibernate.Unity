namespace NHibernate.Bytecode.Unity.Tests.ProxyInterface
{
    public interface IUnityProxy
    {
        int Id { get; set; }

        string Name { get; set; }

        void ThrowDeepException();
    }
}