using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Http;
using static System.Reflection.Metadata.BlobBuilder;

namespace BibliotecaMVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
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
            string stringFileName = ImagemCaminho(b); // Assuming this method returns the image path as a string
            var book = new Book
            {
                Titulo = b.Titulo,
                Autor = b.Autor,
                AnoPublicacao = b.AnoPublicacao,
                Disponivel = b.Disponivel,
                ImagemCaminho = null // Optionally, use the actual path if storing the file path as a string
            };

            // Save the `book` entity to the database here if needed
            // For example:
            // _context.Books.Add(book);
            // _context.SaveChanges();

            return Ok(book); // Return the created book or another appropriate response
        }


        // GET: Books/Edit/5
        public IActionResult Edit(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null) return NotFound();
            return View(book);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book)
        {
            if (id != book.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
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
