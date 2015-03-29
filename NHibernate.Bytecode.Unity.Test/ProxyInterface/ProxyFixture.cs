using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Practices.Unity;
using NHibernate.Engine;
using NUnit.Framework;

namespace NHibernate.Bytecode.Unity.Tests.ProxyInterface
{
    [TestFixture]
    public class ProxyFixture : TestCase
    {
        protected override Cfg.Configuration Cfg
        {
            get { return Container.Resolve<Cfg.Configuration>("TestSource"); }
        }

        protected override ISessionFactoryImplementor Sessions
        {
            get { return (ISessionFactoryImplementor)Container.Resolve<ISessionFactory>("TestSource"); }
        }

        private void SerializeAndDeserialize(ref ISession s)
        {
            // Serialize the session
            using (Stream stream = new MemoryStream())
            {

                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, s);

                // Close the original session
                s.Close();

                // Deserialize the session
                stream.Position = 0;
                s = (ISession)formatter.Deserialize(stream);
            }
        }

        [Test]
        public void ExceptionStackTrace()
        {
            ISession s = OpenSession();
            IUnityProxy ap = new UnityProxyImpl { Id = 1, Name = "first proxy" };
            s.Save(ap);
            s.Flush();
            s.Close();

            s = OpenSession();
            ap = (IUnityProxy)s.Load(typeof(UnityProxyImpl), ap.Id);
            Assert.IsFalse(NHibernateUtil.IsInitialized(ap), "check we have a proxy");

            try
            {
                ap.ThrowDeepException();
                Assert.Fail("Exception not thrown");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("thrown from Level2", ae.Message);

                string[] stackTraceLines = ae.StackTrace.Split('\n');
                Assert.IsTrue(stackTraceLines[0].Contains("Level2"), "top of exception stack is Level2()");
                Assert.IsTrue(stackTraceLines[1].Contains("Level1"), "next on exception stack is Level1()");
            }
            finally
            {
                s.Delete(ap);
                s.Flush();
                s.Close();
            }
        }

        [Test]
        public void Proxy()
        {
            ISession s = OpenSession();
            IUnityProxy ap = new UnityProxyImpl { Id = 1, Name = "first proxy" };
            s.Save(ap);
            s.Flush();
            s.Close();

            s = OpenSession();
            ap = (IUnityProxy)s.Load(typeof(UnityProxyImpl), ap.Id);
            Assert.IsFalse(NHibernateUtil.IsInitialized(ap));
            var id = ap.Id;
            Assert.IsFalse(NHibernateUtil.IsInitialized(ap), "get id should not have initialized it.");
            var name = ap.Name;
            Assert.IsTrue(NHibernateUtil.IsInitialized(ap), "get name should have initialized it.");
            s.Delete(ap);
            s.Flush();
            s.Close();
        }

        [Test]
        public void ProxySerialize()
        {
            ISession s = OpenSession();
            IUnityProxy ap = new UnityProxyImpl { Id = 1, Name = "first proxy" };
            s.Save(ap);
            s.Flush();
            s.Close();

            s = OpenSession();
            ap = (IUnityProxy)s.Load(typeof(UnityProxyImpl), ap.Id);
            Assert.AreEqual(1, ap.Id);

            //Assembly.Load(ap.GetType().Assembly.);

            s.Disconnect();

            SerializeAndDeserialize(ref s);

            s.Reconnect();
            s.Disconnect();

            // serialize and then deserialize the session again - make sure Unity.Interception
            // has no problem with serializing two times - earlier versions of it did.
            SerializeAndDeserialize(ref s);

            s.Close();

            s = OpenSession();
            s.Delete(ap);
            s.Flush();
            s.Close();
        }

        [Test]
        public void SerializeNotFoundProxy()
        {
            ISession s = OpenSession();
            // this does not actually exists in db
            var notThere = (IUnityProxy)s.Load(typeof(UnityProxyImpl), 5);
            Assert.AreEqual(5, notThere.Id);
            s.Disconnect();

            // serialize and then deserialize the session.
            SerializeAndDeserialize(ref s);

            Assert.IsNotNull(s.Load(typeof(UnityProxyImpl), 5), "should be proxy - even though it doesn't exists in db");
            s.Close();
        }
    }
}