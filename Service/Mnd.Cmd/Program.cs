using Mnd.Core.Contracts;
using Mnd.Core.Generators;

Console.WriteLine("Mandelbrot Set");

var start = DateTime.Now;

var frame = new Frame()
{
    C0Re = 0,
    C0Im = 0,
    Width = 3.0,
    CenterX = -0.3,
    CenterY = 0.0,
    FileName = "0.png",
    FilePath = Path.Combine(AppContext.BaseDirectory, "../../../../../data/tmp")
};

var ctx = new CalcContext
{
    Iterations = 1500,
    FrameWidth = 1000,
    FrameHeight = 600,
    RenderType = RenderType.Smooth
};

SimpleGen.Generate(frame, ctx);

var time = DateTime.Now - start;

Console.WriteLine($"Done {time}");