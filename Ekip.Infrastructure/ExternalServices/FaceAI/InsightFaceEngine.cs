using Ekip.Infrastructure.Services.FaceAI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;

namespace Ekip.Infrastructure.ExternalServices.FaceAI
{


    public class InsightFaceEngine : IDisposable
    {
        private readonly InferenceSession _detector;
        private readonly InferenceSession _recognizer;

        private readonly int[] _strides = { 8, 16, 32 };
        private readonly float _scoreThreshold = 0.5f;
        private readonly float _iouThreshold = 0.4f;

        public InsightFaceEngine(IWebHostEnvironment env)
        {
            var options = new SessionOptions();
            options.AppendExecutionProvider_CPU(0);

            var modelRoot = Path.Combine(env.ContentRootPath, "FaceModels", "antelopev2");

            _detector = new InferenceSession(Path.Combine(modelRoot, "scrfd_10g_bnkps.onnx"), options);
            _recognizer = new InferenceSession(Path.Combine(modelRoot, "glintr100.onnx"), options);
        }

        public void Dispose()
        {
            _detector?.Dispose();
            _recognizer?.Dispose();
        }

        private (Mat blobImage, float scale, int padW, int padH) PreProcess(Mat srcImg)
        {
            int targetSize = 640;
            int width = srcImg.Width;
            int height = srcImg.Height;

            float scale = 1.0f;
            if (width > height)
            {
                scale = (float)targetSize / width;
            }
            else
            {
                scale = (float)targetSize / height;
            }

            int newW = (int)(width * scale);
            int newH = (int)(height * scale);

            var resized = new Mat();
            Cv2.Resize(srcImg, resized, new Size(newW, newH));

            var finalImg = new Mat(new Size(targetSize, targetSize), MatType.CV_8UC3, new Scalar(0, 0, 0));

            var roi = new Rect(0, 0, newW, newH);
            resized.CopyTo(new Mat(finalImg, roi));

            resized.Dispose();

            return (finalImg, scale, 0, 0);
        }

        private DenseTensor<float> CreateInputTensor(Mat img)
        {
            using var rgbImg = new Mat();
            Cv2.CvtColor(img, rgbImg, ColorConversionCodes.BGR2RGB);

            var tensor = new DenseTensor<float>(new[] { 1, 3, img.Height, img.Width });

            var indexer = rgbImg.GetGenericIndexer<Vec3b>();

            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    var pixel = indexer[y, x];

                    tensor[0, 0, y, x] = (pixel.Item0 - 127.5f) / 128.0f;
                    tensor[0, 1, y, x] = (pixel.Item1 - 127.5f) / 128.0f;
                    tensor[0, 2, y, x] = (pixel.Item2 - 127.5f) / 128.0f;
                }
            }

            return tensor;
        }

        private List<Anchor> GenerateAnchors(int height, int width)
        {
            var anchors = new List<Anchor>();
            foreach (var stride in _strides)
            {
                int numGridH = height / stride;
                int numGridW = width / stride;

                for (int q = 0; q < numGridH; q++)
                {
                    for (int k = 0; k < numGridW; k++)
                    {
                        for (int n = 0; n < 2; n++)
                        {
                            anchors.Add(new Anchor
                            {
                                Cx = k * stride,
                                Cy = q * stride,
                                Stride = stride
                            });
                        }
                    }
                }
            }
            return anchors;
        }


        public List<FaceInfo> DetectFaces(string imagePath)
        {
            using var img = Cv2.ImRead(imagePath);
            if (img.Empty()) return new List<FaceInfo>();

            var (blobImg, scale, padW, padH) = PreProcess(img);

            var inputTensor = CreateInputTensor(blobImg);

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor(_detector.InputMetadata.Keys.First(), inputTensor)
            };

            using var results = _detector.Run(inputs);

            var anchors = GenerateAnchors(640, 640);

            var faces = DecodeOutputs(results, anchors);

            var bestFaces = NMS(faces, _iouThreshold);

            foreach (var face in bestFaces)
            {
                face.Box = new Rect(
                    (int)(face.Box.X / scale),
                    (int)(face.Box.Y / scale),
                    (int)(face.Box.Width / scale),
                    (int)(face.Box.Height / scale)
                );

                for (int i = 0; i < face.Landmarks.Length; i++)
                {
                    face.Landmarks[i].X /= scale;
                    face.Landmarks[i].Y /= scale;
                }
            }

            blobImg.Dispose();

            return bestFaces;
        }

        private List<FaceInfo> DecodeOutputs(IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results, List<Anchor> anchors)
        {
            var faces = new List<FaceInfo>();
            var resultArray = results.ToArray();

            int anchorIndex = 0;

            for (int strideIdx = 0; strideIdx < 3; strideIdx++)
            {
                var scoreData = resultArray[strideIdx].AsTensor<float>().ToArray();
                var bboxData = resultArray[strideIdx + 3].AsTensor<float>().ToArray();
                var kpsData = resultArray[strideIdx + 6].AsTensor<float>().ToArray();

                int numAnchors = scoreData.Length;

                for (int i = 0; i < numAnchors; i++)
                {
                    float score = scoreData[i];
                    if (score < _scoreThreshold)
                    {
                        anchorIndex++;
                        continue;
                    }

                    if (anchorIndex >= anchors.Count) break;

                    var anchor = anchors[anchorIndex];

                    float dx1 = bboxData[i * 4 + 0] * anchor.Stride;
                    float dy1 = bboxData[i * 4 + 1] * anchor.Stride;
                    float dx2 = bboxData[i * 4 + 2] * anchor.Stride;
                    float dy2 = bboxData[i * 4 + 3] * anchor.Stride;

                    float x1 = anchor.Cx - dx1;
                    float y1 = anchor.Cy - dy1;
                    float x2 = anchor.Cx + dx2;
                    float y2 = anchor.Cy + dy2;

                    var landmarks = new Point2f[5];
                    for (int k = 0; k < 5; k++)
                    {
                        float ldx = kpsData[i * 10 + (k * 2)] * anchor.Stride;
                        float ldy = kpsData[i * 10 + (k * 2) + 1] * anchor.Stride;

                        landmarks[k] = new Point2f(anchor.Cx + ldx, anchor.Cy + ldy);
                    }

                    faces.Add(new FaceInfo
                    {
                        Score = score,
                        Box = new Rect((int)x1, (int)y1, (int)(x2 - x1), (int)(y2 - y1)),
                        Landmarks = landmarks
                    });

                    anchorIndex++;
                }
            }
            return faces;
        }

        private List<FaceInfo> NMS(List<FaceInfo> faces, float iouThreshold)
        {
            var sortedFaces = faces.OrderByDescending(f => f.Score).ToList();
            var selectedFaces = new List<FaceInfo>();

            while (sortedFaces.Count > 0)
            {
                var best = sortedFaces[0];
                selectedFaces.Add(best);
                sortedFaces.RemoveAt(0);

                for (int i = sortedFaces.Count - 1; i >= 0; i--)
                {
                    var other = sortedFaces[i];

                    float x1 = Math.Max(best.Box.X, other.Box.X);
                    float y1 = Math.Max(best.Box.Y, other.Box.Y);
                    float x2 = Math.Min(best.Box.Right, other.Box.Right);
                    float y2 = Math.Min(best.Box.Bottom, other.Box.Bottom);

                    float w = Math.Max(0, x2 - x1);
                    float h = Math.Max(0, y2 - y1);
                    float interArea = w * h;

                    float boxArea1 = best.Box.Width * best.Box.Height;
                    float boxArea2 = other.Box.Width * other.Box.Height;
                    float unionArea = boxArea1 + boxArea2 - interArea;

                    float iou = interArea / unionArea;

                    if (iou > iouThreshold)
                    {
                        sortedFaces.RemoveAt(i);
                    }
                }
            }

            return selectedFaces;
        }

        private readonly float[,] _referencePoints = {
            { 38.2946f, 51.6963f }, 
            { 73.5318f, 51.5014f },
            { 56.0252f, 71.7366f },
            { 41.5493f, 92.3655f },
            { 70.7299f, 92.2041f }
        };

        private Mat AlignFace(Mat srcImg, Point2f[] landmarks)
        {
            var srcPoints = landmarks;
            var dstPoints = new List<Point2f>();
            for (int i = 0; i < 5; i++)
            {
                dstPoints.Add(new Point2f(_referencePoints[i, 0], _referencePoints[i, 1]));
            }

            using var M = Cv2.EstimateAffinePartial2D(InputArray.Create(srcPoints), InputArray.Create(dstPoints));

            var aligned = new Mat();
            Cv2.WarpAffine(srcImg, aligned, M, new Size(112, 112));
            return aligned;
        }

        private float[] ExtractEmbedding(Mat alignedFace)
        {
            var inputTensor = CreateInputTensor(alignedFace);

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor(_recognizer.InputMetadata.Keys.First(), inputTensor)
            };

            using var results = _recognizer.Run(inputs);
            var output = results.First().AsTensor<float>().ToArray();

            return NormalizeEmbedding(output);
        }

        private float[] NormalizeEmbedding(float[] embedding)
        {
            double sum = 0;
            for (int i = 0; i < embedding.Length; i++) sum += embedding[i] * embedding[i];
            float norm = (float)Math.Sqrt(sum);
            for (int i = 0; i < embedding.Length; i++) embedding[i] /= norm;
            return embedding;
        }

        public float[] GetFaceFeature(string imagePath)
        {
            using var img = Cv2.ImRead(imagePath);
            if (img.Empty()) throw new Exception("Could not read image.");

            var detections = DetectFaces(imagePath);
            if (detections.Count == 0) throw new Exception("No face detected in image.");

            var bestFace = detections.OrderByDescending(f => f.Score).First();

            using var alignedFace = AlignFace(img, bestFace.Landmarks);

            return ExtractEmbedding(alignedFace);
        }

    }
}