using GestaoPonto.Controllers;
using GestaoPonto.Data;
using GestaoPonto.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GestaoPonto
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //configuração do banco de dados
            builder.Services.AddDbContext<GestaoPontoDbContext>(options =>
            options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                     new MySqlServerVersion(new Version(8, 0, 23))));

            // Configuração do Identity
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<GestaoPontoDbContext>()
                .AddDefaultTokenProviders();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Register DataSeeder
            builder.Services.AddScoped<DataSeeder>();

            //registra o repositório
            builder.Services.AddScoped<IColaboradorRepository, ColaboradorRepository>();

            // politicas de autorização
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
                options.AddPolicy("ColaboradorPolicy", policy => policy.RequireRole("Colaborador"));
            });

            //rota personalizada para usuarios não autorizados
            builder.Services.ConfigureApplicationCookie(options =>
            {
                // Define o tempo de expiração da sessão (em minutos)
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5); // Exemplo: sessão expira após 5 minutos

                // Expira sessão ao fechar o navegador
                options.SlidingExpiration = false;

                options.LoginPath = "/Conta/Login";
                options.AccessDeniedPath = "/Conta/AcessoNegado";
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();            

            app.UseRouting();

            // Middleware de autenticação e autorização
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            //configuração do seeder
            using (var scope = app.Services.CreateScope())
            {
                var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
                await dataSeeder.SeedSuperUser();
            }



            app.Run();
        }
    }
}
