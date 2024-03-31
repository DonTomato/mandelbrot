using SixLabors.ImageSharp.PixelFormats;

namespace Mnd.Core.Render.Smooth;

public class Smooth2PixelRender : IRenderer
{
    private Rgba32[] _colors;

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

        _colors = colors.Select(Rgba32.ParseHex).ToArray();
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
        var diff = new Rgba32(
            BDiff(color1.R, color2.R),
            BDiff(color1.G, color2.G),
            BDiff(color1.B, color2.B)
        );

        var result = new Rgba32(
            Wrap(color1.R, diff.R, frac),
            Wrap(color1.G, diff.G, frac),
            Wrap(color1.B, diff.B, frac)
            );

        return result;
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
}