using BackEnd.Models;
using BackEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    private readonly GenresService _genresService;

    public GenresController(GenresService genresService) =>
        _genresService = genresService;

    [HttpGet]
    public async Task<List<Genre>> Get() =>
        await _genresService.GetAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Genre>> Get(string id)
    {
        var genre = await _genresService.GetAsync(id);

        if (genre is null)
        {
            return NotFound();
        }

        return genre;
    }

    [HttpPost, Authorize(Roles="Admin")]
    public async Task<IActionResult> Post(Genre newGenre)
    {
        var genre = await _genresService.GetByNameAsync(newGenre.Name);

        if (genre is not null)
        {
            return BadRequest("Жанр уже представлен в библиотеке.");
        }

        await _genresService.CreateAsync(newGenre);

        return CreatedAtAction(nameof(Get), new { id = newGenre.Id }, newGenre);
    }

    [HttpPut("{id:length(24)}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(string id, Genre updatedGenre)
    {
        var genre = await _genresService.GetAsync(id);

        if (genre is null)
        {
            return NotFound();
        }

        updatedGenre.Id = genre.Id;

        await _genresService.UpdateAsync(id, updatedGenre);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        var genre = await _genresService.GetAsync(id);

        if (genre is null)
        {
            return NotFound();
        }

        await _genresService.RemoveAsync(id);

        return NoContent();
    }
}