using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PlayMe.Data;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server;
using PlayMe.Server.Interfaces;
using PlayMe.UnitTest.Plumbing;

namespace PlayMe.UnitTest.PlayMe.Server
{
    [TestFixture]
    public class UserService_Tests:TestBase<UserService>
    {
        [Test]
        public void if_user_is_admin_in_config_IsAdminUser_returns_true()
        {
            //arrange
            string adminUser = "AUser";
            var userSettingsMock = GetMock<IUserSettings>();
            userSettingsMock.Setup(m => m.AdminUsers).Returns(new List<string> {"OneUser", "AUser", "ThreeUser"});

            //act
            bool result = ClassUnderTest.IsUserAdmin(adminUser);

            //assert
            Assert.IsTrue(result);

        }

        [Test]
        public void if_user_is_admin_in_db_IsAdminUser_returns_true()
        {
            //arrange
            string adminUser = "AUser";
            var adminDataServiceMock = GetMock<IRepository<User>>();
            adminDataServiceMock.Setup(m => m.GetAll()).Returns(new List<User> { new User { Username = "OneUser" }, new User { Username = "AUser" }, new User { Username = "ThreeUser" } }.AsQueryable());

            //act
            bool result = ClassUnderTest.IsUserAdmin(adminUser);

            //assert
            Assert.IsTrue(result);

        }

        [Test]
        public void if_user_is_not_admin_in_config_or_db_IsAdminUser_returns_false()
        {
            //arrange
            string adminUser = "AUser";
            var userSettingsMock = GetMock<IUserSettings>();
            userSettingsMock.Setup(m => m.AdminUsers).Returns(new List<string> { "OneUser" });

            var adminDataServiceMock = GetMock<IRepository<User>>();
            adminDataServiceMock.Setup(m => m.GetAll()).Returns(new List<User> { new User { Username = "OneUser" }}.AsQueryable());

            //act
            bool result = ClassUnderTest.IsUserAdmin(adminUser);

            //assert
            Assert.IsFalse(result);

        }
    }
}
