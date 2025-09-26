using Microsoft.EntityFrameworkCore;
using WebAPI_simple.Models.Domain;

namespace WebAPI_simple.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book_Author>()
                .HasOne(b => b.Book)
                .WithMany(ba => ba.Book_Authors)
                .HasForeignKey(bi => bi.BookId);

            modelBuilder.Entity<Book_Author>()
                .HasOne(b => b.Author)
                .WithMany(ba => ba.Book_Authors)
                .HasForeignKey(bi => bi.AuthorId);

            modelBuilder.Entity<Publishers>().HasData(
                new Publishers { Id = 1, Name = "Nhà xuất bản Kim Đồng" },
                new Publishers { Id = 2, Name = "Nhà xuất bản Trẻ" },
                new Publishers { Id = 3, Name = "Nhà xuất bản Giáo Dục" }
            );

            modelBuilder.Entity<Authors>().HasData(
                new Authors { Id = 1, FullName = "Nguyễn Nhật Ánh" },
                new Authors { Id = 2, FullName = "Tô Hoài" },
                new Authors { Id = 3, FullName = "Ngô Tất Tố" }
            );
        }

        public DbSet<Books> Books { get; set; }
        public DbSet<Authors> Authors { get; set; }
        public DbSet<Book_Author> Books_Authors { get; set; }
        public DbSet<Publishers> Publishers { get; set; }
    }
}