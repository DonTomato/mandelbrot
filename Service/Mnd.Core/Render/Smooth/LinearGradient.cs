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
        if (colors.Length < 2)
        {
            throw new ArgumentException("The color array must contain at least two colors");
        }
        var fractionSize = 1.0 / (colors.Length - 1);
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
        
        var p1 = _points.Last(e => e.Fraction <= fraction);
        var p2 = _points.First(e => e.Fraction >= fraction);

        if (Equals(p1, p2))
        {
            return p1.Color;
        }

        var f0 = p1.Fraction;
        var f1 = p2.Fraction;
        var f = (fraction - f0) / (f1 - f0);

        var c1 = p1.Color;
        var c2 = p2.Color;

        var color = new Rgba32(
                (byte)(c1.R + (c2.R - c1.R) * f),
                (byte)(c1.G + (c2.G - c1.G) * f),
                (byte)(c1.B + (c2.B - c1.B) * f),
                (byte)(c1.A + (c2.A - c1.A) * f)
            );

        return color;
    }
}