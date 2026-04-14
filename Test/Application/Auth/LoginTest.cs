using Moq;
using Application.Modules.Auth.Login;
using Application.Interfaces;
using Domain.Interfaces;

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
        var command = new LoginCommand
        (
            "testuser",
            "Password123!"
        );

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("jwt.token.here", result.Token);
    }
}