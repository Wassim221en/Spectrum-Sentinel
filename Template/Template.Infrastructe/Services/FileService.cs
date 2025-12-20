using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Template.Dashboard.Common.Interfaces;

namespace Template.Infrastructe.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _env;

    public FileService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public bool ValidateFile(IFormFile file, string[] allowedExtensions, double maxFileSizeInMB)
    {
        if (file == null) return false;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension)) return false;

        var maxBytes = maxFileSizeInMB * 1024 * 1024;
        if (file.Length > maxBytes) return false;

        return true;
    }

    public async Task<string> UploadAsync(IFormFile file, string folder)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("ملف غير صالح");

        var uploadsFolder = Path.Combine(_env.WebRootPath, folder);
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        // يرجع المسار النسبي للملف (يمكن استخدامه للعرض لاحقًا)
        return Path.Combine(folder, uniqueFileName).Replace("\\", "/");
    }

    public Task<bool> DeleteAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return Task.FromResult(false);

        var fullPath = Path.Combine(_env.WebRootPath, filePath);
        if (!File.Exists(fullPath)) return Task.FromResult(false);

        File.Delete(fullPath);
        return Task.FromResult(true);
    }
}