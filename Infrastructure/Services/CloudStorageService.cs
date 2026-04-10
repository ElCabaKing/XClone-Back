
using Application.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Exceptions;
using Infrastructure.Constants;

namespace Infrastructure.Services;

public class CloudStorageService(Cloudinary cloudinary) : ICloudStorage
{
    public Task DeleteFileAsync(string fileUrl)
    {
        throw new NotImplementedException();
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, fileStream)
        };

        var uploadResult = await cloudinary.UploadAsync(uploadParams);

        if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
        {
            return uploadResult.SecureUrl.ToString();
        }
        else
        {
            throw new ServiceErrorException(ServicesResponseConstants.CLOUD_ERROR(uploadResult.Error.Message));
        }
    }
}