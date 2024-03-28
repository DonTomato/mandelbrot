using Microsoft.AspNetCore.Mvc;
using Mnd.Service.BgWorker;
using Mnd.Service.Logic;
using Mnd.Service.Logic.Interfaces;
using Mnd.Service.Models;

namespace Mnd.Service.Controllers;

[Route("gen")]
[ApiController]
public class GeneratorController(
    IBackgroundTaskQueue taskQueue,
    ISettingsService settings)
    : ControllerBase
{
    [HttpGet("")]
    public string Init()
    {
        return "Mandelbrot Set Generator Api";
    }

    [HttpGet("framesize")]
    public FrameSizeResponse GetFrameSize()
    {
        var (w, h) = settings.GetFrameSize();
        return new FrameSizeResponse
        {
            Width = w,
            Height = h
        };
    }
    
    [HttpPost("frame")]
    public async Task<ActionResult> Generate([FromBody]FrameRequest request)
    {
        var frame = request.GetFrame();
        frame.FilePath = settings.GetStaticPath();
        frame.FileName = DateTime.Now + ".png";

        var workItem = WorkItemCreator.CreateWorkItem(frame, request.UserId, settings);

        await taskQueue.QueueBackgroundWorkItemAsync(workItem);

        return Ok();
    }
}