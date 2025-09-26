using Microsoft.AspNetCore.Mvc;
using WebAPI_simple.Repositories;
using System.Threading.Tasks;

namespace WebAPI_simple.Controllers
{
    [Route("api/book-authors")]
    [ApiController]
    public class BookAuthorsController : ControllerBase
    {
        private readonly IBookAuthorRepository _bookAuthorRepository;

        public BookAuthorsController(IBookAuthorRepository bookAuthorRepository)
        {
            _bookAuthorRepository = bookAuthorRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddBookAuthor([FromBody] AddBookAuthorRequestDto request)
        {
            var result = await _bookAuthorRepository.AddBookAuthorAsync(request.BookId, request.AuthorId);

            if (!result.IsSuccess)
            {
                if (result.Error == "Conflict")
                {
                    return Conflict("Mối quan hệ tác giả-sách này đã tồn tại.");
                }
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
    }
    public class AddBookAuthorRequestDto
    {
        public int BookId { get; set; }
        public int AuthorId { get; set; }
    }
}