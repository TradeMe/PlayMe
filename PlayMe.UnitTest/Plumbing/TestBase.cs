using System;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Kernel;

// ReSharper disable LocalizableElement
namespace PlayMe.UnitTest.Plumbing
{
    public abstract class TestBase<T> where T : class
    {
        private IFixture fixture; 

        [SetUp]
        public void Initialize()
        {
            fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        public Mock<TDependency> GetMock<TDependency>() where TDependency : class
        {
            var mock = new Mock<TDependency>();
            fixture.Register(() => mock.Object);
            return mock;
        }

        public T ClassUnderTest
        {
            get
            {
                fixture.Behaviors.RemoveAt(0);
                fixture.Behaviors.Add(new OmitOnRecursionBehavior());
                fixture.Customize<T>(c => c.FromFactory(new MethodInvoker(new GreedyConstructorQuery())));

                var result = default(T);

                try
                {
                    result = fixture.Create<T>();
                }
                catch(ObjectCreationException ex)
                {
                    Console.WriteLine(ex);
                }

                return result;
            }
        }

        public T ModestClassUnderTest
        {
            get
            {
                return fixture.Create<T>();
            }

        }

        public void Register<TDependency>(TDependency dependency)
        {
            fixture.Register(() => dependency);
        }

    }
}
