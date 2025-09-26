using WebAPI_simple.Helpers;
using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;
using System.Threading.Tasks;

namespace WebAPI_simple.Repositories
{
    public interface IAuthorRepository
    {
        Task<List<AuthorDTO>> GetAllAuthorsAsync();
        Task<AuthorNoIdDTO?> GetAuthorByIdAsync(int id);
        Task<AuthorDTO?> AddAuthorAsync(AddAuthorRequestDTO addAuthorRequestDTO);
        Task<AuthorNoIdDTO?> UpdateAuthorByIdAsync(int id, AuthorNoIdDTO authorNoIdDTO);
        Task<Result<Authors>> DeleteAuthorByIdAsync(int id);
    }
}