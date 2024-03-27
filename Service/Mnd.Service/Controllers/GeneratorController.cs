using Microsoft.AspNetCore.Mvc;
using Mnd.Service.BgWorker;
using Mnd.Service.Models;

namespace Mnd.Service.Controllers;

[Route("gen")]
[ApiController]
public class GeneratorController : ControllerBase
{
    private readonly CancellationToken _cancellationToken;
    private readonly IBackgroundTaskQueue _taskQueue;
    
    public GeneratorController(
        IBackgroundTaskQueue taskQueue,
        IHostApplicationLifetime applicationLifetime)
    {
        _cancellationToken = applicationLifetime.ApplicationStopping;
        _taskQueue = taskQueue;
    }
    
    [HttpGet("")]
    public string Init()
    {
        return "Mandelbrot Set Generator Api";
    }
    
    [HttpPost("frame")]
    public ActionResult Generate([FromBody]FrameRequest request)
    {
        var frame = request.GetFrame();
        frame.FilePath = "../../../../../data/static";
        frame.FileName = DateTime.Now + ".png";

        return Ok();
    }
}