// File: Repositories/SQLAuthorRepository.cs
using Microsoft.EntityFrameworkCore;
using WebAPI_simple.Data;
using WebAPI_simple.Helpers;
using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;
using System.Threading.Tasks;

namespace WebAPI_simple.Repositories
{
    public class SQLAuthorRepository : IAuthorRepository
    {
        private readonly AppDbContext _dbContext;

        public SQLAuthorRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<AuthorDTO>> GetAllAuthorsAsync()
        {
            return await _dbContext.Authors.Select(author => new AuthorDTO()
            {
                Id = author.Id,
                FullName = author.FullName
            }).ToListAsync();
        }

        public async Task<AuthorNoIdDTO?> GetAuthorByIdAsync(int id)
        {
            return await _dbContext.Authors.Where(a => a.Id == id)
                .Select(author => new AuthorNoIdDTO()
                {
                    FullName = author.FullName
                }).FirstOrDefaultAsync();
        }

        public async Task<AuthorDTO?> AddAuthorAsync(AddAuthorRequestDTO addAuthorRequestDTO)
        {
            var authorDomain = new Authors
            {
                FullName = addAuthorRequestDTO.FullName
            };

            await _dbContext.Authors.AddAsync(authorDomain);
            await _dbContext.SaveChangesAsync();

            return new AuthorDTO { Id = authorDomain.Id, FullName = authorDomain.FullName };
        }

        public async Task<AuthorNoIdDTO?> UpdateAuthorByIdAsync(int id, AuthorNoIdDTO authorNoIdDTO)
        {
            var authorDomain = await _dbContext.Authors.FirstOrDefaultAsync(a => a.Id == id);
            if (authorDomain != null)
            {
                authorDomain.FullName = authorNoIdDTO.FullName;
                await _dbContext.SaveChangesAsync();
                return authorNoIdDTO;
            }
            return null;
        }

        public async Task<Result<Authors>> DeleteAuthorByIdAsync(int id)
        {
            var authorDomain = await _dbContext.Authors.FirstOrDefaultAsync(a => a.Id == id);
            if (authorDomain == null)
            {
                return Result<Authors>.Failure($"Tác giả với ID '{id}' không tồn tại.");
            }

            var hasBooks = await _dbContext.Books_Authors.AnyAsync(ba => ba.AuthorId == id);
            if (hasBooks)
            {
                return Result<Authors>.Failure("Tác giả này vẫn còn sách trong hệ thống. Hãy gỡ liên kết trong Book_Author trước khi xóa.");
            }

            _dbContext.Authors.Remove(authorDomain);
            await _dbContext.SaveChangesAsync();

            return Result<Authors>.Success(authorDomain);
        }
    }
}