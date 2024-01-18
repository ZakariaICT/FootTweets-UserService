using AutoMapper;
using Castle.Core.Configuration;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Controllers;
using UserService.DTO;
using UserService.Repositories;

namespace UserserviceTests
{
    [TestFixture]
    public class UserControllerTests
    {
        private UserController _controller;
        private Mock<IUserRepo> _userRepoMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IConfiguration> _configurationMock;

        [SetUp]
        public void Setup()
        {
            _userRepoMock = new Mock<IUserRepo>();
            _mapperMock = new Mock<IMapper>();
            _configurationMock = new Mock<IConfiguration>();

            _controller = new UserController(_configurationMock.Object, _userRepoMock.Object, _mapperMock.Object);
        }

        [Test]
        public void GetUsers_ReturnsOkResult_WithFakeUsers()
        {
            // Arrange
            var fakeUsers = new List<Users> { new Users { Uid = Guid.NewGuid(), Name = "User1" }, new Users { Uid = Guid.NewGuid(), Name = "User2" } };
            if (fakeUsers != null)
            {
                _userRepoMock.Setup(repo => repo.GetAllUsers()).Returns(fakeUsers);
                _mapperMock.Setup(mapper => mapper.Map<IEnumerable<UsersReadDTO>>(fakeUsers)).Returns(new List<UsersReadDTO>());
            }
            else
            {
                // Handle the case where fakeUsers is null, e.g., log an error or provide a default behavior.
            }


            // Act
            var result = _controller.GetUsers();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
        }


        [Test]
        public void GetUserByID_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var fakeUserId = Guid.NewGuid();
            var fakeUser = new Users { Uid = fakeUserId, Name = "User1" };
            _userRepoMock.Setup(repo => repo.GetUserByID(fakeUserId)).Returns(fakeUser);
            _mapperMock.Setup(mapper => mapper.Map<UsersReadDTO>(fakeUser)).Returns(new UsersReadDTO());

            // Act
            var result = _controller.GetUserByID(fakeUserId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
        }

        [Test]
        public void GetUserByID_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var fakeUserId = Guid.NewGuid();
            _userRepoMock.Setup(repo => repo.GetUserByID(fakeUserId)).Returns((Users)null);

            // Act
            var result = _controller.GetUserByID(fakeUserId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        // Similar tests can be created for other controller methods...
    }
}
