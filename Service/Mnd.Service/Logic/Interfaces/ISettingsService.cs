using Mnd.Service.SR;

namespace Mnd.Service.Logic.Interfaces;

public interface ISettingsService
{
    string GetStaticPath();
    public (int Width, int Height) GetFrameSize();
    public int GetMaxIterations();
}