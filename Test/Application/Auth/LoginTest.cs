using Moq;
using Application.Modules.Auth.Login;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Exceptions;

namespace Test.Application.Auth;

public class LoginTest
{
    [Fact]
    public async Task Login_Success()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var tokenServiceMock = new Mock<ITokenService>();

        var user = new Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "testemail@example.com",
            PasswordHash = "hashedpassword"
        };

        userRepositoryMock.Setup(repo => repo.GetByUsernameorEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        passwordServiceMock.Setup(service => service.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        tokenServiceMock.Setup(service => service.CreateToken(It.IsAny<Guid>()))
            .Returns("jwt.token.here");

        var handler = new LoginHandler(tokenServiceMock.Object, userRepositoryMock.Object, passwordServiceMock.Object);
        var command = new LoginCommand("testuser", "Password123!");

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("jwt.token.here", result.Token);
    }

    [Fact]
    public async Task Login_UserNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var tokenServiceMock = new Mock<ITokenService>();

        userRepositoryMock.Setup(repo => repo.GetByUsernameorEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Domain.Entities.User)null);

        var handler = new LoginHandler(tokenServiceMock.Object, userRepositoryMock.Object, passwordServiceMock.Object);
        var command = new LoginCommand("nonexistent", "Password123!");

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command));
    }

    [Fact]
    public async Task Login_InvalidPassword_ThrowsBadRequestException()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var tokenServiceMock = new Mock<ITokenService>();

        var user = new Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashedpassword"
        };

        userRepositoryMock.Setup(repo => repo.GetByUsernameorEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        passwordServiceMock.Setup(service => service.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        var handler = new LoginHandler(tokenServiceMock.Object, userRepositoryMock.Object, passwordServiceMock.Object);
        var command = new LoginCommand("testuser", "WrongPassword123!");

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(async () => await handler.Handle(command));
    }

    [Fact]
    public async Task Login_TokenServiceFails_ThrowsServiceErrorException()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var tokenServiceMock = new Mock<ITokenService>();

        var user = new Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashedpassword"
        };

        userRepositoryMock.Setup(repo => repo.GetByUsernameorEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        passwordServiceMock.Setup(service => service.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        tokenServiceMock.Setup(service => service.CreateToken(It.IsAny<Guid>()))
            .Throws(new Exception("Token creation failed"));

        var handler = new LoginHandler(tokenServiceMock.Object, userRepositoryMock.Object, passwordServiceMock.Object);
        var command = new LoginCommand("testuser", "Password123!");

        // Act & Assert
        await Assert.ThrowsAsync<ServiceErrorException>(async () => await handler.Handle(command));
    }

    [Fact]
    public async Task Login_RepositoryFails_ThrowsServiceErrorException()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var tokenServiceMock = new Mock<ITokenService>();

        userRepositoryMock.Setup(repo => repo.GetByUsernameorEmailAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Database error"));

        var handler = new LoginHandler(tokenServiceMock.Object, userRepositoryMock.Object, passwordServiceMock.Object);
        var command = new LoginCommand("testuser", "Password123!");

        // Act & Assert
        await Assert.ThrowsAsync<ServiceErrorException>(async () => await handler.Handle(command));
    }
}