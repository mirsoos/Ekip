using Ekip.Application.Interfaces;
using Ekip.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace Ekip.Infrastructure.Services.FaceAI
{
    public class InsightFaceVerificationService : IFaceVerificationService
    {
        private readonly InsightFaceEngine _engine;
        private readonly IWebHostEnvironment _environment;
        private readonly IFileService _fileService;

        public InsightFaceVerificationService(InsightFaceEngine engine,IWebHostEnvironment environment,IFileService fileService)
        {
            _engine = engine;
            _environment = environment;
            _fileService = fileService;
        }

        public async Task<(bool, double, Guid, string)> VerifyAsync(string originalAvatarUrl,string capturedPhotoUrl,CancellationToken cancellationToken)
        {
            var root = _environment.WebRootPath;
            var path1 = Path.Combine(root, originalAvatarUrl.TrimStart('/'));
            var path2 = Path.Combine(root, capturedPhotoUrl.TrimStart('/'));

            try
            {
                var emb1 = _engine.GetFaceFeature(path1);
                var emb2 = _engine.GetFaceFeature(path2);
                var similarity = CosineSimilarity(emb1, emb2);

                return (similarity > 0.60, similarity, Guid.NewGuid(), "InsightFace");
            }
            catch(Exception ex)
            {
                return (false, 0.0, Guid.NewGuid(), "InsightFace");
            }
        }

        private static double CosineSimilarity(float[] a, float[] b)
        {
            double dot = 0, normA = 0, normB = 0;
            for (int i = 0; i < a.Length; i++)
            {
                dot += a[i] * b[i];
                normA += a[i] * a[i];
                normB += b[i] * b[i];
            }
            return dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
        }

        public async Task<string> ArchiveFile(string capturedPhotoUrl, CancellationToken cancellationToken)
        {
            var root = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var sourcePath = Path.Combine(root, capturedPhotoUrl.TrimStart('/'));

            var extension = Path.GetExtension(sourcePath);
            var fileName = $"{Guid.NewGuid()}{extension}";

            var datePath = DateTime.Now.ToString("yyyy/MM/dd");
            var destSubFolder = Path.Combine("images", "Evidences");
            var destDirectory = Path.Combine(root, destSubFolder, datePath);

            if (!Directory.Exists(destDirectory))
                Directory.CreateDirectory(destDirectory);

            var destPath = Path.Combine(destDirectory, fileName);

            await Task.Run(() => File.Move(sourcePath, destPath), cancellationToken);

            var relativeUrl = Path.Combine(destSubFolder, datePath, fileName).Replace("\\", "/");
            return $"/{relativeUrl}";
        }

        public async Task DeleteFile(string fileUrl, CancellationToken cancellationToken)
        {
            await _fileService.DeleteFile(fileUrl,cancellationToken);
        }
    }
}
