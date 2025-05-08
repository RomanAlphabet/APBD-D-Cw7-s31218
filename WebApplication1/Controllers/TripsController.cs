using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class TripsController(IDbService dbService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTripsAsync()
    {
        try
        {
            return Ok(await dbService.GetTripsAsync());
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}