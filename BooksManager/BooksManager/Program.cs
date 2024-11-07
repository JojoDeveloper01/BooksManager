using BibliotecaMVC.Data;
using Microsoft.EntityFrameworkCore;

namespace BooksManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configuração do ApplicationDbContext com o SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configurar serviços de autenticação, autorização, sessão e cache
            builder.Services.AddAuthentication("CookieAuthentication")
                .AddCookie("CookieAuthentication", options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });

            builder.Services.AddAuthorization(); // Adicionar serviço de autorização
            builder.Services.AddDistributedMemoryCache(); // Adicionar cache em memória
            builder.Services.AddSession();

            // Adicionar suporte completo a MVC com Views
            builder.Services.AddControllersWithViews(); // Substitua AddControllers() por AddControllersWithViews()

            var app = builder.Build();

            // Configurar o middleware
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseStaticFiles(); // Permite servir arquivos estáticos (CSS, JS, imagens)

            // Configurar a rota padrão
            app.MapDefaultControllerRoute();

            app.Run();
        }
    }
}
