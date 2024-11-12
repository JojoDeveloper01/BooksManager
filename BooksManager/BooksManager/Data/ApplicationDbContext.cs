using Microsoft.EntityFrameworkCore;
using BibliotecaMVC.Models;
using BooksManager.Models; // Update to match the namespace of Book

namespace BibliotecaMVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
    }
}
