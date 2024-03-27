using Microsoft.AspNetCore.Mvc;
using Mnd.Service.Models;

namespace Mnd.Service.Controllers;

[Route("gen")]
[ApiController]
public class GeneratorController : ControllerBase
{
    [HttpGet("")]
    public string Init()
    {
        return "ok";
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