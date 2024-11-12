using BibliotecaMVC.Data;
using BooksManager.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BooksManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context; // Initialize DbContext
        }

        public IActionResult Index()
        {
            var books = _context.Books.ToList(); // Retrieve books from the database
            return View(books); // Pass the books list to the view
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
