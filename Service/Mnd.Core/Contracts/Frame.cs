using Mnd.Core.Generators;

namespace Mnd.Core.Contracts;

public class Frame
{
    public string UserId { get; set; }
    public double C0Re { get; set; }
    public double C0Im { get; set; }
    public double CenterX { get; set; }
    public double CenterY { get; set; }
    public double Width { get; set; }
    public string FileName { get; set; }
    public string FilePath { get; set; }

    // TODO - pass cancellation token into GeneratePicture
    public void GeneratePicture(CalcContext context, CancellationToken token)
    {
        MGen.Generate(this, context);
    }

    // async ValueTask BuildWorkItemAsync(CancellationToken token, Func<Task> callback)
    // {
    //     // TODO: handle default context
    //     GeneratePicture(new CalcContext
    //     {
    //         Iterations = 1500,
    //         FrameWidth = 1600,
    //         FrameHeight = 1200,
    //         RenderType = RenderType.Smooth
    //     });
    //     await callback();
    // }
}
