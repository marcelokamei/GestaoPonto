using GestaoPonto.Data;
using GestaoPonto.Models;
using GestaoPonto.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoPonto.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class RegistoPontoController : Controller
    {
        private readonly GestaoPontoDbContext _context;
        private readonly IColaboradorRepository _colaboradorRepository;

        public RegistoPontoController(GestaoPontoDbContext context, IColaboradorRepository colaboradorRepository)
        {
            _context = context;
            _colaboradorRepository = colaboradorRepository;
        }

        //listagem de registos de ponto
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            var totalRegistros = await _colaboradorRepository.GetTotalRegistrosAsync();
            var totalPages = (int)Math.Ceiling(totalRegistros / (double)pageSize);

            var registros = await _colaboradorRepository.GetRegistrosAsync(pageNumber, pageSize);
            if (registros == null || !registros.Any())
            {
                TempData["ErrorMessage"] = "Nenhum registro encontrado!";
            }

            ViewData["CurrentPage"] = pageNumber;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalPages"] = totalPages;

            return View(registros);
        }

        /*     
             public async Task<IActionResult> Index()
             {
                 var registosPonto = await _context.RegistosPonto.ToListAsync();
                 return View(registosPonto);
             }*/

        //criar novo registo de ponto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(RegistoPonto registoPonto)
        {
            if (ModelState.IsValid)
            {
                await _context.RegistosPonto.AddAsync(registoPonto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(registoPonto);
        }
    }
}
