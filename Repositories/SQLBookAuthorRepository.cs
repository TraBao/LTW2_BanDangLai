using Microsoft.EntityFrameworkCore;
using WebAPI_simple.Data;
using WebAPI_simple.Helpers;
using WebAPI_simple.Models.Domain;
using System.Threading.Tasks;

namespace WebAPI_simple.Repositories
{
    public class SQLBookAuthorRepository : IBookAuthorRepository
    {
        private readonly AppDbContext _dbContext;

        public SQLBookAuthorRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<Book_Author>> AddBookAuthorAsync(int bookId, int authorId)
        {
            var bookExists = await _dbContext.Books.AnyAsync(b => b.Id == bookId);
            if (!bookExists)
            {
                return Result<Book_Author>.Failure($"Sách với ID '{bookId}' không tồn tại.");
            }
            var authorExists = await _dbContext.Authors.AnyAsync(a => a.Id == authorId);
            if (!authorExists)
            {
                return Result<Book_Author>.Failure($"Tác giả với ID '{authorId}' không tồn tại.");
            }
            var relationshipExists = await _dbContext.Books_Authors
                .AnyAsync(ba => ba.BookId == bookId && ba.AuthorId == authorId);

            if (relationshipExists)
            {
                return Result<Book_Author>.Failure("Conflict");
            }

            var newBookAuthor = new Book_Author
            {
                BookId = bookId,
                AuthorId = authorId
            };

            await _dbContext.Books_Authors.AddAsync(newBookAuthor);
            await _dbContext.SaveChangesAsync();

            return Result<Book_Author>.Success(newBookAuthor);
        }
    }
}