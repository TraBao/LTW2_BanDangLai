using Microsoft.EntityFrameworkCore;
using WebAPI_simple.Data;
using WebAPI_simple.Helpers;
using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;

namespace WebAPI_simple.Repositories
{
    public class SQLBookRepository : IBookRepository
    {
        private readonly AppDbContext _dbContext;

        public SQLBookRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<BookWithAuthorAndPublisherDTO>> GetAllBooksAsync(
            string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true,
            int pageNumber = 1, int pageSize = 100)
        {
            var books = _dbContext.Books.AsQueryable();
            if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if (filterOn.Equals("Title", StringComparison.OrdinalIgnoreCase))
                {
                    books = books.Where(x => x.Title.Contains(filterQuery));
                }
            }
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (sortBy.Equals("Title", StringComparison.OrdinalIgnoreCase))
                {
                    books = isAscending ? books.OrderBy(x => x.Title) : books.OrderByDescending(x => x.Title);
                }
            }

            var skipResults = (pageNumber - 1) * pageSize;
            books = books.Skip(skipResults).Take(pageSize);

            var bookDTOs = await books
                .Include(b => b.Publisher)
                .Include(b => b.Book_Authors).ThenInclude(ba => ba.Author)
                .Select(book => new BookWithAuthorAndPublisherDTO()
                {
                    Id = book.Id,
                    Title = book.Title,
                    Description = book.Description,
                    IsRead = book.IsRead,
                    DateRead = book.DateRead,
                    Rate = book.Rate,
                    Genre = book.Genre,
                    CoverUrl = book.CoverUrl,
                    PublisherName = book.Publisher.Name,
                    AuthorNames = book.Book_Authors.Select(ba => ba.Author.FullName).ToList()
                }).ToListAsync();

            return bookDTOs;
        }

        public BookWithAuthorAndPublisherDTO? GetBookById(int id)
        {
            var bookWithDomain = _dbContext.Books
                .Where(b => b.Id == id)
                .Include(b => b.Publisher)
                .Include(b => b.Book_Authors).ThenInclude(ba => ba.Author)
                .Select(book => new BookWithAuthorAndPublisherDTO()
                {
                    Id = book.Id,
                    Title = book.Title,
                    Description = book.Description,
                    IsRead = book.IsRead,
                    DateRead = book.DateRead,
                    Rate = book.Rate,
                    Genre = book.Genre,
                    CoverUrl = book.CoverUrl,
                    PublisherName = book.Publisher.Name,
                    AuthorNames = book.Book_Authors.Select(n => n.Author.FullName).ToList()
                }).FirstOrDefault();

            return bookWithDomain;
        }

        public Result<Books> AddBook(AddBookRequestDTO addBookRequestDTO)
        {
            const int MAX_BOOKS_PER_AUTHOR = 20;
            const int MAX_BOOKS_PER_PUBLISHER_PER_YEAR = 100;
            if (addBookRequestDTO.AuthorIds == null || !addBookRequestDTO.AuthorIds.Any())
            {
                return Result<Books>.Failure("Sách phải có ít nhất một tác giả.");
            }

            var publisherExists = _dbContext.Publishers.Any(p => p.Id == addBookRequestDTO.PublisherID);
            if (!publisherExists)
            {
                return Result<Books>.Failure($"Publisher với ID '{addBookRequestDTO.PublisherID}' không tồn tại.");
            }
            var validAuthorCount = _dbContext.Authors.Count(a => addBookRequestDTO.AuthorIds.Contains(a.Id));
            if (validAuthorCount != addBookRequestDTO.AuthorIds.Count)
            {
                return Result<Books>.Failure("Một hoặc nhiều Author ID không tồn tại.");
            }
            var titleExists = _dbContext.Books.Any(b =>
                b.Title.ToLower() == addBookRequestDTO.Title.ToLower() &&
                b.PublisherID == addBookRequestDTO.PublisherID);
            if (titleExists)
            {
                return Result<Books>.Failure($"Tên sách '{addBookRequestDTO.Title}' đã tồn tại với nhà xuất bản này.");
            }
            foreach (var authorId in addBookRequestDTO.AuthorIds)
            {
                var currentBookCount = _dbContext.Books_Authors.Count(ba => ba.AuthorId == authorId);
                if (currentBookCount >= MAX_BOOKS_PER_AUTHOR)
                {
                    return Result<Books>.Failure($"Tác giả với ID '{authorId}' đã đạt giới hạn {MAX_BOOKS_PER_AUTHOR} cuốn sách.");
                }
            }
            var yearOfBook = addBookRequestDTO.DateAdded.Year;
            var publisherBookCountInYear = _dbContext.Books.Count(b =>
                b.PublisherID == addBookRequestDTO.PublisherID &&
                b.DateAdded.Year == yearOfBook);
            if (publisherBookCountInYear >= MAX_BOOKS_PER_PUBLISHER_PER_YEAR)
            {
                return Result<Books>.Failure($"Nhà xuất bản đã đạt giới hạn {MAX_BOOKS_PER_PUBLISHER_PER_YEAR} cuốn sách trong năm {yearOfBook}.");
            }
            var bookDomainModel = new Books
            {
                Title = addBookRequestDTO.Title,
                Description = addBookRequestDTO.Description,
                IsRead = addBookRequestDTO.IsRead,
                DateRead = addBookRequestDTO.DateRead,
                Rate = addBookRequestDTO.Rate,
                Genre = addBookRequestDTO.Genre,
                CoverUrl = addBookRequestDTO.CoverUrl,
                DateAdded = addBookRequestDTO.DateAdded,
                PublisherID = addBookRequestDTO.PublisherID
            };

            _dbContext.Books.Add(bookDomainModel);
            _dbContext.SaveChanges();

            foreach (var authorId in addBookRequestDTO.AuthorIds)
            {
                var bookAuthor = new Book_Author() { BookId = bookDomainModel.Id, AuthorId = authorId };
                _dbContext.Books_Authors.Add(bookAuthor);
            }
            _dbContext.SaveChanges();
            return Result<Books>.Success(bookDomainModel);
        }

        public AddBookRequestDTO? UpdateBookById(int id, AddBookRequestDTO bookDTO)
        {
            var bookDomain = _dbContext.Books.FirstOrDefault(n => n.Id == id);
            if (bookDomain == null)
            {
                return null;
            }
            var titleExists = _dbContext.Books.Any(b =>
                b.Title.ToLower() == bookDTO.Title.ToLower() &&
                b.PublisherID == bookDTO.PublisherID &&
                b.Id != id);

            if (titleExists)
            {
                return null;
            }

            if (bookDTO.AuthorIds == null || !bookDTO.AuthorIds.Any())
            {
                return null;
            }
            var publisherExists = _dbContext.Publishers.Any(p => p.Id == bookDTO.PublisherID);
            var validAuthorCount = _dbContext.Authors.Count(a => bookDTO.AuthorIds.Contains(a.Id));
            if (!publisherExists || validAuthorCount != bookDTO.AuthorIds.Count)
            {
                return null;
            }
            bookDomain.Title = bookDTO.Title;
            bookDomain.Description = bookDTO.Description;
            bookDomain.IsRead = bookDTO.IsRead;
            bookDomain.DateRead = bookDTO.DateRead;
            bookDomain.Rate = bookDTO.Rate;
            bookDomain.Genre = bookDTO.Genre;
            bookDomain.CoverUrl = bookDTO.CoverUrl;
            bookDomain.DateAdded = bookDTO.DateAdded;
            bookDomain.PublisherID = bookDTO.PublisherID;

            _dbContext.SaveChanges();
            var existingAuthors = _dbContext.Books_Authors.Where(a => a.BookId == id).ToList();
            if (existingAuthors.Any())
            {
                _dbContext.Books_Authors.RemoveRange(existingAuthors);
            }

            foreach (var authorId in bookDTO.AuthorIds)
            {
                var bookAuthor = new Book_Author() { BookId = id, AuthorId = authorId };
                _dbContext.Books_Authors.Add(bookAuthor);
            }
            _dbContext.SaveChanges();
            return bookDTO;
        }

        public Books? DeleteBookById(int id)
        {
            var bookDomain = _dbContext.Books.FirstOrDefault(n => n.Id == id);
            if (bookDomain != null)
            {
                _dbContext.Books.Remove(bookDomain);
                _dbContext.SaveChanges();
            }
            return bookDomain;
        }
    }
}