
namespace Ekip.Infrastructure.Services.FaceAI.Constants
{
    public static class FaceAiConstants
    {
        public static readonly float[,] ReferencePoints = {
            { 38.2946f, 51.6963f },
            { 73.5318f, 51.5014f },
            { 56.0252f, 71.7366f },
            { 41.5493f, 92.3655f },
            { 70.7299f, 92.2041f }
        };

        public const int TargetImageSize = 640;
        public const int FaceAlignmentSize = 112;
    }
}
