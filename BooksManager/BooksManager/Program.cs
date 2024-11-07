using BibliotecaMVC.Data;
using Microsoft.EntityFrameworkCore;

namespace BooksManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configura��o do ApplicationDbContext com o SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configurar servi�os de autentica��o, autoriza��o, sess�o e cache
            builder.Services.AddAuthentication("CookieAuthentication")
                .AddCookie("CookieAuthentication", options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });

            builder.Services.AddAuthorization(); // Adicionar servi�o de autoriza��o
            builder.Services.AddDistributedMemoryCache(); // Adicionar cache em mem�ria
            builder.Services.AddSession();

            // Adicionar suporte completo a MVC com Views
            builder.Services.AddControllersWithViews(); // Substitua AddControllers() por AddControllersWithViews()

            var app = builder.Build();

            // Configurar o middleware
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseStaticFiles(); // Permite servir arquivos est�ticos (CSS, JS, imagens)

            // Configurar a rota padr�o
            app.MapDefaultControllerRoute();

            app.Run();
        }
    }
}
