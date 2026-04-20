using Moq;
using Application.Modules.Auth.Login;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Exceptions;
using Domain.Entities;
using System.Linq.Expressions;

namespace Test.Application.Auth;

public class LoginTest
{

    [Fact]
    public async Task Login_Success()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var uowMock = new Mock<IUOW>();
        var tokenServiceMock = new Mock<ITokenService>();
        var tokenRepositoryMock = new Mock<ITokenRepository>();


        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "testemail@example.com",
            PasswordHash = "hashedpassword"
        };

        userRepositoryMock.Setup(repo => repo.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(user);

        passwordServiceMock.Setup(service => service.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        tokenServiceMock.Setup(service => service.CreateToken(It.IsAny<Guid>()))
            .Returns("jwt.token.here");

        tokenRepositoryMock.Setup(repo => repo.StoreRefreshTokenAsync(It.IsAny<Token>()))
            .Returns(Task.CompletedTask);

        uowMock.Setup(uow => uow.UserRepository).Returns(userRepositoryMock.Object);
        uowMock.Setup(uow => uow.TokenRepository).Returns(tokenRepositoryMock.Object);

        var handler = new LoginHandler(tokenServiceMock.Object, uowMock.Object, passwordServiceMock.Object);
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
        var uowMock = new Mock<IUOW>();

        uowMock.Setup(uow => uow.UserRepository).Returns(userRepositoryMock.Object);
        uowMock.Setup(uow => uow.TokenRepository).Returns(tokenRepositoryMock.Object);

        var handler = new LoginHandler(tokenServiceMock.Object, uowMock.Object, passwordServiceMock.Object);
        var command = new LoginCommand("nonexistent", "Password123!");

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await handler.Handle(command));
    }

    [Fact]
    public async Task Login_InvalidPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var uowMock = new Mock<IUOW>();
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

        uowMock.Setup(uow => uow.UserRepository).Returns(userRepositoryMock.Object);
        uowMock.Setup(uow => uow.TokenRepository).Returns(tokenRepositoryMock.Object);

        passwordServiceMock.Setup(service => service.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        tokenRepositoryMock.Setup(repo => repo.StoreRefreshTokenAsync(It.IsAny<Token>()))
            .Returns(Task.CompletedTask);

        var handler = new LoginHandler(tokenServiceMock.Object, uowMock.Object, passwordServiceMock.Object);
        var command = new LoginCommand("testuser", "WrongPassword123!");

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await handler.Handle(command));
    }

}