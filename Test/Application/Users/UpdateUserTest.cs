using Moq;
using Application.Modules.Users.UpdateProfile;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Exceptions;

namespace Test.Application.Users;

public class UpdateUserTest
{
    [Fact]
    public async Task UpdateUser_Success()
    {
        // Arrange
        var uowMock = new Mock<IUOW>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var cloudStorageMock = new Mock<ICloudStorage>();

        uowMock.Setup(uow => uow.UserRepository).Returns(userRepositoryMock.Object);

        var userId = Guid.NewGuid();
        var existingUser = new Domain.Entities.User
        {
            Id = userId,
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Old",
            LastName = "Name"
        };

        userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        userRepositoryMock.Setup(repo => repo.UsernameOrEmailExists(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        userRepositoryMock.Setup(repo => repo.Update(It.IsAny<Domain.Entities.User>()))
            .ReturnsAsync(new Domain.Entities.User
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com",
                FirstName = "New",
                LastName = "Name"
            });

        var handler = new UpdateProfileHandler(uowMock.Object);
        var command = new UpdateProfileCommand(userId, "New", "Name", "test@example.com", null);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New", result.Data.FirstName);
        Assert.Equal("Name", result.Data.LastName);
        userRepositoryMock.Verify(repo => repo.Update(It.IsAny<Domain.Entities.User>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_UserNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var uowMock = new Mock<IUOW>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var cloudStorageMock = new Mock<ICloudStorage>();

        uowMock.Setup(uow => uow.UserRepository).Returns(userRepositoryMock.Object);

        userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Domain.Entities.User?)null);

        var handler = new UpdateProfileHandler(uowMock.Object);
        var command = new UpdateProfileCommand(Guid.NewGuid(), "New", "Name", "test@example.com", null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command));
    }

    [Fact]
    public async Task UpdateUser_RepositoryCheckFails_ThrowsServiceErrorException()
    {
        // Arrange
        var uowMock = new Mock<IUOW>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var cloudStorageMock = new Mock<ICloudStorage>();

        uowMock.Setup(uow => uow.UserRepository).Returns(userRepositoryMock.Object);

        userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new Exception("Database error"));

        var handler = new UpdateProfileHandler(uowMock.Object);
        var command = new UpdateProfileCommand(Guid.NewGuid(), "New", "Name", "test@example.com", null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await handler.Handle(command));
    }

}
