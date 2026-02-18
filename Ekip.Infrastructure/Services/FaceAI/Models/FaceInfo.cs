using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip.Infrastructure.Services.FaceAI.Models
{
    public class FaceInfo
    {
        public float Score { get; set; }
        public Rect Box { get; set; }
        public Point2f[] Landmarks { get; set; }
    }
    
}
