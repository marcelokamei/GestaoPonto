using GestaoPonto.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoPonto.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class AdminController : Controller
    {

        private readonly IColaboradorRepository _colaboradorRepository;
        public AdminController(IColaboradorRepository colaboradorRepository)
        {
            _colaboradorRepository = colaboradorRepository;
        }

        // pagina inicial do admin
        public IActionResult PainelAdmin()
        {
            return View();
        }

        // gerenciar colaboradores
        public async Task<IActionResult> GerenciarColaborador()
        {
            // Obtém a lista de colaboradores do repositório
            var colaboradores = await _colaboradorRepository.GetAllAsync();
            return View(colaboradores); // redireciona para a página de colaboradores
        }

        // gerenciar registo de ponto
        public IActionResult VisualizarRegistoPonto()
        {
            return View("Index", "RegistoPonto"); // redireciona para a página de registo de ponto
        }
    }
}
