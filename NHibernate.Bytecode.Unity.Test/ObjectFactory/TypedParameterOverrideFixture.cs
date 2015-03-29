using Microsoft.Practices.Unity;
using NHibernate.Unity.Bytecode;
using NUnit.Framework;

namespace NHibernate.Bytecode.Unity.Tests.ObjectFactory
{
    internal class SomeClass
    {
        public SomeClass()
        {
        }

        public SomeClass(System.Type type, string value)
        {
            @Type = type;
            @String = value;
        }

        public SomeClass(System.Type typeValue, string stringValue, int intValue)
            : this(typeValue, stringValue)
        {
            @Int = intValue;
        }

        public System.Type @Type { get; private set; }
        public string @String { get; private set; }
        public int @Int { get; private set; }
    }

    [TestFixture]
    class TypedParameterOverrideFixture
    {
        [Test]
        public void TestTypedParameterOverride()
        {
            var container = new UnityContainer().RegisterType<SomeClass>();
            var someClass = container.Resolve<SomeClass>(new TypedParameterOverride(new object[] { typeof(TypedParameterOverride), "somevalue" }));

            Assert.That(someClass.Type, Is.EqualTo(typeof(TypedParameterOverride)));
            Assert.That(someClass.String, Is.EqualTo("somevalue"));
            Assert.That(someClass.Int, Is.EqualTo(default(int)));
        }
    }
}
