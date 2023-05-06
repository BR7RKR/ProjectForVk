using Microsoft.AspNetCore.Mvc;
using ProjectForVk.Application.Services;
using ProjectForVk.Core.Entities.DB;

namespace ProjectForVk.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class StateController : Controller
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
            return BadRequest();
        }
    }
}