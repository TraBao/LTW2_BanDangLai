using WebAPI_simple.Helpers;
using WebAPI_simple.Models.Domain;

namespace WebAPI_simple.Repositories
{
    public interface IBookAuthorRepository
    {
        Task<Result<Book_Author>> AddBookAuthorAsync(int bookId, int authorId);
    }
}