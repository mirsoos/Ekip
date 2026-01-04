using Microsoft.AspNetCore.Http;

namespace Ekip.Application.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadImageAsync(IFormFile file ,string folderName,CancellationToken cancellationToken);
        ValueTask DeleteFile(string fileUrl , CancellationToken cancellationToken);
    }
}
