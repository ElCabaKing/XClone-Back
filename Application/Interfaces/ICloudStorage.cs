namespace Application.Interfaces;

public interface ICloudStorage
{
    public Task<string?> UploadFileAsync(
        Stream? FileStream, 
        string? fileName, 
        string folder);
    public Task DeleteFileAsync(string fileUrl);
}