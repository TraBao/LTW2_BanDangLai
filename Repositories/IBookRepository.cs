using WebAPI_simple.Helpers;
using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;

namespace WebAPI_simple.Repositories
{
    public interface IBookRepository
    {
        Task<List<BookWithAuthorAndPublisherDTO>> GetAllBooksAsync(
            string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true,
            int pageNumber = 1, int pageSize = 100);
        BookWithAuthorAndPublisherDTO? GetBookById(int id);
        Result<Books> AddBook(AddBookRequestDTO addBookRequestDTO);

        AddBookRequestDTO? UpdateBookById(int id, AddBookRequestDTO bookDTO);
        Books? DeleteBookById(int id);
    }
}