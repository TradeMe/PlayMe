using System;
using FluentNHibernate.Testing;
using NUnit.Framework;
using PlayMe.Data.NHibernate;
using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.UnitTest.PlayMe.Data.NHibernate
{
    [TestFixture]
    public class Mapping_Tests
    {
        [Test]
        public void CanCorrectlyMapUser()
        {
            var sessionFactory = SessionFactoryBuilder.CreateSessionFactory();
            var session = sessionFactory.OpenSession();
            new PersistenceSpecification<User>(session)
                //.CheckProperty(c => c.Id, new Guid("36bec9eb-5edf-485d-9de1-a34900b2d8ac"))
                .CheckProperty(c => c.Username, "TRADEME\\osymes")
                .CheckProperty(c => c.IsAdmin, true)
                .VerifyTheMappings();
        }
    }
}
