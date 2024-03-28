using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Mnd.Service.BgWorker;
using Mnd.Service.Logic.Interfaces;
using Mnd.Service.Models;
using Mnd.Service.SR;

namespace Mnd.Service.Controllers;

[Route("gen")]
[ApiController]
public class GeneratorController(
    IBackgroundTaskQueue taskQueue,
    ISettingsService settings,
    IHubContext<WsHub> hubContext)
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
        
        await taskQueue.QueueBackgroundWorkItemAsync(frame.GetBackgrounWorkItem(async () =>
        {
            await hubContext.Clients.All.SendAsync("SendFrame", request.UserId, JsonSerializer.Serialize(frame));
        }));

        return Ok();
    }
}