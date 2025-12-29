using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OrganizaDinDin.Application.Interfaces;
using OrganizaDinDin.Models;

namespace OrganizaDinDin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GastosController(IGastoService gastoService) : Controller
    {
        private readonly IGastoService _gastoService = gastoService;

        public async Task<IActionResult> Index()
        {
            var gastos = await _gastoService.GetAllGastosAsync();
            return View(gastos);
        }

        public async Task<IActionResult> Resumo()
        {
            var gastos = await _gastoService.GetAllGastosAsync();
            return View(gastos);
        }

        [HttpPost]
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
