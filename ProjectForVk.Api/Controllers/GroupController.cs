using Microsoft.AspNetCore.Mvc;
using ProjectForVk.Application.Services;
using ProjectForVk.Core.Entities.DB;

namespace ProjectForVk.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class GroupController : Controller
{
    private readonly IGroupService _groupService;

    public GroupController(IGroupService groupService)
    {
        _groupService = groupService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] UserGroupEntity groupEntity)
    {
        try
        {
            await _groupService.AddGroupAsync(groupEntity);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}