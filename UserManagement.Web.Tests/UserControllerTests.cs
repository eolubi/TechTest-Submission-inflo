using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using UserManagement.Models;
using UserManagement.Services.Domain;
using UserManagement.Web.Models.Users;
using UserManagement.WebMS.Controllers;
using Xunit;

namespace UserManagement.Web.Tests
{
    public class UsersControllerTests
    {
        [Fact]
        public async Task List_NoStatus_ReturnsView_WithAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new() { Id = 1, Forename = "A", Surname = "One", Email = "a1@example.com", IsActive = true },
                new() { Id = 2, Forename = "B", Surname = "Two", Email = "b2@example.com", IsActive = false }
            };

            var svc = new Mock<IUserService>();
            svc.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(users);

            var logger = new Mock<ILogger<UsersController>>();
            var controller = new UsersController(svc.Object, logger.Object);

            // Act
            var result = await controller.List(null, CancellationToken.None);

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserListViewModel>(view.Model);
            model.Items.Select(i => i.Email).Should().BeEquivalentTo(users.Select(u => u.Email));

            svc.Verify(s => s.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
            svc.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task List_StatusActive_UsesActiveFilter_AndMapsModel()
        {
            // Arrange
            var active = new List<User>
            {
                new() { Id = 10, Forename = "Active", Surname = "User", Email = "active@example.com", IsActive = true }
            };

            var svc = new Mock<IUserService>(MockBehavior.Strict);
            svc.Setup(s => s.FilterByActiveAsync(true, It.IsAny<CancellationToken>()))
               .ReturnsAsync(active);

            var logger = new Mock<ILogger<UsersController>>();
            var controller = new UsersController(svc.Object, logger.Object);

            // Act
            var result = await controller.List("active", CancellationToken.None);

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserListViewModel>(view.Model);
            model.Items.Should().HaveCount(1);
            model.Items[0].Email.Should().Be("active@example.com");
            model.Items[0].IsActive.Should().BeTrue();

            svc.Verify(s => s.FilterByActiveAsync(true, It.IsAny<CancellationToken>()), Times.Once);
            svc.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task List_StatusInactive_UsesInactiveFilter_AndMapsModel()
        {
            // Arrange
            var inactive = new List<User>
            {
                new() { Id = 20, Forename = "Inactive", Surname = "User", Email = "inactive@example.com", IsActive = false }
            };

            var svc = new Mock<IUserService>(MockBehavior.Strict);
            svc.Setup(s => s.FilterByActiveAsync(false, It.IsAny<CancellationToken>()))
               .ReturnsAsync(inactive);

            var logger = new Mock<ILogger<UsersController>>();
            var controller = new UsersController(svc.Object, logger.Object);

            // Act
            var result = await controller.List("inactive", CancellationToken.None);

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserListViewModel>(view.Model);
            model.Items.Should().HaveCount(1);
            model.Items[0].Email.Should().Be("inactive@example.com");
            model.Items[0].IsActive.Should().BeFalse();

            svc.Verify(s => s.FilterByActiveAsync(false, It.IsAny<CancellationToken>()), Times.Once);
            svc.VerifyNoOtherCalls();
        }
    }
}
