using Mnd.Core.Contracts;
using Mnd.Core.Generators;
using Mnd.Core.Render.Smooth;

Console.WriteLine("Mandelbrot Set");

var start = DateTime.Now;

var path = Path.Combine(AppContext.BaseDirectory, "../../../../../data/tmp");

Directory.CreateDirectory(path);

var frame = new Frame()
{
    C0Re = 0,
    C0Im = 0,
    Width = 0.3,
    CenterX = 0.4,
    CenterY = 0.27,
    FileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.png",
    FilePath = path
};

const int k = 2;

var ctx = new CalcContext
{
    Iterations = 1500,
    FrameWidth = 1600 * k,
    FrameHeight = 1200 * k,
    RenderType = RenderType.Smooth
};

// SimpleGen.Generate(frame, ctx);
// MGen.Generate(frame, ctx);

double f = 1.1799748935731764;
var renderer = new Smooth2PixelRender();
var color = renderer.CycleColor(f);

var time = DateTime.Now - start;

Console.WriteLine($"Done {time}");