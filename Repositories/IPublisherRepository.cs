using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;
using System.Threading.Tasks;

namespace WebAPI_simple.Repositories
{
    public interface IPublisherRepository
    {
        Task<List<PublisherDTO>> GetAllPublishersAsync();
        Task<PublisherNoIdDTO?> GetPublisherByIdAsync(int id);
        Task<PublisherDTO?> AddPublisherAsync(AddPublisherRequestDTO addPublisherRequestDTO);
        Task<PublisherNoIdDTO?> UpdatePublisherByIdAsync(int id, PublisherNoIdDTO publisherNoIdDTO);
        Task<Publishers?> DeletePublisherByIdAsync(int id);
    }
}