using BackEnd.Models;
using BackEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly BooksService _booksService;
    private readonly AuthorsService _authorsService;
    private readonly GenresService _genresService;

    public BooksController(BooksService booksService, AuthorsService authorsService, GenresService genresService)
    {
        _booksService = booksService;
        _authorsService = authorsService;
        _genresService = genresService;
    }

    [HttpGet]
    public async Task<List<Book>> Get()
    {
        var books = await _booksService.GetAsync();
        foreach (var book in books)
        {
            book.Authors = GetBookAuthors(book.AuthorsId);
            book.Genres = GetBookGenres(book.GenresId);
        }
        return books;
    }

    [HttpGet("name/{name}")]
    public async Task<List<Book>> GetByName(string name)
    {
        var books = await _booksService.GetByNameAsync(name);
        foreach (var book in books)
        {
            book.Authors = GetBookAuthors(book.AuthorsId);
            book.Genres = GetBookGenres(book.GenresId);
        }
        return books;
    }

    [HttpGet("author/{id:length(24)}")]
    public async Task<List<Book>> GetByAuthor(string id)
    {
        var books = await _booksService.GetByAuthorAsync(id);
        foreach (var book in books)
        {
            book.Authors = GetBookAuthors(book.AuthorsId);
            book.Genres = GetBookGenres(book.GenresId);
        }
        return books;
    }

    [HttpGet("genre/{id:length(24)}")]
    public async Task<List<Book>> GetByGenre(string id)
    {
        var books = await _booksService.GetByGenreAsync(id);
        foreach (var book in books)
        {
            book.Authors = GetBookAuthors(book.AuthorsId);
            book.Genres = GetBookGenres(book.GenresId);
        }
        return books;
    }

    [HttpGet("year/{year}")]
    public async Task<List<Book>> GetByPublishingYear(int year)
    {
        var books = await _booksService.GetByYearAsync(year);
        foreach (var book in books)
        {
            book.Authors = GetBookAuthors(book.AuthorsId);
            book.Genres = GetBookGenres(book.GenresId);
        }
        return books;
    }

    [HttpGet("language/{language}")]
    public async Task<List<Book>> GetByOriginalLang(string language)
    {
        var books = await _booksService.GetByLanguageAsync(language);
        foreach (var book in books)
        {
            book.Authors = GetBookAuthors(book.AuthorsId);
            book.Genres = GetBookGenres(book.GenresId);
        }
        return books;
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Book>> Get(string id)
    {
        var book = await _booksService.GetAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        book.Authors = GetBookAuthors(book.AuthorsId);
        book.Genres = GetBookGenres(book.GenresId);

        return book;
    }

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Post(Book newBook)
    {
        var book = await _booksService.GetByISBNAsync(newBook.ISBN);

        if (book is not null)
        {
            return BadRequest("Книга уже представлена в библиотеке.");
        }

        await _booksService.CreateAsync(newBook);

        return CreatedAtAction(nameof(Get), new { id = newBook.Id }, newBook);
    }

    [HttpPut("{id:length(24)}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(string id, Book updatedBook)
    {
        var book = await _booksService.GetAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        updatedBook.Id = book.Id;

        await _booksService.UpdateAsync(id, updatedBook);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        var book = await _booksService.GetAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        await _booksService.RemoveAsync(id);

        return NoContent();
    }

    private List<Genre> GetBookGenres(List<string> genresId) =>
        genresId.Select(genre => _genresService.GetAsync(genre).Result!).ToList();

    private List<Author> GetBookAuthors(List<string> authorsId) =>
        authorsId.Select(author => _authorsService.GetAsync(author).Result!).ToList();
}