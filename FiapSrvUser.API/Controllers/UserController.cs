using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using FiapSrvUser.Application.DTOs;
using FiapSrvUser.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapSrvUser.API.Controllers;

[ApiController]
[Route("api/users")]
[ExcludeFromCodeCoverage]
public class UserController : ControllerBase
{
    private readonly IUserService _service;
    public UserController(IUserService service) 
    {
        _service = service;
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Player, Publisher")]
    public async Task<ActionResult<UserDto>> GetById(string id)
    {
        var userId = Guid.Parse(id);
        var user = await _service.GetByIdAsync(userId);

        return Ok(user);
    }

    [HttpGet]
    [Authorize(Roles = "Player, Publisher")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users = await _service.GetAllAsync();
        return Ok(users);
    }

    [HttpPatch]
    [Authorize(Roles = "Player, Publisher")]
    public async Task<IActionResult> Update([FromBody] UpdateUserDto user)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.Name)!);
        await _service.UpdateAsync(userId, user);

        return NoContent(); 
    }
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Player, Publisher")]
    public async Task<IActionResult> Delete()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.Name)!);
        await _service.DeleteAsync(userId);

        return NoContent();
    }

    [HttpGet("players")]
    [Authorize(Roles = "Player, Publisher")]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetPlayers()
    {
        var players = await _service.GetPlayersAsync();
        return Ok(players);
    }

    [HttpGet("publishers")]
    [Authorize(Roles = "Player, Publisher")]
    public async Task<ActionResult<IEnumerable<PublisherDto>>> GetPublishers()
    {
        var publishers = await _service.GetPlublishersAsync();
        return Ok(publishers);
    }
}
