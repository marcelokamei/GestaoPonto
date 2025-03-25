using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GestaoPonto.Models;
using GestaoPonto.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text;

namespace GestaoPonto.Controllers
{

    public class ColaboradorController : Controller
    {
        private readonly IColaboradorRepository _colaboradorRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public ColaboradorController(IColaboradorRepository colaboradorRepository, UserManager<IdentityUser> userManager)
        {
            _colaboradorRepository = colaboradorRepository;
            _userManager = userManager;
        }

        // Painel do Colaborador
        [Authorize(Policy = "ColaboradorPolicy")]
        public IActionResult PainelColaborador()
        {
            return View();
        }

        // Registrar Ponto (GET)

        [HttpGet]
        [Authorize(Policy = "ColaboradorPolicy")]
        public IActionResult RegistarPonto()
        {
            return View();
        }

        // Registrar Ponto (POST)
        [HttpPost]
        [Authorize(Policy = "ColaboradorPolicy")]
        public async Task<IActionResult> RegistarPonto(string tipo)
        {
            if (tipo != "Entrada" && tipo != "Saída")
            {
                TempData["ErrorMessage"] = "Tipo de ponto inválido!";
                return RedirectToAction(nameof(PainelColaborador));
            }

            // Extrair o UserId dos Claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                TempData["ErrorMessage"] = "Usuário não autenticado!";
                return RedirectToAction(nameof(PainelColaborador));
            }

            var userId = userIdClaim.Value;

            // Obter o colaborador pelo IdentityUserId
            var colaborador = await _colaboradorRepository.GetByIdentityUserIdAsync(userId);
            if (colaborador == null)
            {
                TempData["ErrorMessage"] = "Colaborador não encontrado!";
                return RedirectToAction(nameof(PainelColaborador));
            }

            var registro = new RegistoPonto
            {
                ColaboradorId = colaborador.Id,
                DataHora = DateTime.Now,
                Tipo = tipo
            };

            try
            {
                await _colaboradorRepository.AddRegistroAsync(registro);
                TempData["Mensagem"] = $"Ponto de {tipo} registrado com sucesso às {registro.DataHora:HH:mm}!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao registrar ponto: {ex.Message}";
            }

            return RedirectToAction(nameof(PainelColaborador));
        }



        // Visualizar Histórico de Registros
        [Authorize(Policy = "ColaboradorPolicy")]
        public async Task<IActionResult> MeusRegistos(int pageNumber = 1, int pageSize = 10)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                TempData["ErrorMessage"] = "Usuário não autenticado!";
                return RedirectToAction(nameof(PainelColaborador));
            }

            var userId = userIdClaim.Value;

            // Obter o colaborador pelo IdentityUserId
            var colaborador = await _colaboradorRepository.GetByIdentityUserIdAsync(userId);
            if (colaborador == null)
            {
                TempData["ErrorMessage"] = "Colaborador não encontrado!";
                return RedirectToAction(nameof(PainelColaborador));
            }

            var totalRegistros = await _colaboradorRepository.GetTotalRegistrosByColaboradorIdAsync(colaborador.Id);
            var totalPages = (int)Math.Ceiling(totalRegistros / (double)pageSize);

            var registros = await _colaboradorRepository.GetRegistrosByColaboradorIdAsync(colaborador.Id, pageNumber, pageSize);
            if (registros == null || !registros.Any())
            {
                TempData["ErrorMessage"] = "Nenhum registro encontrado!";
            }

            ViewData["CurrentPage"] = pageNumber;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalPages"] = totalPages;

            return View(registros);
        }

        //CRUD colaborador
        //criar novo colaborador
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Criar()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Criar(Colaborador colaborador)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new IdentityUser
                    {
                        UserName = colaborador.Email,
                        Email = colaborador.Email
                    };
                    var result = await _userManager.CreateAsync(user, colaborador.Senha);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, colaborador.Role);
                        colaborador.IdentityUserId = user.Id;
                        // Adicionar log de depuração
                        Console.WriteLine($"IdentityUserId atribuído: {colaborador.IdentityUserId}");
                        await _colaboradorRepository.AddAsync(colaborador);
                        TempData["Mensagem"] = "Colaborador criado com sucesso!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Ocorreu um erro ao adicionar o colaborador: {ex.Message}");
                }
            }
            else
            {
                // Adicione mensagens de erro detalhadas
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }
            }
            return View(colaborador);
        }





        //listar colaboradores
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Index()
        {
            var colaboradores = await _colaboradorRepository.GetAllAsync();
            return View(colaboradores);
        }

        //exibir detalhes do colaborador
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Detalhes(int id)
        {
            var colaborador = await _colaboradorRepository.GetByIdAsync(id);
            return View(colaborador);
        }

        //editar colaborador
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Editar(int id)
        {
            var colaborador = await _colaboradorRepository.GetByIdAsync(id);
            if (colaborador == null)
            {
                return NotFound();
            }
            return View(colaborador);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Editar(Colaborador colaborador)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(colaborador.IdentityUserId);
                    if (user != null)
                    {
                        user.UserName = colaborador.Email;
                        user.Email = colaborador.Email;
                        var result = await _userManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            await _colaboradorRepository.UpdateAsync(colaborador);
                            TempData["Mensagem"] = "Colaborador editado com sucesso!";
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Usuário não encontrado.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Ocorreu um erro ao editar o colaborador: {ex.Message}");
                }
            }
            else
            {
                // Adicione mensagens de erro detalhadas
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }
            }
            return View(colaborador);
        }



        // Excluir colaborador (GET)
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Excluir(int id)
        {
            var colaborador = await _colaboradorRepository.GetByIdAsync(id);
            if (colaborador == null)
            {
                return NotFound();
            }
            return View(colaborador);
        }

        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> ExcluirConfirmado(int id)
        {
            try
            {
                var colaborador = await _colaboradorRepository.GetByIdAsync(id);
                if (colaborador != null)
                {
                    var user = await _userManager.FindByIdAsync(colaborador.IdentityUserId);
                    if (user != null)
                    {
                        var result = await _userManager.DeleteAsync(user);
                        if (result.Succeeded)
                        {
                            await _colaboradorRepository.DeleteAsync(colaborador);
                            TempData["Mensagem"] = "Colaborador excluído com sucesso!";
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Usuário não encontrado.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Colaborador não encontrado.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Ocorreu um erro ao excluir o colaborador: {ex.Message}");
            }
            return View();
        }


        // API para Exportar Registros para CSV
        [Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> ExportarRegistrosParaCsv()
        {
            var registros = await _colaboradorRepository.GetAllRegistrosAsync();
            var csv = new StringBuilder();
            csv.AppendLine("Colaborador,DataHora,Tipo");

            foreach (var registro in registros)
            {
                csv.AppendLine($"{registro.Colaborador.Nome},{registro.DataHora:dd/MM/yyyy HH:mm},{registro.Tipo}");
            }

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "registros.csv");
        }


    }
}
