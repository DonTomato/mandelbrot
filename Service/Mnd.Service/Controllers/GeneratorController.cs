using Microsoft.AspNetCore.Mvc;
using Mnd.Core.Contracts;
using Mnd.Service.BgWorker;
using Mnd.Service.Logic;
using Mnd.Service.Logic.Interfaces;
using Mnd.Service.Models;
using Mnd.Service.SR;

namespace Mnd.Service.Controllers;

[Route("gen")]
[ApiController]
public class GeneratorController(
    IBackgroundTaskQueue taskQueue,
    ISettingsService settings, 
    WsConnectionManager connectionManager)
    : ControllerBase
{
    [HttpGet("")]
    public string Init()
    {
        return "Mandelbrot Set Generator Api";
    }

    [HttpGet("framesize/{userId}")]
    public async Task<FrameSizeResponse> GetFrameSize(string userId)
    {
        var (w, h) = settings.GetFrameSize();
        
        Frame frame = new Frame
        {
            C0Re = 0,
            C0Im = 0,
            Width = 3.5,
            CenterX = -0.7,
            CenterY = 0,
            FileName = "mandelbrot.png",
            FilePath = settings.GetStaticPath()
        };

        if (!Directory.Exists(frame.FilePath))
        {
            Directory.CreateDirectory(frame.FilePath);
        }
        
        var workItem = WorkItemCreator.CreateWorkItem(frame, userId, settings, connectionManager);

        await taskQueue.QueueBackgroundWorkItemAsync(workItem);
        
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
        frame.FileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";

        var workItem = WorkItemCreator.CreateWorkItem(frame, request.UserId, settings, connectionManager);

        await taskQueue.QueueBackgroundWorkItemAsync(workItem);

        return Ok();
    }
}