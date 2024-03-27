using Mnd.Core.Contracts;

namespace Mnd.Service.Models;

public class FrameRequest
{
    public string UserId { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double W { get; set; }

    public int FrameX { get; set; }
    public int FrameY { get; set; }
    public int FrameW { get; set; }

    public Frame GetFrame()
    {
        var ratio = (double) Height / Width;
        var wn = X * FrameW / Width;

        return new Frame
        {
            UserId = UserId,
            CenterX = X + (FrameX + 0.5 * (FrameW - Width)) * X / Width,
            CenterY = Y - (FrameY + 0.5 * (FrameW - Height) * ratio) * Y / Height,
            Width = W * FrameW / Width,
        };
    }
}
