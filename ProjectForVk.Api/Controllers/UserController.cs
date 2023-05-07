using Microsoft.AspNetCore.Mvc;
using ProjectForVk.Application.Services;
using ProjectForVk.Core.Entities.DTO;

namespace ProjectForVk.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public sealed class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost] // вообще я бы это сделал обычным гет запросом, но в задании было четко сказано делать все запросы через json, поэтому пришлось такое написать
    public async Task<IActionResult> GetUser([FromBody] UserRequestDtoEntity userRequest)
    {
        try
        {
            var user = await _userService.GetUserAsync(userRequest.Id);
            return new ObjectResult(user);
        }
        catch (Exception e)
        {
            return NotFound(new { error = e.Message});
        }
    }
    
    [HttpPost] // вообще я бы это сделал обычным гет запросом, но в задании было четко сказано делать все запросы через json, поэтому пришлось такое написать
    public async Task<IActionResult> GetUsers([FromBody] PaginationFilterDto filter)
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

    [HttpPost]
    public async Task<IActionResult> BlockUser([FromBody] UserRequestDtoEntity userRequest)
    {
        try
        {
            await _userService.BlockUserAsync(userRequest.Id);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(new { error = e.Message});
        }
    }
}