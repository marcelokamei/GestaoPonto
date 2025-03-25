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

            //configura��o do banco de dados
            builder.Services.AddDbContext<GestaoPontoDbContext>(options =>
            options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                     new MySqlServerVersion(new Version(8, 0, 23))));

            // Configura��o do Identity
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<GestaoPontoDbContext>()
                .AddDefaultTokenProviders();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Register DataSeeder
            builder.Services.AddScoped<DataSeeder>();

            //registra o reposit�rio
            builder.Services.AddScoped<IColaboradorRepository, ColaboradorRepository>();

            // politicas de autoriza��o
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
                options.AddPolicy("ColaboradorPolicy", policy => policy.RequireRole("Colaborador"));
            });

            //rota personalizada para usuarios n�o autorizados
            builder.Services.ConfigureApplicationCookie(options =>
            {
                // Define o tempo de expira��o da sess�o (em minutos)
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5); // Exemplo: sess�o expira ap�s 5 minutos

                // Expira sess�o ao fechar o navegador
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

            // Middleware de autentica��o e autoriza��o
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            //configura��o do seeder
            using (var scope = app.Services.CreateScope())
            {
                var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
                await dataSeeder.SeedSuperUser();
            }



            app.Run();
        }
    }
}
