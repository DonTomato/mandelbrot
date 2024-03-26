using SixLabors.ImageSharp.PixelFormats;

namespace Mnd.Core.Render.Smooth;

public class LinearGradient
{
    private readonly Point[] _points;
    struct Point
    {
        public double Fraction;
        public Rgba32 Color;
    }
    
    public LinearGradient(Rgba32[] colors)
    {
        var fractionSize = 1.0 / colors.Length;
        _points = colors.Select((c, i) => new Point
        {
            Fraction = i * fractionSize,
            Color = c
        }).ToArray();
    }

    public Rgba32 GetColorAt(double fraction)
    {
        if (fraction >= 1.0)
        {
            return _points.Last().Color;
        }

        if (fraction == 0.0)
        {
            return _points.First().Color;
        }
        
        var points = _points.Where(e => e.Fraction >= fraction).Take(2).ToArray();

        if (points[0].Fraction <= fraction || points.Length == 1)
        {
            return points[0].Color;
        }

        var f0 = points[0].Fraction;
        var f1 = points[1].Fraction;
        var f = (fraction - f0) / (f1 - f0);

        var c1 = points[0].Color;
        var c2 = points[1].Color;

        var color = new Rgba32(
                (byte)(c1.R + (c2.R - c1.R) * f),
                (byte)(c1.G + (c2.G - c1.G) * f),
                (byte)(c1.B + (c2.B - c1.B) * f),
                (byte)(c1.A + (c2.A - c1.A) * f)
            );

        return color;
    }
}