using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LivePaint.Models;

namespace LivePaint.Models
{
    public class DrawModel
    {
        public double StartX { get; set; }
        public double StartY { get; set; }
        public double EndX { get; set; }
        public double EndY { get; set; }
        public int ColorCode { get; set; }
        public double Thickness { get; set; }
    }

}
