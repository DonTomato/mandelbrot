using SixLabors.ImageSharp.PixelFormats;

namespace Mnd.Core.Render.Smoth;

public class LinearGradient
{
    public LinearGradient(Rgba32[] colors)
    {
        
    }
}

public static class GradientGenerator
{
    public static Func<float, Rgba32> CreateLinearGradient(Rgba32[] colors)
    {
        if (colors == null || colors.Length < 2)
            throw new ArgumentException("At least two colors are required for a gradient.");

        return (float fraction) =>
        {
            if (fraction < 0.0f || fraction > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(fraction), "Fraction must be between 0.0 and 1.0.");

            // Normalize the fraction to the number of colors minus one
            float normalizedFraction = (fraction * (colors.Length - 1));

            // Find the two colors that the fraction falls between
            int index1 = (int)Math.Floor(normalizedFraction);
            int index2 = (int)Math.Ceiling(normalizedFraction);

            // Calculate the fraction between the two colors
            float colorFraction = normalizedFraction - index1;

            // Get the two colors
            Rgba32 color1 = colors[index1];
            Rgba32 color2 = colors[index2];

            // Calculate the new color
            Rgba32 newColor = new Rgba32(
                (byte)(color1.R + (color2.R - color1.R) * colorFraction),
                (byte)(color1.G + (color2.G - color1.G) * colorFraction),
                (byte)(color1.B + (color2.B - color1.B) * colorFraction),
                (byte)(color1.A + (color2.A - color1.A) * colorFraction)
            );

            return newColor;
        };
    }
}