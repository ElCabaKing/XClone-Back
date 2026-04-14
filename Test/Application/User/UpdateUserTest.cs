using Moq;
using Application.Modules.User.UpdateUser;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Exceptions;

namespace Test.Application.User;

public class UpdateUserTest
{
    [Fact]
    public async Task UpdateUser_Success()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var cloudStorageMock = new Mock<ICloudStorage>();

        var userId = Guid.NewGuid();
        var existingUser = new Domain.Entities.User
        {
            Id = userId,
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Old",
            LastName = "Name"
        };

        userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId))
            .ReturnsAsync(existingUser);

        userRepositoryMock.Setup(repo => repo.UsernameOrEmailExists(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        userRepositoryMock.Setup(repo => repo.UpdateUserAsync(It.IsAny<Domain.Entities.User>()))
            .ReturnsAsync(new Domain.Entities.User
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com",
                FirstName = "New",
                LastName = "Name"
            });

        var handler = new UpdateUserHandler(userRepositoryMock.Object, cloudStorageMock.Object);
        var command = new UpdateUserCommand(userId, "New", "Name");

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New", result.Data.FirstName);
        Assert.Equal("Name", result.Data.LastName);
        userRepositoryMock.Verify(repo => repo.UpdateUserAsync(It.IsAny<Domain.Entities.User>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_UserNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var cloudStorageMock = new Mock<ICloudStorage>();

        userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Domain.Entities.User)null);

        var handler = new UpdateUserHandler(userRepositoryMock.Object, cloudStorageMock.Object);
        var command = new UpdateUserCommand(Guid.NewGuid(), "New", "Name");

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command));
    }

    [Fact]
    public async Task UpdateUser_EmailAlreadyExists_ThrowsAlreadyExistsException()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var cloudStorageMock = new Mock<ICloudStorage>();

        var userId = Guid.NewGuid();
        var existingUser = new Domain.Entities.User
        {
            Id = userId,
            Username = "testuser",
            Email = "old@example.com",
            FirstName = "Test",
            LastName = "User"
        };

        userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId))
            .ReturnsAsync(existingUser);

        // Simular que el email nuevo ya existe para otro usuario
        userRepositoryMock.Setup(repo => repo.UsernameOrEmailExists(It.IsAny<string>(), "newemail@example.com"))
            .ReturnsAsync(true);

        var handler = new UpdateUserHandler(userRepositoryMock.Object, cloudStorageMock.Object);
        var command = new UpdateUserCommand(userId, "New", "Name", "newemail@example.com");

        // Act & Assert
        await Assert.ThrowsAsync<AlreadyExistsException>(async () => await handler.Handle(command));
    }

    [Fact]
    public async Task UpdateUser_InvalidEmail_ThrowsBadRequestException()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var cloudStorageMock = new Mock<ICloudStorage>();

        var userId = Guid.NewGuid();
        var existingUser = new Domain.Entities.User
        {
            Id = userId,
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User"
        };

        userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId))
            .ReturnsAsync(existingUser);

        var handler = new UpdateUserHandler(userRepositoryMock.Object, cloudStorageMock.Object);
        var command = new UpdateUserCommand(userId, "New", "Name", "invalid-email");

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(async () => await handler.Handle(command));
    }

    [Fact]
    public async Task UpdateUser_DatabaseUpdateFails_ThrowsServiceErrorException()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var cloudStorageMock = new Mock<ICloudStorage>();

        var userId = Guid.NewGuid();
        var existingUser = new Domain.Entities.User
        {
            Id = userId,
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User"
        };

        userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId))
            .ReturnsAsync(existingUser);

        userRepositoryMock.Setup(repo => repo.UsernameOrEmailExists(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        userRepositoryMock.Setup(repo => repo.UpdateUserAsync(It.IsAny<Domain.Entities.User>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        var handler = new UpdateUserHandler(userRepositoryMock.Object, cloudStorageMock.Object);
        var command = new UpdateUserCommand(userId, "New", "Name");

        // Act & Assert
        await Assert.ThrowsAsync<ServiceErrorException>(async () => await handler.Handle(command));
    }

    [Fact]
    public async Task UpdateUser_FirstNameTooLong_ThrowsBadRequestException()
    {
        // Arrange
        var handler = new UpdateUserHandler(
            new Mock<IUserRepository>().Object,
            new Mock<ICloudStorage>().Object
        );
        var longName = new string('a', 101); // Más de 100 caracteres
        var command = new UpdateUserCommand(Guid.NewGuid(), longName, "Last");

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(async () => await handler.Handle(command));
    }

    [Fact]
    public async Task UpdateUser_RepositoryCheckFails_ThrowsServiceErrorException()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var cloudStorageMock = new Mock<ICloudStorage>();

        userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new Exception("Database error"));

        var handler = new UpdateUserHandler(userRepositoryMock.Object, cloudStorageMock.Object);
        var command = new UpdateUserCommand(Guid.NewGuid(), "New", "Name");

        // Act & Assert
        await Assert.ThrowsAsync<ServiceErrorException>(async () => await handler.Handle(command));
    }

    [Fact]
    public async Task UpdateUser_EmailCheckFails_ThrowsServiceErrorException()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var cloudStorageMock = new Mock<ICloudStorage>();

        var userId = Guid.NewGuid();
        var existingUser = new Domain.Entities.User
        {
            Id = userId,
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User"
        };

        userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId))
            .ReturnsAsync(existingUser);

        userRepositoryMock.Setup(repo => repo.UsernameOrEmailExists(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Database error"));

        var handler = new UpdateUserHandler(userRepositoryMock.Object, cloudStorageMock.Object);
        var command = new UpdateUserCommand(userId, "New", "Name", "newemail@example.com");

        // Act & Assert
        await Assert.ThrowsAsync<ServiceErrorException>(async () => await handler.Handle(command));
    }
}
