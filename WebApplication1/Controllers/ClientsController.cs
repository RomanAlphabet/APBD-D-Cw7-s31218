using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Exceptions;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController(IDbService dbService) : ControllerBase
{
    [HttpGet("{id:int}/trips")]
    public async Task<IActionResult> GetClientTripsAsync(int id)
    {
        try
        {
            return Ok(await dbService.GetClientTripsAsync(id));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
        
    }
}