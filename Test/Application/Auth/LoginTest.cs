using Moq;
using Application.Modules.Auth.Login;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Exceptions;
using Domain.Entities;

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
        var tokenRepositoryMock = new Mock<ITokenRepository>();

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

        tokenRepositoryMock.Setup(repo => repo.StoreRefreshTokenAsync(It.IsAny<Token>()))
            .Returns(Task.CompletedTask);

        var handler = new LoginHandler(tokenServiceMock.Object, userRepositoryMock.Object, passwordServiceMock.Object, tokenRepositoryMock.Object);
        var command = new LoginCommand("testuser", "Password123!");

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("jwt.token.here", result.Token);
    }

    [Fact]
    public async Task Login_UserNotFound_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var tokenServiceMock = new Mock<ITokenService>();
        var tokenRepositoryMock = new Mock<ITokenRepository>();

        userRepositoryMock.Setup(repo => repo.GetByUsernameorEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Domain.Entities.User)null);


        var handler = new LoginHandler(tokenServiceMock.Object, userRepositoryMock.Object, passwordServiceMock.Object, tokenRepositoryMock.Object);
        var command = new LoginCommand("nonexistent", "Password123!");

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await handler.Handle(command));
    }

    [Fact]
    public async Task Login_InvalidPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var tokenServiceMock = new Mock<ITokenService>();
        var tokenRepositoryMock = new Mock<ITokenRepository>();

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

        tokenRepositoryMock.Setup(repo => repo.StoreRefreshTokenAsync(It.IsAny<Token>()))
            .Returns(Task.CompletedTask);

        var handler = new LoginHandler(tokenServiceMock.Object, userRepositoryMock.Object, passwordServiceMock.Object, tokenRepositoryMock.Object);
        var command = new LoginCommand("testuser", "WrongPassword123!");

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await handler.Handle(command));
    }

    [Fact]
    public async Task Login_TokenServiceFails_ThrowsServiceErrorException()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var tokenServiceMock = new Mock<ITokenService>();
        var tokenRepositoryMock = new Mock<ITokenRepository>();

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
            .Throws(new ServiceErrorException("Token creation failed"));

        var handler = new LoginHandler(tokenServiceMock.Object, userRepositoryMock.Object, passwordServiceMock.Object, tokenRepositoryMock.Object);
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
        var tokenRepositoryMock = new Mock<ITokenRepository>();
        userRepositoryMock.Setup(repo => repo.GetByUsernameorEmailAsync(It.IsAny<string>()))
            .ThrowsAsync(new ServiceErrorException("User not found"));

        tokenRepositoryMock.Setup(repo => repo.StoreRefreshTokenAsync(It.IsAny<Token>()))
            .ThrowsAsync(new ServiceErrorException("Token creation failed"));

        var handler = new LoginHandler(tokenServiceMock.Object, userRepositoryMock.Object, passwordServiceMock.Object, tokenRepositoryMock.Object);
        var command = new LoginCommand("testuser", "Password123!");

        // Act & Assert
        await Assert.ThrowsAsync<ServiceErrorException>(async () => await handler.Handle(command));
    }
}