using Moq;
using Application.Modules.Users.CreateUser;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Exceptions;
using Domain.Entities;
using System.Linq.Expressions;

namespace Test.Application.Users;

public class CreateUserTest
{
    [Fact]
    public async Task CreateUser_Success()
    {
        // Arrange
        var uowMock = new Mock<IUOW>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var emailTemplateRepositoryMock = new Mock<IEmailTemplateRepository>();
        var cloudStorageMock = new Mock<ICloudStorage>();
        var emailServiceMock = new Mock<IEmailService>();

        uowMock.Setup(uow => uow.UserRepository).Returns(userRepositoryMock.Object);
        uowMock.Setup(uow => uow.EmailTemplateRepository).Returns(emailTemplateRepositoryMock.Object);

        userRepositoryMock.Setup(repo => repo.UsernameOrEmailExists(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        emailTemplateRepositoryMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<EmailTemplate, bool>>>()))
            .ReturnsAsync(new EmailTemplate
            {
                Name = "WelcomeEmail",
                Subject = "Welcome to XClone!",
                Body = "Hello {{first_name}}, welcome to XClone!"
            });

        passwordServiceMock.Setup(service => service.HashPassword(It.IsAny<string>()))
            .Returns("hashedpassword");
        cloudStorageMock.Setup(storage => storage.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("http://example.com/profile.jpg");

        userRepositoryMock.Setup(repo => repo.Create(It.IsAny<User>()))
            .ReturnsAsync((User user) => new User
            {
                Id = Guid.NewGuid(),
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePictureUrl = user.ProfilePictureUrl
            });

        var handler = new CreateUserHandler(emailServiceMock.Object, passwordServiceMock.Object, uowMock.Object, cloudStorageMock.Object);
        var command = new CreateUserCommand("testuser", "epicouser@example.com", "Password123!", "Test", "User");

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Username, result.Data.Username);
        Assert.Equal(command.Email, result.Data.Email);
        Assert.Equal(command.FirstName, result.Data.FirstName);
        Assert.Equal(command.LastName, result.Data.LastName);
        userRepositoryMock.Verify(repo => repo.Create(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task CreateUser_UsernameAlreadyExists_ThrowsAlreadyExistsException()
    {
        // Arrange
        var uowMock = new Mock<IUOW>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var cloudStorageMock = new Mock<ICloudStorage>();
        var emailServiceMock = new Mock<IEmailService>();

        uowMock.Setup(uow => uow.UserRepository).Returns(userRepositoryMock.Object);

        userRepositoryMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new User());

        var handler = new CreateUserHandler(emailServiceMock.Object, passwordServiceMock.Object, uowMock.Object, cloudStorageMock.Object);
        var command = new CreateUserCommand("existinguser", "new@example.com", "Password123!", "Test", "User");

        // Act & Assert
        await Assert.ThrowsAsync<AlreadyExistsException>(async () => await handler.Handle(command));
    }

    [Fact]
    public async Task CreateUser_CloudStorageUploadFails_ThrowsServiceErrorException()
    {
        // Arrange
        var uowMock = new Mock<IUOW>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var cloudStorageMock = new Mock<ICloudStorage>();
        var emailServiceMock = new Mock<IEmailService>();

        uowMock.Setup(uow => uow.UserRepository).Returns(userRepositoryMock.Object);

        userRepositoryMock.Setup(repo => repo.UsernameOrEmailExists(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        passwordServiceMock.Setup(service => service.HashPassword(It.IsAny<string>()))
            .Returns("hashedpassword");

        cloudStorageMock.Setup(storage => storage.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(string.Empty); // Simula un fallo en la subida

        var handler = new CreateUserHandler(emailServiceMock.Object, passwordServiceMock.Object, uowMock.Object, cloudStorageMock.Object);
        var command = new CreateUserCommand("testuser", "test@example.com", "Password123!", "Test", "User", new MemoryStream(), "profile.jpg");

        // Act & Assert
        await Assert.ThrowsAsync<ServiceErrorException>(async () => await handler.Handle(command));
    }


    [Fact]
    public async Task CreateUser_RepositoryCheckFails_ThrowsServiceErrorException()
    {
        // Arrange
        var uowMock = new Mock<IUOW>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var cloudStorageMock = new Mock<ICloudStorage>();
        var emailServiceMock = new Mock<IEmailService>();

        uowMock.Setup(uow => uow.UserRepository).Returns(userRepositoryMock.Object);

        userRepositoryMock.Setup(repo => repo.UsernameOrEmailExists(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new ServiceErrorException("Database error"));

        var handler = new CreateUserHandler(emailServiceMock.Object, passwordServiceMock.Object, uowMock.Object, cloudStorageMock.Object);
        var command = new CreateUserCommand("testuser", "test@example.com", "Password123!", "Test", "User", new MemoryStream(), "profile.jpg"   );

        // Act & Assert
        await Assert.ThrowsAsync<ServiceErrorException>(async () => await handler.Handle(command));
    }
}
