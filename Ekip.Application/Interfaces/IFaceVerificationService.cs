
namespace Ekip.Application.Interfaces
{
    public interface IFaceVerificationService
    {
        Task<(bool IsMatch, double Confidence, Guid ReferenceId , string provider)> VerifyAsync(string originalAvatarUrl, string capturedPhotoUrl, CancellationToken ct);
        Task<string> ArchiveFile(string capturedPhotoUrl,CancellationToken cancellationToken);
        Task DeleteFile(string fileUrl , CancellationToken cancellationToken);
    }
}
