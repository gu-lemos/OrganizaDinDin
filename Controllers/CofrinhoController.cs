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

        private const int PageSize = 12;

        public async Task<IActionResult> Index(List<int>? tipos, string? usuarioId, DateTime? dataInicio, DateTime? dataFim, int pagina = 1)
        {
            pagina = Math.Max(1, pagina);
            var transacoesPaginadas = await _cofrinhoService.GetTransacoesFilteredAsync(tipos, usuarioId, dataInicio, dataFim, pagina, PageSize);
            var saldoAtual = await _cofrinhoService.GetSaldoAsync();
            var usuarios = await _cofrinhoService.GetUsuariosAsync();
            var meta = await _cofrinhoService.GetMetaAsync();

            ViewBag.Saldo = saldoAtual;
            ViewBag.Meta = meta;
            ViewBag.Usuarios = usuarios.ToDictionary(u => u.Id!, u => u.Nome);
            ViewBag.Tipos = tipos ?? new List<int>();
            ViewBag.UsuarioId = usuarioId;
            ViewBag.DataInicio = dataInicio?.ToString("yyyy-MM-dd");
            ViewBag.DataFim = dataFim?.ToString("yyyy-MM-dd");

            return View(transacoesPaginadas);
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

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Editar([FromBody] CofrinhoEditarDto dto)
        {
            try
            {
                var transacao = await _cofrinhoService.EditarAsync(dto);
                return Json(new { success = true, data = transacao });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Excluir([FromBody] CofrinhoExcluirDto dto)
        {
            try
            {
                await _cofrinhoService.ExcluirAsync(dto.Id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> SetMeta([FromBody] CofrinhoMetaDto dto)
        {
            try
            {
                await _cofrinhoService.SetMetaAsync(dto);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
