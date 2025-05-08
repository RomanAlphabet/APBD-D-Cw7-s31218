using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs;
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
        catch (NoTripsException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateClientAsync([FromBody] ClientCreateDto clientCreateDto)
    {
        try
        {
            return Ok($"Created client with id: {await dbService.CreateClientAsync(clientCreateDto)}");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}