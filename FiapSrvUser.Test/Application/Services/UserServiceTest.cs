using FiapSrvUser.Application.DTOs;
using FiapSrvUser.Application.Exceptions;
using FiapSrvUser.Application.Interfaces.Repositories;
using FiapSrvUser.Application.Services;
using FiapSrvUser.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class UserServiceTest
{
    private readonly Mock<IUserRepository> _repoMock;
    private readonly Mock<ILogger<UserService>> _loggerMock;
    private readonly UserService _service;

    public UserServiceTest()
    {
        _repoMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<UserService>>();
        _service = new UserService(_repoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsUserDto_WhenUserExists()
    {
        var userId = Guid.NewGuid();
        var player = new Player { Id = userId, Name = "Test", Username = "test", Email = "test@test.com" };
        _repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(player);

        var result = await _service.GetByIdAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenUserNotFound()
    {
        var userId = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(userId));
    }

    [Fact]
    public async Task UpdateAsync_UpdatesUser_WhenUserExists()
    {
        var userId = Guid.NewGuid();
        var user = new Player { Id = userId, Name = "Old", Username = "old" };
        var dto = new UpdateUserDto { Name = "New", UserName = "new" };
        _repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        await _service.UpdateAsync(userId, dto);

        _repoMock.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Name == "New" && u.Username == "new")), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsNotFoundException_WhenUserNotFound()
    {
        var userId = Guid.NewGuid();
        var dto = new UpdateUserDto { Name = "New", UserName = "new" };
        _repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(userId, dto));
    }

    [Fact]
    public async Task DeleteAsync_DeletesUser_WhenUserExists()
    {
        var userId = Guid.NewGuid();
        var user = new Player { Id = userId };
        _repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        await _service.DeleteAsync(userId);

        _repoMock.Verify(r => r.DeleteAsync(userId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsNotFoundException_WhenUserNotFound()
    {
        var userId = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(userId));
    }

    [Fact]
    public async Task GetPlayersAsync_ReturnsPlayerDtos()
    {
        var players = new List<Player> { new Player { Id = Guid.NewGuid(), Name = "Player1" } };
        _repoMock.Setup(r => r.GetPlayersAsync()).ReturnsAsync(players);

        var result = await _service.GetPlayersAsync();

        Assert.Single(result);
    }

    [Fact]
    public async Task GetPlublishersAsync_ReturnsPublisherDtos()
    {
        var publishers = new List<Publisher> { new Publisher { Id = Guid.NewGuid(), Name = "Publisher1" } };
        _repoMock.Setup(r => r.GetPublishersAsync()).ReturnsAsync(publishers);

        var result = await _service.GetPlublishersAsync();

        Assert.Single(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsUserDtos()
    {
        var users = new List<User>
        {
            new Player { Id = Guid.NewGuid(), Name = "Player1" },
            new Publisher { Id = Guid.NewGuid(), Name = "Publisher1" }
        };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        var result = await _service.GetAllAsync();

        Assert.Equal(2, ((List<UserDto>)result).Count);
    }
}
