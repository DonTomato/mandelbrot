using Mnd.Service.Logic.Interfaces;

namespace Mnd.Service.Logic;

public class SettingsService : ISettingsService
{
    string StaticPath { get; }
    private int Width { get; }
    private int Height { get; }


    public SettingsService(ConfigurationManager config)
    {
        var settings = config.GetSection("Settings");
        StaticPath = settings["StaticPath"]!;

        var frameSize = settings.GetSection("FrameSize");
        if (int.TryParse(frameSize["Width"], out int width))
            Width = width;
        else
            throw new Exception("Invalid FrameSize.Width setting");
        
        if (int.TryParse(frameSize["Height"], out int height))
            Height = height;
        else
            throw new Exception("Invalid FrameSize.Height setting");
    }

    public string GetStaticPath() => StaticPath;
    public (int Width, int Height) GetFrameSize() => (Width, Height);
}