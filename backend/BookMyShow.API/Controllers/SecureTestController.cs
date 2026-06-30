using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookMyShow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/secure-test")]
public class SecureTestController : ControllerBase
{
    [HttpGet("me")]
    public IActionResult Me()
    {
        var roles = User.FindAll(ClaimTypes.Role)
            .Select(x => x.Value)
            .ToArray();

        return Ok(new
        {
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            Email = User.FindFirstValue(ClaimTypes.Email),
            Roles = roles
        });
    }
}
