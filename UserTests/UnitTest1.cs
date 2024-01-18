using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using UserService.Controllers;
using UserService.DTO;
using UserService.Model;
using UserService.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UserTests
{
    [TestClass]
    public class UnitTest1
    {
        private UserController _controller;
        private Mock<IUserRepo> _userRepoMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IConfiguration> _configurationMock;
        private UserRepo _userRepo;

        [TestInitialize]
        public void Setup()
        {
            _userRepoMock = new Mock<IUserRepo>();
            _mapperMock = new Mock<IMapper>();
            _configurationMock = new Mock<IConfiguration>();

            _controller = new UserController(_configurationMock.Object, _userRepoMock.Object, _mapperMock.Object);
        }

        [TestMethod]
        public void GetUsers_ReturnsOkResult_WithFakeUsers()
        {
            // Arrange
            var fakeUsers = new List<Users> { new Users { Uid = Guid.NewGuid(), Name = "User1" }, new Users { Uid = Guid.NewGuid(), Name = "User2" } };
            _userRepoMock.Setup(repo => repo.GetAllUsers()).Returns(fakeUsers);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<UsersReadDTO>>(fakeUsers)).Returns(new List<UsersReadDTO>());

            // Act
            var result = _controller.GetUsers();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
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
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void GetUserByID_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var fakeUserId = Guid.NewGuid();
            _userRepoMock.Setup(repo => repo.GetUserByID(fakeUserId)).Returns((Users)null);

            // Act
            var result = _controller.GetUserByID(fakeUserId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
    }
}
