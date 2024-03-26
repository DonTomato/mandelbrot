using SixLabors.ImageSharp.PixelFormats;

namespace Mnd.Core.Render.Smooth;

public class SmoothPixelRender : IRenderer
{
    private readonly LinearGradient _gradient;
    
    public SmoothPixelRender()
    {
        var colors = new string[]
        {
            "000000",
            "FF0000",
            "FFA500",
            "FFFF00",
            "00FF00",
            "0000FF",
            "4B0082",
            "EE82EE",
            "33FFFF"
        };
        
        _gradient = new LinearGradient(colors.Select(Rgba32.ParseHex).ToArray());
    }

    public Rgba32 GetPixel(int iteration, int maxIterations, (double r, double i) z)
    {
        var x = z.r;
        var y = z.i;
        
        var fraction = (iteration - Math.Log(Math.Log(Math.Sqrt(x * x + y * y)))) / maxIterations;
        return _gradient.GetColorAt(fraction);
    }
}