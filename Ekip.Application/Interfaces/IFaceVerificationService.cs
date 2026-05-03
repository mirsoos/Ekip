
namespace Ekip.Application.Interfaces
{
    public interface IFaceVerificationService
    {
        Task<(bool IsMatch, double Confidence, Guid ReferenceId , string provider)> VerifyAsync(string originalAvatarUrl, string capturedPhotoUrl, CancellationToken ct);
        Task<string> ArchiveFileAsync(string capturedPhotoUrl,CancellationToken cancellationToken);
        Task DeleteFileAsync(string fileUrl , CancellationToken cancellationToken);
    }
}
