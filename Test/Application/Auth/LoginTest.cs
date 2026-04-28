using System.Linq.Expressions;
using Application.Cache;
using Application.Interfaces;
using Application.Modules.Auth.Login;
using AppWeb.Controllers;
using AppWeb.Requests.Auth;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Test.Application.Auth;

public class LoginTest
{
    [Fact]
    public async Task Login_ReturnsOk_AndSetsAccessTokenCookie()
    {
        var userId = Guid.NewGuid();
        var userRepositoryMock = new Mock<IUserRepository>();
        var uowMock = new Mock<IUOW>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var tokenServiceMock = new Mock<ITokenService>();
        var tokenCacheMock = new Mock<ITokenCacheServiceGeneric<RefreshTokenCacheEntity>>();

        uowMock.Setup(uow => uow.UserRepository).Returns(userRepositoryMock.Object);

        userRepositoryMock
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new User
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashed-password"
            });

        passwordServiceMock
            .Setup(service => service.VerifyPassword("Password123!", "hashed-password"))
            .Returns(true);

        passwordServiceMock
            .Setup(service => service.HashPassword("refresh-token-value"))
            .Returns("refresh-token-hash");

        tokenServiceMock
            .Setup(service => service.CreateToken(userId))
            .Returns("jwt.token.here");

        tokenServiceMock
            .Setup(service => service.CreateRefreshToken())
            .Returns("refresh-token-value");

        tokenCacheMock
            .Setup(service => service.SaveAsync(It.IsAny<RefreshTokenCacheEntity>()))
            .Returns(Task.CompletedTask);

        var loginHandler = new LoginHandler(
            tokenServiceMock.Object,
            tokenCacheMock.Object,
            uowMock.Object,
            passwordServiceMock.Object);

        var controller = new AuthController(loginHandler, null!, null!)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        var result = await controller.Login(new LoginRequest
        {
            Username = "testuser",
            Password = "Password123!"
        });

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(okResult.Value);
        Assert.Equal("jwt.token.here", response.Token);
        Assert.Equal("refresh-token-value", response.RefreshToken);

        tokenCacheMock.Verify(
            service => service.SaveAsync(It.Is<RefreshTokenCacheEntity>(entity =>
                entity.UserId == userId &&
                entity.TokenHash == "refresh-token-hash" &&
                entity.Purpose == "refresh-token")),
            Times.Once);

        var setCookieHeader = controller.Response.Headers["Set-Cookie"].ToString();
        Assert.Contains("access_token=jwt.token.here", setCookieHeader);
        Assert.Contains("httponly", setCookieHeader, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("secure", setCookieHeader, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("samesite=none", setCookieHeader, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Login_WithInvalidModel_ReturnsBadRequest_AndDoesNotCallUserRepository()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var uowMock = new Mock<IUOW>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var tokenServiceMock = new Mock<ITokenService>();
        var tokenCacheMock = new Mock<ITokenCacheServiceGeneric<RefreshTokenCacheEntity>>();

        uowMock.Setup(uow => uow.UserRepository).Returns(userRepositoryMock.Object);

        var loginHandler = new LoginHandler(
            tokenServiceMock.Object,
            tokenCacheMock.Object,
            uowMock.Object,
            passwordServiceMock.Object);

        var controller = new AuthController(loginHandler, null!, null!)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        controller.ModelState.AddModelError(nameof(LoginRequest.Username), "Required");

        var result = await controller.Login(new LoginRequest());

        Assert.IsType<BadRequestObjectResult>(result);
        userRepositoryMock.Verify(
            repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()),
            Times.Never);
    }
}