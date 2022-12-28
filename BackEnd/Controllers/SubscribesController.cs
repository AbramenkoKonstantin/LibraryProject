using BackEnd.Models;
using BackEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BackEnd.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SubscribesController : ControllerBase
{
    private readonly SubscribesService _subscribesService;
    private readonly BooksService _booksService;

    public SubscribesController(SubscribesService subscribesService, BooksService booksService)
    {
        _subscribesService = subscribesService;
        _booksService = booksService;
    }

    [HttpGet]
    public async Task<List<Subscribe>> Get()
    {
        var subscribes = await _subscribesService.GetAsync();
        foreach (var subscribe in subscribes)
        {
            subscribe.Books = GetSubsribeBooks(subscribe.BooksId);
        }
        return subscribes;
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Subscribe>> Get(string id)
    {
        var subscribe = await _subscribesService.GetAsync(id);

        if (subscribe is null)
        {
            return NotFound();
        }

        subscribe.Books = GetSubsribeBooks(subscribe.BooksId);

        return subscribe;
    }

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Post(Subscribe newSubsribe)
    {
        var subscribe = await _subscribesService.GetByNameAsync(newSubsribe.Name);

        if (subscribe is not null)
        {
            return BadRequest("Подписка уже представлена в библиотеке.");
        }

        await _subscribesService.CreateAsync(newSubsribe);

        return CreatedAtAction(nameof(Get), new { id = newSubsribe.Id }, newSubsribe);
    }

    [HttpPut("{id:length(24)}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(string id, Subscribe updatedSubsribe)
    {
        var subscribe = await _subscribesService.GetAsync(id);

        if (subscribe is null)
        {
            return NotFound();
        }

        updatedSubsribe.Id = subscribe.Id;

        await _subscribesService.UpdateAsync(id, updatedSubsribe);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        var subscribe = await _subscribesService.GetAsync(id);

        if (subscribe is null)
        {
            return NotFound();
        }

        await _subscribesService.RemoveAsync(id);

        return NoContent();
    }

    private List<Book> GetSubsribeBooks(List<string> booksId) =>
        booksId.Select(book => _booksService.GetAsync(book).Result!).ToList();
}