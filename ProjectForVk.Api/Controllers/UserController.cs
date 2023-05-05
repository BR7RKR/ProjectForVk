using Microsoft.AspNetCore.Mvc;
using ProjectForVk.Application.Services;
using ProjectForVk.Core.Entities.DTO;

namespace ProjectForVk.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class UserController : Controller
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
            return NotFound();
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var user = await _userService.GetUsersAsync();
            return new ObjectResult(user);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return NotFound();
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
            Console.WriteLine(e);
            return BadRequest();
        }
    }

    [HttpPost]
    public async Task<IActionResult> BlockUser([FromBody] int id)
    {
        try
        {
            await _userService.BlockUserAsync(id);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest();
        }
    }
}