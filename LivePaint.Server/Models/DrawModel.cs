namespace LivePaint.Server.Models
{
    public class DrawModel
    {
        public double startX { get; set; }
        public double startY { get; set; }
        public double currX { get; set; }
        public double currY { get; set; }
        public string color { get; set; }
        public double thickness { get; set; }
    }
}
