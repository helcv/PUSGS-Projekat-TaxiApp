using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Taxi_App;

[Authorize(Roles = "Admin")]
public class AdminController : BaseApiController
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpPatch("{id}/accept")]
    public async Task<IActionResult> AcceptVerification(int id)
    {
        var result = await _adminService.VerificationAsync(id, true);
        if (!result.IsSuccess)  return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPatch("{id}/deny")]
    public async Task<IActionResult> DenyVerification(int id)
    {
        var result = await _adminService.VerificationAsync(id, false);
        if (!result.IsSuccess)  return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("drivers")]
    public async Task<ActionResult<List<DriverDto>>> GetAllDrivers()
    {
        var result = await _adminService.GetAllDriversAsync();
        return Ok(result);
    }

    [HttpGet("rides")]
    public async Task<ActionResult<List<RideDto>>> GetAllRides()
    {
        var result = await _adminService.GetAllRidesAsync();
        return Ok(result);
    }

    [HttpPatch("{id}/block")]
    public async Task<ActionResult<BlockDriverDto>> BlockDriver(int id)
    {
        var result = await _adminService.ChangeBlockingStatusAsync(id, true);
        if (!result.IsSuccess) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPatch("{id}/unblock")]
    public async Task<ActionResult<BlockDriverDto>> UnblockDriver(int id)
    {
        var result = await _adminService.ChangeBlockingStatusAsync(id, false);
        if (!result.IsSuccess) return BadRequest(result.Error);

        return Ok(result.Value);
    }
}
