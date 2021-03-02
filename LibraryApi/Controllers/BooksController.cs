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

        [HttpPost("/books")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 15)]
        public async Task<ActionResult> AddABook([FromBody] PostBookRequest request)
        {
            var bookToSave = _mapper.Map<Book>(request);
            _context.Books.Add(bookToSave);
            await _context.SaveChangesAsync();
            var response = _mapper.Map<GetBookDetailsResponse>(bookToSave);

            return CreatedAtRoute("books#getbookbyid", new { id = response.Id }, response);
        }

        [HttpGet("/books")]
        public async Task<ActionResult> GetAllBooks()
        {
            var data = await _context.Books.Where(b => b.IsAvailable)
                .ProjectTo<BookSummaryItem>(_config)
                .ToListAsync();

            var response = new GetBooksSummaryResponse
            {
                Data = data
            };
            return Ok(response);
        }

        [HttpGet("/books/{id:int}", Name ="books#getbookbyid")]
        public async Task<ActionResult> GetBookById(int id)
        {
            var book = await _context.Books
                .Where(b => b.IsAvailable && b.Id == id)
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
