using GestaoPonto.Data;
using GestaoPonto.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GestaoPonto.Controllers
{
    public class ContaController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly DataSeeder _dataSeeder;


        public ContaController(SignInManager<IdentityUser> signInManager, DataSeeder dataSeeder)
        {
            _signInManager = signInManager;
            _dataSeeder = dataSeeder;

        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Verifica se o usuário tem o role "Admin"
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("PainelAdmin", "Admin");
                }

                // Redirecionar colaborador ao seu painel
                return RedirectToAction("PainelColaborador", "Colaborador");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Valida o modelo antes de processar
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Tenta autenticar o usuário
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Senha, model.RememberMe, false);
            if (result.Succeeded)
            {
                // Obtém o usuário autenticado
                var user = await _signInManager.UserManager.FindByEmailAsync(model.Email);

                // Verifica o papel do usuário e redireciona adequadamente
                if (await _signInManager.UserManager.IsInRoleAsync(user, "Admin"))
                {
                    return RedirectToAction("PainelAdmin", "Admin");
                }
                else if (await _signInManager.UserManager.IsInRoleAsync(user, "Colaborador"))
                {
                    return RedirectToAction("PainelColaborador", "Colaborador");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            // Retorna uma mensagem de erro caso a autenticação falhe
            ModelState.AddModelError(string.Empty, "Login inválido. Verifique suas credenciais.");
            return View(model);
        }

        // logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {

            // Faz logout do usuário atual
            await _signInManager.SignOutAsync();

            // Redireciona para a página inicial
            return RedirectToAction("Index", "Home");
        }

        // Acesso Negado
        public IActionResult AcessoNegado()
        {
            return View();
        }
    }
}
