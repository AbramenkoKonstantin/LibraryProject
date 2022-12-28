using BackEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackEnd.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TokenController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly UsersService _usersService;

    public TokenController(
            TokenService tokenService, 
            UsersService usersService
        )
    {
        _tokenService = tokenService;
        _usersService = usersService;
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        if (HttpContext.User.Identity is not ClaimsIdentity identity)
        {
            return Unauthorized();
        }

        var email = identity.FindFirst("Email")!.Value;
        var role = identity.FindFirst("Role")!.Value;

        var user = await _usersService.GetByEmailAsync(email);

        if (user is null)
        {
            return Unauthorized();
        }

        var refreshToken = user.RefreshToken;

        if (!refreshToken.IsValid)
        {
            return Unauthorized();
        }

        user.AccessToken = _tokenService.GenerateAccessToken(email, role);

        return Ok(user);
    }
}