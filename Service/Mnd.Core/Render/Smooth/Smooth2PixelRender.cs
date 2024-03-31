using SixLabors.ImageSharp.PixelFormats;

namespace Mnd.Core.Render.Smooth;

public class Smooth2PixelRender : IRenderer
{
    private RgbColor[] _colors;

    public Smooth2PixelRender()
    {
        var colors = new []
        {
            "#07005d",
            "#111987",
            "#1e4aac",
            "#4376cd",
            "#86afe1",
            "#d0e8f7",
            "#ede7be",
            "#f5c95a",
            "#fda801",
            "#c88101",
            "#945400",
            "#643101",
            "#421206",
            "#0e030e",
            "#050026",
            "#050047"
        };

        var colors2 = new (byte, byte, byte)[]
        {
            (7, 0, 93),
            (17, 25, 135),
            (30, 74, 172),
            (67, 118, 205),
            (134, 175, 225),
            (208, 232, 247),
            (237, 231, 190),
            (245, 201, 90),
            (253, 168, 1),
            (200, 129, 1),
            (148, 81, 0),
            (100, 49, 1),
            (66, 18, 6),
            (14, 3, 14),
            (5, 0, 38),
            (5, 0, 71)
        };

        _colors = colors2.Select(e => new RgbColor(
            ConvByteToD(e.Item1),
            ConvByteToD(e.Item2),
            ConvByteToD(e.Item3))).ToArray();
    }
    
    private class RgbColor
    {
        public RgbColor(double _r, double _g, double _b)
        {
            r = _r;
            g = _g;
            b = _b;
        }
        
        public double r { get; set; }
        public double g { get; set; }
        public double b { get; set; }

        public override string ToString()
        {
            return $"r: {r}; g: {g}; b: {b}";
        }

        public Rgba32 GetColor()
        {
            return new Rgba32(Conver(r), Conver(g), Conver(b));
        }

        static byte Conver(double x)
        {
            var a = Math.Pow(x, 1.0 / 2.2) * 255.0;
            var n = a < 0.0
                ? 0.0
                : (a > 255.0 ? 255.0 : a);
            return (byte)a;
        }
    }

    public Rgba32 GetPixel(int iteration, int maxIterations, (double r, double i) z)
    {
        double fraction = GetFraction(iteration, maxIterations, z);
        return CycleColor(fraction);
    }

    double GetFraction(int iteration, int maxIterations, (double r, double i) z)
    {
        var x = z.r;
        var y = z.i;

        var magn = x * x + y * y;

        var a = (double)iteration + 1.0 - Math.Log2(Math.Log2(magn));
        a /= 6.0;
        return a;
    }

    public Rgba32 CycleColor(double fraction)
    {
        var len = _colors.Length;
        var normX = ((fraction % len) + len) % len;
        int idx = (int)normX;
        var frac = normX - Math.Floor(normX);

        var color1 = _colors[idx];
        var color2 = _colors[(idx + 1) % len];
        var diff = new RgbColor(
            color2.r - color1.r,
            color2.g - color1.g,
            color2.b - color1.b);
        // var diff = new Rgba32(
        //     BDiff(color1.R, color2.R),
        //     BDiff(color1.G, color2.G),
        //     BDiff(color1.B, color2.B)
        // );

        // var result = new Rgba32(
        //     Wrap(color1.R, diff.R, frac),
        //     Wrap(color1.G, diff.G, frac),
        //     Wrap(color1.B, diff.B, frac)
        //     );

        var result = new RgbColor(
            WrapD(color1.r, diff.r, frac),
            WrapD(color1.g, diff.g, frac),
            WrapD(color1.b, diff.b, frac));
        
        return result.GetColor();
    }
    
    static byte BDiff(byte a, byte b)
    {
        return a < b ? (byte)(b - a) : (byte)(a - b); 
    }
    
    static byte Wrap(byte a, byte b, double fraction)
    {
        var r = a + (int)(b * fraction);
        r = r >= 256 ? r - 256 : r;
        return (byte)r;
    }

    static double ConvByteToD(byte x)
    {
        var a = Math.Pow((double)x / 255.0, 2.2);
        return a > 1.0 ? 1.0 : a;
    }
    
    static double WrapD(double a, double b, double fraction)
    {
        return a + fraction * b;
    }
}