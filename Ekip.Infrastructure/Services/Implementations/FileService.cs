using Ekip.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace Ekip.Infrastructure.Services.Implementations
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> UploadImageAsync(Stream fileStream, string fileName, string folderName, CancellationToken cancellationToken)
        {
            if (fileStream == null || fileStream.Length == 0)
                throw new Exception("File stream is empty");

            string wwwRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string uploadsFolder = Path.Combine(wwwRootPath, "images", folderName);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fs, cancellationToken);
            }

            return $"/images/{folderName}/{uniqueFileName}";
        }

        public async ValueTask DeleteFile(string fileUrl, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(fileUrl)) return;

            string wwwRootPath = _environment.WebRootPath;
            if (string.IsNullOrEmpty(wwwRootPath))
            {
                wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            string physicalPath = Path.Combine(wwwRootPath, fileUrl.TrimStart('/'));

            if (File.Exists(physicalPath))
            {
                try
                {
                    await Task.Run(() => File.Delete(physicalPath), cancellationToken);
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Error deleting file {physicalPath}: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
