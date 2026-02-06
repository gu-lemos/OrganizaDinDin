using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OrganizaDinDin.Application.Interfaces;
using OrganizaDinDin.Application.DTOs;
using OrganizaDinDin.Application.Filters;

namespace OrganizaDinDin.Controllers
{
    [Authorize(Policy = "UserAccess")]
    public class CofrinhoController(ICofrinhoService cofrinhoService) : Controller
    {
        private readonly ICofrinhoService _cofrinhoService = cofrinhoService;

        public async Task<IActionResult> Index()
        {
            var transacoes = await _cofrinhoService.GetAllTransacoesAsync();
            var saldo = await _cofrinhoService.GetSaldoAsync();
            var usuarios = await _cofrinhoService.GetUsuariosAsync();

            ViewBag.Saldo = saldo;
            ViewBag.Usuarios = usuarios.ToDictionary(u => u.Id!, u => u.Nome);

            return View(transacoes);
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Depositar([FromBody] CofrinhoDepositoDto dto)
        {
            try
            {
                var transacao = await _cofrinhoService.DepositarAsync(dto);
                return Json(new { success = true, data = transacao });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Resgatar([FromBody] CofrinhoResgateDto dto)
        {
            try
            {
                var transacao = await _cofrinhoService.ResgatarAsync(dto);
                return Json(new { success = true, data = transacao });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
