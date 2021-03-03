using AutoMapper;
using LibraryApi.Domain;
using LibraryApi.Models.Books;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace LibraryApi.Controllers
{
    public class BooksController : ControllerBase
    {
        private readonly LibraryDataContext _context;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _config;
        private readonly ILogger<BooksController> _logger;

        public BooksController(LibraryDataContext context, IMapper mapper, MapperConfiguration config, ILogger<BooksController> logger)
        {
            _context = context;
            _mapper = mapper;
            _config = config;
            _logger = logger;
        }

        // this is a mini put. It should be use instead of a full put or patch. adding /genre to the end exposes a piece of the resource
        // that can be changed. If you want to allow the user to change more props, just right another method. 
        [HttpPut("/books/{id:int}/genre")]
        public async Task<ActionResult> UpdateGenre(int id, [FromBody] string genre)
        {
            var book = await _context.AvailableBooks.SingleOrDefaultAsync(b => b.Id == id);

            if(book != null)
            {
                book.Genre = genre; // Beware, we aren't validating here. You'd have to find another way to apply validation rules. 
                await _context.SaveChangesAsync();
                return Accepted();
            } else
            {
                return NotFound();
            }
        }

        [HttpDelete("/books/{id:int}")]
        public async Task<ActionResult> RemoveBookFromInventory(int id)
        {
            var book = await _context.AvailableBooks.SingleOrDefaultAsync(b => b.Id == id);
            if(book != null)
            {
                book.IsAvailable = false;
                await _context.SaveChangesAsync();
            }
            return NoContent();
        }


        /// <summary>
        /// Use this to add a book to our inventory. 
        /// </summary>
        /// <param name="request">Which book you want to add</param>
        /// <returns>A new book!</returns>

        [HttpPost("/books")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 15)]
        public async Task<ActionResult<GetBookDetailsResponse>> AddABook([FromBody] PostBookRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var bookToSave = _mapper.Map<Book>(request);
            _context.Books.Add(bookToSave);
            await _context.SaveChangesAsync();
            var response = _mapper.Map<GetBookDetailsResponse>(bookToSave);

            return CreatedAtRoute("books#getbookbyid", new { id = response.Id }, response);
        }

        [HttpGet("/books")]
        public async Task<ActionResult<GetBooksSummaryResponse>> GetAllBooks([FromQuery] string genre = null)
        {
            var query = _context.AvailableBooks;
            if(genre != null)
            {
                query = query.Where(b => b.Genre == genre);
            }

            var data = await query.ProjectTo<BookSummaryItem>(_config).ToListAsync();

            var response = new GetBooksSummaryResponse
            {
                Data = data,
                GenreFilter = genre
            };
            return Ok(response);
        }

        [HttpGet("/books/{id:int}", Name ="books#getbookbyid")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetBookDetailsResponse>> GetBookById(int id)
        {
            var book = await _context.AvailableBooks
                .Where(b => b.Id == id)
                .ProjectTo<GetBookDetailsResponse>(_config)
                .SingleOrDefaultAsync();

            if(book == null)
            {
                _logger.LogWarning("No book with that id!", id);
                return NotFound();
            } else
            {
                return Ok(book);
            }
        }
    }
}
