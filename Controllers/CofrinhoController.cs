using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizaDinDin.Application.DTOs;
using OrganizaDinDin.Application.Filters;
using OrganizaDinDin.Application.Interfaces;

namespace OrganizaDinDin.Controllers
{
    [Authorize(Policy = "UserAccess")]
    public class CofrinhoController(ICofrinhoService cofrinhoService) : AppController
    {
        private const int TamanhoPagina = 12;

        private readonly ICofrinhoService _cofrinhoService = cofrinhoService;

        public async Task<IActionResult> Index(List<int>? tipos, string? usuarioId, DateTime? dataInicio, DateTime? dataFim, int pagina = 1)
        {
            pagina = Math.Max(1, pagina);

            var transacoesTask = _cofrinhoService.GetTransacoesFilteredAsync(tipos, usuarioId, dataInicio, dataFim, pagina, TamanhoPagina);
            var saldoTask = _cofrinhoService.GetSaldoAsync();
            var usuariosTask = _cofrinhoService.GetUsuariosAsync();
            var metaTask = _cofrinhoService.GetMetaAsync();
            await Task.WhenAll(transacoesTask, saldoTask, usuariosTask, metaTask);

            ViewBag.Saldo = saldoTask.Result;
            ViewBag.Meta = metaTask.Result;
            ViewBag.Usuarios = usuariosTask.Result.ToDictionary(u => u.Id!, u => u.Nome);
            ViewBag.Tipos = tipos ?? [];
            ViewBag.UsuarioId = usuarioId;
            ViewBag.DataInicio = dataInicio?.ToString("yyyy-MM-dd");
            ViewBag.DataFim = dataFim?.ToString("yyyy-MM-dd");

            return View(transacoesTask.Result);
        }

        [HttpPost]
        [ValidateModel]
        public Task<IActionResult> Depositar([FromBody] CofrinhoDepositoDto dto) => ExecutarAsync(() => _cofrinhoService.DepositarAsync(dto));

        [HttpPost]
        [ValidateModel]
        public Task<IActionResult> Resgatar([FromBody] CofrinhoResgateDto dto) => ExecutarAsync(() => _cofrinhoService.ResgatarAsync(dto));

        [HttpPost]
        [ValidateModel]
        public Task<IActionResult> Editar([FromBody] CofrinhoEditarDto dto) => ExecutarAsync(() => _cofrinhoService.EditarAsync(dto));

        [HttpPost]
        [ValidateModel]
        public Task<IActionResult> Excluir([FromBody] CofrinhoExcluirDto dto) => ExecutarAsync(() => _cofrinhoService.ExcluirAsync(dto.Id));

        [HttpPost]
        [ValidateModel]
        public Task<IActionResult> SetMeta([FromBody] CofrinhoMetaDto dto) => ExecutarAsync(() => _cofrinhoService.SetMetaAsync(dto));
    }
}
