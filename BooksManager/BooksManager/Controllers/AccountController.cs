using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Verificar problemas de validação
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(model);
            }

            // Verificar se o email já está em uso
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Este email já está em uso. Por favor, escolha outro.");
                return View(model);
            }

            var user = new User
            {
                Nome = model.Nome,
                Email = model.Email,
                PasswordHash = ComputeHash(model.Password),
                Role = model.Role
            };

            _context.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Login));
        }

        public class RegisterViewModel
        {
            [Required(ErrorMessage = "O nome é obrigatório.")]
            [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
            public string? Nome { get; set; }

            [Required(ErrorMessage = "O email é obrigatório.")]
            [EmailAddress(ErrorMessage = "Formato de email inválido.")]
            public string? Email { get; set; }

            [Required(ErrorMessage = "A senha é obrigatória.")]
            [MinLength(8, ErrorMessage = "A senha deve ter no mínimo 8 caracteres.")]
            public string? Password { get; set; }

            [Required(ErrorMessage = "A função é obrigatória.")]
            public string? Role { get; set; }
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Retorna erros de validação para a view
                return View(model);
            }

            // Verificar se o email existe
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

            if (user == null)
            {
                ModelState.AddModelError("Email", "Este email não está registrado.");
                return View(model);
            }

            // Verificar a senha
            var passwordHash = ComputeHash(model.Password);
            if (user.PasswordHash != passwordHash)
            {
                ModelState.AddModelError("Password", "Senha incorreta.");
                return View(model);
            }

            // Login bem-sucedido
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserName", user.Nome);
            HttpContext.Session.SetString("UserRole", user.Role);

            return RedirectToAction("Index", "Home");
        }

        public class LoginViewModel
        {
            [Required(ErrorMessage = "O email é obrigatório.")]
            [EmailAddress(ErrorMessage = "Formato de email inválido.")]
            public string? Email { get; set; }

            [Required(ErrorMessage = "A senha é obrigatória.")]
            public string? Password { get; set; }
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
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("A senha está vazia ao gerar o hash.");
                return string.Empty;
            }

            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
