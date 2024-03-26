using SixLabors.ImageSharp.PixelFormats;

namespace Mnd.Core.Render;

public interface IRenderer
{
    Rgba32 GetPixel(int iteration, int maxIterations, (double r, double i) z);
}