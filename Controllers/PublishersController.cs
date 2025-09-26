// File: Controllers/PublishersController.cs
using Microsoft.AspNetCore.Mvc;
using WebAPI_simple.Models.DTO;
using WebAPI_simple.Repositories;
using System.Threading.Tasks;

namespace WebAPI_simple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherRepository _publisherRepository;
        public PublishersController(IPublisherRepository publisherRepository)
        {
            _publisherRepository = publisherRepository;
        }

        [HttpGet("get-all-publisher")]
        public async Task<IActionResult> GetAllPublishers()
        {
            var allPublishers = await _publisherRepository.GetAllPublishersAsync();
            return Ok(allPublishers);
        }

        [HttpGet("get-publisher-by-id/{id}")]
        public async Task<IActionResult> GetPublisherById(int id)
        {
            var publisher = await _publisherRepository.GetPublisherByIdAsync(id);
            if (publisher == null) return NotFound();
            return Ok(publisher);
        }

        [HttpPost("add-publisher")]
        public async Task<IActionResult> AddPublisher([FromBody] AddPublisherRequestDTO addPublisherRequestDTO)
        {
            var newPublisher = await _publisherRepository.AddPublisherAsync(addPublisherRequestDTO);
            if (newPublisher == null)
            {
                return BadRequest($"Tên nhà xuất bản '{addPublisherRequestDTO.Name}' đã tồn tại.");
            }
            return CreatedAtAction(nameof(GetPublisherById), new { id = newPublisher.Id }, newPublisher);
        }

        [HttpPut("update-publisher-by-id/{id}")]
        public async Task<IActionResult> UpdatePublisherById(int id, [FromBody] PublisherNoIdDTO publisherNoIdDTO)
        {
            var updatedPublisher = await _publisherRepository.UpdatePublisherByIdAsync(id, publisherNoIdDTO);
            if (updatedPublisher == null) return NotFound();
            return Ok(updatedPublisher);
        }

        [HttpDelete("delete-publisher-by-id/{id}")]
        public async Task<IActionResult> DeletePublisherById(int id)
        {
            var publisherExists = await _publisherRepository.GetPublisherByIdAsync(id);
            if (publisherExists == null)
            {
                return NotFound();
            }

            var deletedPublisherDomain = await _publisherRepository.DeletePublisherByIdAsync(id);
            if (deletedPublisherDomain == null)
            {
                return BadRequest("Không thể xóa nhà xuất bản này vì vẫn còn sách tham chiếu.");
            }

            return NoContent();
        }
    }
}