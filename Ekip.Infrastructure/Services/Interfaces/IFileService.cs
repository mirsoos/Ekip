
namespace Ekip.Infrastructure.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadImageAsync(Stream fileStream, string fileName, string folderName, CancellationToken cancellationToken);
        ValueTask DeleteFile(string fileUrl, CancellationToken cancellationToken);
    }
}
