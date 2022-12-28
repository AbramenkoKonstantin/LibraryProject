using BackEnd.Models;
using BackEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly AuthorsService _authorsService;

    public AuthorsController(AuthorsService booksService) =>
        _authorsService = booksService;

    [HttpGet]
    public async Task<List<Author>> Get() =>
        await _authorsService.GetAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Author>> Get(string id)
    {
        var author = await _authorsService.GetAsync(id);

        if (author is null)
        {
            return NotFound();
        }

        return author;
    }

    [HttpPost, Authorize(Roles="Admin")]
    public async Task<IActionResult> Post(Author newAuthor)
    {
        var author = await _authorsService.GetByNameAsync(newAuthor.Name);

        if (author is not null)
        {
            return BadRequest("Автор уже представлен в библиотеке.");
        }

        await _authorsService.CreateAsync(newAuthor);

        return CreatedAtAction(nameof(Get), new { id = newAuthor.Id }, newAuthor);
    }

    [HttpPut("{id:length(24)}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(string id, Author updatedAuthor)
    {
        var author = await _authorsService.GetAsync(id);

        if (author is null)
        {
            return NotFound();
        }

        updatedAuthor.Id = author.Id;

        await _authorsService.UpdateAsync(id, updatedAuthor);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        var author = await _authorsService.GetAsync(id);

        if (author is null)
        {
            return NotFound();
        }

        await _authorsService.RemoveAsync(id);

        return NoContent();
    }
}