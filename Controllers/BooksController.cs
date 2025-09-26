using Microsoft.AspNetCore.Mvc;
using WebAPI_simple.CustomActionFilters;
using WebAPI_simple.Models.DTO;
using WebAPI_simple.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI_simple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet("get-all-books")]
        [Authorize(Roles = "Read,Write")]
        public async Task<IActionResult> GetAllBooks(
            [FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy, [FromQuery] bool? isAscending,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
        {
            var allBooks = await _bookRepository.GetAllBooksAsync(
                filterOn, filterQuery,
                sortBy, isAscending ?? true,
                pageNumber, pageSize
            );
            return Ok(allBooks);
        }

        [HttpGet("get-book-by-id/{id}")]
        public IActionResult GetBookById(int id)
        {
            var book = _bookRepository.GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        [HttpPost("add-book")]
        [Authorize(Roles = "Write")]
        [ValidateModel]
        public IActionResult AddBook([FromBody] AddBookRequestDTO addBookRequestDTO)
        {
            var result = _bookRepository.AddBook(addBookRequestDTO);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }
            var addedBookDomain = result.Value;
            var bookDto = _bookRepository.GetBookById(addedBookDomain.Id);
            return CreatedAtAction(nameof(GetBookById), new { id = addedBookDomain.Id }, bookDto);
        }

        [HttpPut("update-book-by-id/{id}")]
        [Authorize(Roles = "Write")]
        [ValidateModel]
        public IActionResult UpdateBookById(int id, [FromBody] AddBookRequestDTO bookDTO)
        {
            var updatedBookDto = _bookRepository.UpdateBookById(id, bookDTO);

            if (updatedBookDto == null)
            {
                var bookExists = _bookRepository.GetBookById(id) != null;
                if (!bookExists)
                {
                    return NotFound($"Không tìm thấy sách với ID = {id}.");
                }
                else
                {
                    ModelState.AddModelError("", "Không thể cập nhật sách. Vui lòng kiểm tra lại các lý do sau: \n" +
                                                 "- Tên sách đã tồn tại với nhà xuất bản này. \n" +
                                                 "- Sách phải có ít nhất một tác giả. \n" +
                                                 "- Publisher ID và tất cả Author ID phải tồn tại.");
                    return BadRequest(ModelState);
                }
            }
            return Ok(updatedBookDto);
        }

        [HttpDelete("delete-book-by-id/{id}")]
        [Authorize(Roles = "Write")]
        public IActionResult DeleteBookById(int id)
        {
            var deletedBook = _bookRepository.DeleteBookById(id);

            if (deletedBook == null)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}