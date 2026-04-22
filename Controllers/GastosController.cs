using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OrganizaDinDin.Application.Interfaces;
using OrganizaDinDin.Application.DTOs;
using OrganizaDinDin.Application.Filters;

namespace OrganizaDinDin.Controllers
{
    [Authorize(Policy = "UserAccess")]
    public class GastosController(IGastoService gastoService) : Controller
    {
        private readonly IGastoService _gastoService = gastoService;

        private const int PageSize = 12;

        public async Task<IActionResult> Index(string? descricao, List<int>? tipos, DateTime? dataInicio, DateTime? dataFim, int pagina = 1)
        {
            pagina = Math.Max(1, pagina);
            var gastosPaginados = await _gastoService.GetGastosFilteredAsync(descricao, tipos, dataInicio, dataFim, pagina, PageSize);

            ViewBag.Descricao = descricao;
            ViewBag.Tipos = tipos ?? [];
            ViewBag.DataInicio = dataInicio?.ToString("yyyy-MM-dd");
            ViewBag.DataFim = dataFim?.ToString("yyyy-MM-dd");

            return View(gastosPaginados);
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] GastoDto gastoDto)
        {
            try
            {
                var gasto = await _gastoService.CreateGastoAsync(gastoDto);
                return Json(new { success = true, data = gasto });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Update([FromBody] GastoDto gastoDto)
        {
            try
            {
                var gasto = await _gastoService.UpdateGastoAsync(gastoDto);
                return Json(new { success = true, data = gasto });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] string id)
        {
            try
            {
                var success = await _gastoService.DeleteGastoAsync(id);
                return Json(new { success });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
