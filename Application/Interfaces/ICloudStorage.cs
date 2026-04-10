namespace Application.Interfaces;

public interface ICloudStorage
{
    public Task<string> UploadFileAsync(Stream FileStream, string fileName);
    public Task DeleteFileAsync(string fileUrl);
}