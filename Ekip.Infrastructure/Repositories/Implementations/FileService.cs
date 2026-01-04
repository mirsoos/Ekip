using Ekip.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class FileService : IFileService
    {   
        private readonly IWebHostEnvironment _environment;
        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folderName ,CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                throw new Exception("File is empty");

            string wwwRootPath = _environment.WebRootPath;
            if (string.IsNullOrEmpty(wwwRootPath))
            {
                wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            string uploadsFolder = Path.Combine(wwwRootPath, "images", folderName);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return $"/images/{folderName}/{uniqueFileName}";
        }

        public async ValueTask DeleteFile(string fileUrl,CancellationToken cancellationToken)
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
