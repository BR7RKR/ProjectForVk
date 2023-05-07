using Microsoft.AspNetCore.Mvc;
using ProjectForVk.Api.Authorization;
using ProjectForVk.Application.Services;
using ProjectForVk.Core.Entities.DB;

namespace ProjectForVk.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
public sealed class StateController : Controller
{
    private readonly IStateService _stateService;

    public StateController(IStateService stateService)
    {
        _stateService = stateService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateState([FromBody] UserStateEntity stateEntity)
    {
        try
        {
            await _stateService.AddStateAsync(stateEntity);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(new { error = e.Message});
        }
    }
}