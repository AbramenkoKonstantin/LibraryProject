using BackEnd.Models;
using BackEnd.Models.User;
using BackEnd.Helpers;
using BackEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UsersService _usersService;
    private readonly SubscribesService _subscribesService;
    private readonly TokenService _tokenService;

    public UsersController(
            UsersService usersService, 
            SubscribesService subscribesService,
            TokenService tokenService
        )
    {
        _usersService = usersService;
        _subscribesService = subscribesService;
        _tokenService = tokenService;
    }

    [HttpGet]
    public async Task<List<User>> Get()
    {
        List<User> users = await _usersService.GetAsync();
        foreach (var user in users)
        {
            user.Subscribes = GetUserSubscribes(user.SubscribesId);
        }
        return users;
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<User>> Get(string id)
    {
        var user = await _usersService.GetAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        user.Subscribes = GetUserSubscribes(user.SubscribesId);

        return user;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody]LoginModel request)
    {
        var existedUser = await _usersService.GetByEmailAsync(request.Email);

        if (existedUser is not null)
        {
            return BadRequest("Почта уже зарегистрирована.");
        }

        PasswordHashingHelper.CreatePasswordHash(
                request.Password,
                out byte[] passwordHash,
                out byte[] passwordSalt
            );

        User user = new()
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            Email = request.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            SubscribesId = new List<DBUserSubscribe>(),
            Subscribes = new List<UserSubscribe>(),
            AccessToken = _tokenService.GenerateAccessToken(request.Email),
            RefreshToken = _tokenService.GenerateRefreshToken(),
            Vallet = 0,
        };

        await _usersService.CreateAsync(user);

        return Ok(user);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<User>> Login([FromBody]LoginModel request)
    {
        var user = await _usersService.GetByEmailAsync(request.Email);

        if (user is null)
        {
            return BadRequest("Неверно указан Email.");
        }

        if (!PasswordHashingHelper.VerifyPasswordHash(
                    request.Password,
                    user.PasswordHash,
                    user.PasswordSalt
                ))
        {
            return BadRequest("Неверный пароль.");
        }

        user.Subscribes = GetUserSubscribes(user.SubscribesId);

        user.AccessToken = _tokenService.GenerateAccessToken(request.Email);
        user.RefreshToken = _tokenService.GenerateRefreshToken();

        await _usersService.UpdateAsync(user.Id!, user);

        return Ok(user);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, User updatedUser)
    {
        var user = await _usersService.GetAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        updatedUser.Id = user.Id;

        await _usersService.UpdateAsync(id, updatedUser);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _usersService.GetAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        await _usersService.RemoveAsync(id);

        return NoContent();
    }

    private List<UserSubscribe> GetUserSubscribes(List<DBUserSubscribe> subscribesId)
    {
        var subscribeList = subscribesId
                    .Select(subscribe => _subscribesService.GetAsync(subscribe.SubscribeId!).Result!)
                    .ToList();

        var userSubscribeList = subscribeList
            .Select(sub => {
                var subExp = subscribesId
                    .Where(expireSub => expireSub.SubscribeId == sub.Id)
                    .Single()
                    .Expires;
                return new UserSubscribe()
                {
                    Id = sub.Id,
                    Name = sub.Name,
                    BooksId = sub.BooksId,
                    Books = sub.Books,
                    Description = sub.Description,
                    Expires = subExp,
                    ImageLink = sub.ImageLink,
                    Price = sub.Price
                };
            })
            .ToList();

        return userSubscribeList;
    }
}