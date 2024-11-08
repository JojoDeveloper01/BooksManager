using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksManager.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Gera o hash da senha antes de salvar
                user.PasswordHash = ComputeHash(user.PasswordHash);
                _context.Add(user);
                await _context.SaveChangesAsync();

                // Redireciona para a página de login após o registro
                return RedirectToAction(nameof(Login));
            }
            return View(user);
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            var passwordHash = ComputeHash(password);
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == passwordHash);

            if (user != null)
            {
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("UserName", user.Nome);
                HttpContext.Session.SetString("UserRole", user.Role); // Certifique-se de salvar a role

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Login falhou! Verifique as credenciais.");
            return View();
        }


        // Logout: Limpa a sessão e redireciona para a página de login
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // Função auxiliar para calcular o hash da senha
        private string ComputeHash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
