using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Http;
using static System.Reflection.Metadata.BlobBuilder;
using System.IO;
using System;
using Microsoft.EntityFrameworkCore;
using BooksManager.Models; // Update to match the namespace of Book

namespace BibliotecaMVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnv;

        public BooksController(ApplicationDbContext context, IWebHostEnvironment webHostEnv)
        {
            _context = context;
            _webHostEnv = webHostEnv;  // Initialize _webHostEnv here
        }

        public IActionResult Index()
        {
            // Verifica se o usuário está autenticado
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Account"); // Redireciona para login se não estiver autenticado
            }

            // Verifica se o usuário é administrador
            if (HttpContext.Session.GetString("UserRole") != "Administrador")
            {
                return RedirectToAction("AccessDenied", "Account"); // Redireciona para página de acesso negado
            }

            var books = _context.Books.ToList();
            return View(books); // Exibe a página se o usuário for um administrador
        }

        // Outros métodos de criação, edição e exclusão de livros seguem a mesma lógica de verificação manual.

        // GET: Books/Details/5
        public IActionResult Details(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null) return NotFound();
            return View(book);
        }
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "Administrador")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book b)
        {
            // Save the image and get the path as a string
            string stringFileName = UploadImage(b);
            var book = new Book
            {
                Titulo = b.Titulo,
                Autor = b.Autor,
                Description = b.Description,
                AnoPublicacao = b.AnoPublicacao,
                ImagemPath = stringFileName // Store the image file path as a string
            };

            // Save the `book` entity to the database
            _context.Books.Add(book);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        private string UploadImage(Book b)
        {
            string fileName = null;
            if (b.Imagem != null)
            {
                // Use _webHostEnv to access the web root path
                string uploadDir = Path.Combine(_webHostEnv.WebRootPath, "Images");
                fileName = Guid.NewGuid().ToString() + "-" + b.Imagem.FileName;
                string filePath = Path.Combine(uploadDir, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    b.Imagem.CopyTo(fileStream);
                }
            }
            return fileName;
        }

        // GET: Books/Edit/5
        [HttpGet]
        [ActionName("Edit")]
        public IActionResult EditGet(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null) return NotFound();
            return View("Edit", book); // Explicitly return the "Edit" view
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id, Book book, IFormFile Imagem)
        {
            if (id != book.Id) return NotFound();

            if (ModelState.IsValid)
            {
                // Check if a new image has been uploaded
                if (Imagem != null)
                {
                    // Delete the old image if it exists
                    if (!string.IsNullOrEmpty(book.ImagemPath))
                    {
                        string oldImagePath = Path.Combine(_webHostEnv.WebRootPath, "Images", book.ImagemPath);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Upload the new image
                    string uploadDir = Path.Combine(_webHostEnv.WebRootPath, "Images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Imagem.FileName;
                    string filePath = Path.Combine(uploadDir, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Imagem.CopyToAsync(fileStream);
                    }

                    // Update the ImagemPath with the new file name
                    book.ImagemPath = uniqueFileName;
                }

                try
                {
                    // Update the book record in the database
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Books.Any(e => e.Id == book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View("Edit", book); // Explicitly return the "Edit" view
        }



        // GET: Books/Delete/5
        public IActionResult Delete(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null) return NotFound();
            return View(book);
        }

        // POST: Books/Delete/5
        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            // Redireciona para a lista de livros após a exclusão
            return RedirectToAction(nameof(Index));
        }

    }
}
