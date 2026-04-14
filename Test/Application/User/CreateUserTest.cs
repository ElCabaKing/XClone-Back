using Moq;
using Application.Modules.User.CreateUser;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Exceptions;

namespace Test.Application.User;

public class CreateUserTest
{
    [Fact]
    public async Task CreateUser_Success()
    {
        // Arrange
        var passwordServiceMock = new Mock<IPasswordService>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var cloudStorageMock = new Mock<ICloudStorage>();

        userRepositoryMock.Setup(repo => repo.UsernameOrEmailExists(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        passwordServiceMock.Setup(service => service.HashPassword(It.IsAny<string>()))
            .Returns("hashedpassword");
        cloudStorageMock.Setup(storage => storage.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync("http://example.com/profile.jpg");

        userRepositoryMock.Setup(repo => repo.CreateUserAsync(It.IsAny<Domain.Entities.User>()))
            .ReturnsAsync((Domain.Entities.User user) => new Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePictureUrl = user.ProfilePictureUrl
            });

        var handler = new CreateUserHandler(passwordServiceMock.Object, userRepositoryMock.Object, cloudStorageMock.Object);
        var command = new CreateUserCommand("testuser", "epicouser@example.com", "Password123!", "Test", "User");

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Username, result.Data.Username);
        Assert.Equal(command.Email, result.Data.Email);
        Assert.Equal(command.FirstName, result.Data.FirstName);
        Assert.Equal(command.LastName, result.Data.LastName);
        userRepositoryMock.Verify(repo => repo.CreateUserAsync(It.IsAny<Domain.Entities.User>()), Times.Once);
    }

    [Fact]
    public async Task CreateUser_UsernameAlreadyExists_ThrowsAlreadyExistsException()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var cloudStorageMock = new Mock<ICloudStorage>();

        userRepositoryMock.Setup(repo => repo.UsernameOrEmailExists(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        var handler = new CreateUserHandler(passwordServiceMock.Object, userRepositoryMock.Object, cloudStorageMock.Object);
        var command = new CreateUserCommand("existinguser", "new@example.com", "Password123!", "Test", "User");

        // Act & Assert
        await Assert.ThrowsAsync<AlreadyExistsException>(async () => await handler.Handle(command));
    }

    [Fact]
    public async Task CreateUser_CloudStorageUploadFails_ThrowsServiceErrorException()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var cloudStorageMock = new Mock<ICloudStorage>();

        userRepositoryMock.Setup(repo => repo.UsernameOrEmailExists(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        passwordServiceMock.Setup(service => service.HashPassword(It.IsAny<string>()))
            .Returns("hashedpassword");

        cloudStorageMock.Setup(storage => storage.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Cloudinary error"));

        var handler = new CreateUserHandler(passwordServiceMock.Object, userRepositoryMock.Object, cloudStorageMock.Object);
        var command = new CreateUserCommand("testuser", "test@example.com", "Password123!", "Test", "User");

        // Act & Assert
        await Assert.ThrowsAsync<ServiceErrorException>(async () => await handler.Handle(command));
    }

    [Fact]
    public async Task CreateUser_DatabaseInsertFails_ThrowsServiceErrorException()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var cloudStorageMock = new Mock<ICloudStorage>();

        userRepositoryMock.Setup(repo => repo.UsernameOrEmailExists(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        passwordServiceMock.Setup(service => service.HashPassword(It.IsAny<string>()))
            .Returns("hashedpassword");

        cloudStorageMock.Setup(storage => storage.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync("http://example.com/profile.jpg");

        userRepositoryMock.Setup(repo => repo.CreateUserAsync(It.IsAny<Domain.Entities.User>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        var handler = new CreateUserHandler(passwordServiceMock.Object, userRepositoryMock.Object, cloudStorageMock.Object);
        var command = new CreateUserCommand("testuser", "test@example.com", "Password123!", "Test", "User");

        // Act & Assert
        await Assert.ThrowsAsync<ServiceErrorException>(async () => await handler.Handle(command));
    }

    [Fact]
    public async Task CreateUser_InvalidEmail_ThrowsBadRequestException()
    {
        // Arrange
        var handler = new CreateUserHandler(
            new Mock<IPasswordService>().Object,
            new Mock<IUserRepository>().Object,
            new Mock<ICloudStorage>().Object
        );
        var command = new CreateUserCommand("testuser", "invalid-email", "Password123!", "Test", "User");

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(async () => await handler.Handle(command));
    }

    [Fact]
    public async Task CreateUser_PasswordTooShort_ThrowsBadRequestException()
    {
        // Arrange
        var handler = new CreateUserHandler(
            new Mock<IPasswordService>().Object,
            new Mock<IUserRepository>().Object,
            new Mock<ICloudStorage>().Object
        );
        var command = new CreateUserCommand("testuser", "test@example.com", "Pass1!", "Test", "User");

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(async () => await handler.Handle(command));
    }

    [Fact]
    public async Task CreateUser_UsernameTooShort_ThrowsBadRequestException()
    {
        // Arrange
        var handler = new CreateUserHandler(
            new Mock<IPasswordService>().Object,
            new Mock<IUserRepository>().Object,
            new Mock<ICloudStorage>().Object
        );
        var command = new CreateUserCommand("user", "test@example.com", "Password123!", "Test", "User");

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(async () => await handler.Handle(command));
    }

    [Fact]
    public async Task CreateUser_RepositoryCheckFails_ThrowsServiceErrorException()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var cloudStorageMock = new Mock<ICloudStorage>();

        userRepositoryMock.Setup(repo => repo.UsernameOrEmailExists(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Database error"));

        var handler = new CreateUserHandler(passwordServiceMock.Object, userRepositoryMock.Object, cloudStorageMock.Object);
        var command = new CreateUserCommand("testuser", "test@example.com", "Password123!", "Test", "User");

        // Act & Assert
        await Assert.ThrowsAsync<ServiceErrorException>(async () => await handler.Handle(command));
    }
}
