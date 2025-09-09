using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using UserManagement.Data;
using UserManagement.Services.Domain;
using Xunit;

namespace UserManagement.Services.Tests
{
    public class UserServiceTests
    {
        [Fact]
        public async Task FilterByActiveAsync_True_ReturnsOnlyActive()
        {
            using var ctx = CreateCleanContext();
            var logger = new Mock<ILogger<UserService>>().Object;
            var sut = new UserService(ctx, logger);

            var result = await sut.FilterByActiveAsync(true, CancellationToken.None);

            result.Should().NotBeEmpty();
            result.Should().OnlyContain(u => u.IsActive);
        }

        [Fact]
        public async Task FilterByActiveAsync_False_ReturnsOnlyInactive()
        {
            using var ctx = CreateCleanContext();
            var logger = new Mock<ILogger<UserService>>().Object;
            var sut = new UserService(ctx, logger);

            var result = await sut.FilterByActiveAsync(false, CancellationToken.None);

            result.Should().NotBeEmpty();
            result.Should().OnlyContain(u => !u.IsActive);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsSeededUsers()
        {
            using var ctx = CreateCleanContext();
            var logger = new Mock<ILogger<UserService>>().Object;
            var sut = new UserService(ctx, logger);

            var result = await sut.GetAllAsync(CancellationToken.None);

            result.Should().NotBeNull();
            result.Count.Should().BeGreaterThan(0);
        }

        private static DataContext CreateCleanContext()
        {
            // ensure a clean in-memory DB per test run
            using (var tmp = new DataContext())
            {
                tmp.Database.EnsureDeleted();
            }
            return new DataContext();
        }
    }
}
