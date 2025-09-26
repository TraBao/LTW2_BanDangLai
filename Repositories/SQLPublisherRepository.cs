using Microsoft.EntityFrameworkCore;
using WebAPI_simple.Data;
using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;
using System.Threading.Tasks;

namespace WebAPI_simple.Repositories
{
    public class SQLPublisherRepository : IPublisherRepository
    {
        private readonly AppDbContext _dbContext;
        public SQLPublisherRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PublisherDTO>> GetAllPublishersAsync()
        {
            return await _dbContext.Publishers.Select(p => new PublisherDTO()
            {
                Id = p.Id,
                Name = p.Name
            }).ToListAsync();
        }

        public async Task<PublisherNoIdDTO?> GetPublisherByIdAsync(int id)
        {
            return await _dbContext.Publishers.Where(p => p.Id == id)
                .Select(p => new PublisherNoIdDTO()
                {
                    Name = p.Name
                }).FirstOrDefaultAsync();
        }

        public async Task<PublisherDTO?> AddPublisherAsync(AddPublisherRequestDTO addPublisherRequestDTO)
        {
            var nameExists = await _dbContext.Publishers
                                .AnyAsync(p => p.Name.ToLower() == addPublisherRequestDTO.Name.ToLower());
            if (nameExists)
            {
                return null;
            }

            var publisherDomain = new Publishers()
            {
                Name = addPublisherRequestDTO.Name
            };
            await _dbContext.Publishers.AddAsync(publisherDomain);
            await _dbContext.SaveChangesAsync();

            return new PublisherDTO { Id = publisherDomain.Id, Name = publisherDomain.Name };
        }

        public async Task<PublisherNoIdDTO?> UpdatePublisherByIdAsync(int id, PublisherNoIdDTO publisherNoIdDTO)
        {
            var publisherDomain = await _dbContext.Publishers.FirstOrDefaultAsync(p => p.Id == id);
            if (publisherDomain != null)
            {
                publisherDomain.Name = publisherNoIdDTO.Name;
                await _dbContext.SaveChangesAsync();
                return publisherNoIdDTO;
            }
            return null;
        }

        public async Task<Publishers?> DeletePublisherByIdAsync(int id)
        {
            var hasBooks = await _dbContext.Books.AnyAsync(b => b.PublisherID == id);
            if (hasBooks)
            {
                return null;
            }

            var publisherDomain = await _dbContext.Publishers.FirstOrDefaultAsync(p => p.Id == id);
            if (publisherDomain != null)
            {
                _dbContext.Publishers.Remove(publisherDomain);
                await _dbContext.SaveChangesAsync();
            }
            return publisherDomain;
        }
    }
}