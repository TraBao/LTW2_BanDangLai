// File: Controllers/AuthorsController.cs
using Microsoft.AspNetCore.Mvc;
using WebAPI_simple.Models.DTO;
using WebAPI_simple.Repositories;
using System.Threading.Tasks;

namespace WebAPI_simple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorsController(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        [HttpGet("get-all-author")]
        public async Task<IActionResult> GetAllAuthors()
        {
            var allAuthors = await _authorRepository.GetAllAuthorsAsync();
            return Ok(allAuthors);
        }

        [HttpGet("get-author-by-id/{id}")]
        public async Task<IActionResult> GetAuthorById(int id)
        {
            var author = await _authorRepository.GetAuthorByIdAsync(id);
            if (author == null) return NotFound();
            return Ok(author);
        }

        [HttpPost("add-author")]
        public async Task<IActionResult> AddAuthor([FromBody] AddAuthorRequestDTO addAuthorRequestDTO)
        {
            var author = await _authorRepository.AddAuthorAsync(addAuthorRequestDTO);
            return Ok(author);
        }

        [HttpPut("update-author-by-id/{id}")]
        public async Task<IActionResult> UpdateAuthorById(int id, [FromBody] AuthorNoIdDTO authorNoIdDTO)
        {
            var updatedAuthor = await _authorRepository.UpdateAuthorByIdAsync(id, authorNoIdDTO);
            if (updatedAuthor == null) return NotFound();
            return Ok(updatedAuthor);
        }

        [HttpDelete("delete-author-by-id/{id}")]
        public async Task<IActionResult> DeleteAuthorById(int id)
        {
            var result = await _authorRepository.DeleteAuthorByIdAsync(id);

            if (!result.IsSuccess)
            {
                if (result.Error.Contains("không tồn tại"))
                {
                    return NotFound(result.Error);
                }
                return BadRequest(result.Error);
            }

            return NoContent();
        }
    }
}