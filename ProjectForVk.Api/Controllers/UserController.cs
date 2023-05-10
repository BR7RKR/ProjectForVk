using Microsoft.AspNetCore.Mvc;
using ProjectForVk.Api.Authorization;
using ProjectForVk.Application.Services;
using ProjectForVk.Core.Entities.DTO;

namespace ProjectForVk.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
public sealed class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUser(int id)
    {
        try
        {
            var user = await _userService.GetUserAsync(id);
            return new ObjectResult(user);
        }
        catch (Exception e)
        {
            return NotFound(new { error = e.Message});
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] PaginationFilterDto filter)
    {
        try
        {
            var user = await _userService.GetUsersAsync(filter);
            return new ObjectResult(user);
        }
        catch (Exception e)
        {
            return NotFound(new { error = e.Message});
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> RegisterUser([FromBody] UserDtoEntity userDto)
    {
        try
        {
            await _userService.AddUserAsync(userDto);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(new { error = e.Message});
        }
    }

    [HttpDelete]
    public async Task<IActionResult> BlockUser(int id)
    {
        try
        {
            await _userService.BlockUserAsync(id);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(new { error = e.Message});
        }
    }
}