using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace GestaoPonto.Data
{
    public class DataSeeder
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DataSeeder(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedSuperUser()
        {
            // Criação de roles
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                var role = new IdentityRole("Admin");
                await _roleManager.CreateAsync(role);
            }

            if (!await _roleManager.RoleExistsAsync("Colaborador"))
            {
                var role = new IdentityRole("Colaborador");
                await _roleManager.CreateAsync(role);
            }

            // Criação de super usuário
            var adminEmail = "kamei@kamei.pt";
            var adminPassword = "ElGalego25.";
            if (await _userManager.FindByEmailAsync(adminEmail) == null)
            {
                var user = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail
                };
                var result = await _userManager.CreateAsync(user, adminPassword);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }

        public async Task SeedDataAsync()
        {
            await SeedSuperUser();
        }
    }
}

