using Moq;
using Application.Modules.User.CreateUser;
using Application.Interfaces;
using Domain.Interfaces;

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
            } );

        var handler = new CreateUserHandler(passwordServiceMock.Object, userRepositoryMock.Object, cloudStorageMock.Object);
        var command = new CreateUserCommand
        (
            "testuser",
            "epicouser@example.com",
            "Password123!",
            "Test",
            "User"
        );
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
}
