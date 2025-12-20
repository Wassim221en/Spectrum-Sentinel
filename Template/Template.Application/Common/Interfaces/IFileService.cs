using Microsoft.AspNetCore.Http;

namespace Template.Dashboard.Common.Interfaces;

public interface IFileService
{
   
    Task<string> UploadAsync(IFormFile file, string folder);
    Task<bool> DeleteAsync(string filePath);
    bool ValidateFile(IFormFile file, string[] allowedExtensions, double maxFileSizeInMB);
}