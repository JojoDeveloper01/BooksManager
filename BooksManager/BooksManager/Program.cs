using BibliotecaMVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BooksManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configura o DbContext com o SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configura autentica��o com cookies
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Expira��o do cookie
                    options.SlidingExpiration = true; // Atualiza o te  mpo de expira��o ao usar
                });

            builder.Services.AddAuthorization();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(); // Configura sess�es
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configura o middleware de sess�o e autentica��o/autoriza��o
            app.UseStaticFiles();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            // Configura a rota padr�o
            app.MapDefaultControllerRoute();

            app.Run();
        }
    }
}
