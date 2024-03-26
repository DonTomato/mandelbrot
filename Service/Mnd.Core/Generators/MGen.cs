using System.Collections.Concurrent;
using System.Drawing;
using System.Net.Mime;
using Mnd.Core.Contracts;
using Mnd.Core.Render;
using Mnd.Core.Render.Smooth;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Mnd.Core.Generators;

public static class MGen
{
    public static void Generate(Frame frame, CalcContext context)
    {
        IRenderer renderer = new SmoothPixelRender();
        
        var realHeight = frame.Width * context.FrameHeight / context.FrameWidth;

        var x0 = frame.CenterX - frame.Width / 2;
        var y0 = frame.CenterY - realHeight / 2;
        var xe = x0 + frame.Width;
        var ye = y0 + realHeight;

        var bug = new ConcurrentBag<(int, Rgba32[])>();

        Parallel.For(0, context.FrameWidth, x =>
        {
            var data = new Rgba32[context.FrameHeight];
            for (int y = 0; y < context.FrameHeight; y++)
            {
                var xr = (double) x;
                var yr = (double)(context.FrameHeight - y);

                var z = (Re: frame.C0Re, Im: frame.C0Im);

                var c = (
                    Re: x0 + (xr / context.FrameWidth) * (xe - x0),
                    Im: y0 + (yr / context.FrameHeight) * (ye - y0)
                );

                var color = Rgba32.ParseHex("000");

                for (int i = 0; i < context.Iterations; i++)
                {
                    if (z.Re * z.Re + z.Im * z.Im > 4.0)
                    {
                        color = renderer.GetPixel(i, context.Iterations, z);
                        break;
                    }

                    z = (
                        Re: z.Re * z.Re - z.Im * z.Im + c.Re,
                        Im: 2.0 * z.Re * z.Im + c.Im
                    );
                }

                data[y] = color;
            }

            bug.Add((x, data));
        });

        using (Image<Rgba32> bitmap = new Image<Rgba32>(context.FrameWidth, context.FrameHeight))
        {
            var data = bug.ToArray().OrderBy(e => e.Item1).ToArray();
            
            for (int x = 0; x < context.FrameWidth; x++)
            {
                for (int y = 0; y < context.FrameHeight; y++)
                {
                    bitmap[x, y] = data[x].Item2[y];
                }
            }

            var fileName = Path.Combine(frame.FilePath, frame.FileName);
            bitmap.Save(fileName);
        }
    }
}