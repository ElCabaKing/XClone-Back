
using Application.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Exceptions;
using Infrastructure.Constants;
using Shared.Constants;

namespace Infrastructure.Services;

public class CloudStorageService(Cloudinary cloudinary) : ICloudStorage
{
    public Task DeleteFileAsync(string fileUrl)
    {
        throw new NotImplementedException();
    }

    public async Task<string?> UploadFileAsync(
      Stream? fileStream,
      string? fileName,
      string folder
  )
    {
        if (fileStream == null || fileStream.Length == 0)
        {
            return null;
        }

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName!, fileStream),
            Folder = folder
        };

        var uploadResult = await cloudinary.UploadAsync(uploadParams);

        if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
        {
            return uploadResult.SecureUrl.ToString();
        }
        else
        {
            throw new ServiceErrorException(
                ResponseConstants.CLOUD_ERROR(uploadResult.Error.Message)
            );
        }
    }
}