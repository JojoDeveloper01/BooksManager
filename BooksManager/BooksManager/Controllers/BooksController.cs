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
        [HttpPost]
        public IActionResult Create(Book b)
        {
            // Verifica se o `ModelState` é válido antes de tentar salvar os dados no banco de dados
            if (ModelState.IsValid)
            {
                // Salva a imagem e obtém o caminho como string
                string stringFileName = UploadImage(b);
                var book = new Book
                {
                    Titulo = b.Titulo,
                    Autor = b.Autor,
                    Description = b.Description,
                    AnoPublicacao = b.AnoPublicacao,
                    ImagemPath = stringFileName // Armazena o caminho do arquivo da imagem como string
                };

                // Salvar a entidade `book` no banco de dados
                _context.Books.Add(book);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            // Caso o `ModelState` não seja válido, retorna à vista "Create" com os erros de validação
            return View("Create", b);
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
        public async Task<IActionResult> EditPost(int id, Book book, IFormFile? Imagem) // IFormFile é opcional aqui
        {
            if (id != book.Id) return NotFound();

            // A validação ModelState.IsValid não deve considerar a propriedade 'Imagem' obrigatória
            if (ModelState.IsValid)
            {
                // Buscar o livro existente no banco de dados
                var existingBook = await _context.Books.FindAsync(id);
                if (existingBook == null) return NotFound();

                // Atualizar as propriedades individualmente
                existingBook.Titulo = book.Titulo;
                existingBook.Autor = book.Autor;
                existingBook.AnoPublicacao = book.AnoPublicacao;
                existingBook.Description = book.Description; // Atualizar Descrição

                // Verificar se uma nova imagem foi carregada
                if (Imagem != null)
                {
                    // Apagar a imagem antiga, se ela existir
                    if (!string.IsNullOrEmpty(existingBook.ImagemPath))
                    {
                        string oldImagePath = Path.Combine(_webHostEnv.WebRootPath, "Images", existingBook.ImagemPath);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Fazer o upload da nova imagem
                    string uploadDir = Path.Combine(_webHostEnv.WebRootPath, "Images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Imagem.FileName;
                    string filePath = Path.Combine(uploadDir, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Imagem.CopyToAsync(fileStream);
                    }

                    // Atualizar o ImagemPath com o nome do novo arquivo
                    existingBook.ImagemPath = uniqueFileName;
                }

                try
                {
                    // Salvar as alterações no banco de dados
                    _context.Update(existingBook);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Books.Any(e => e.Id == existingBook.Id))
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
            return View("Edit", book); // Retornar explicitamente para a View "Edit"
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
