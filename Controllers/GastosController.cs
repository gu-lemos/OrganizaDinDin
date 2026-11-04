using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizaDinDin.Application.DTOs;
using OrganizaDinDin.Application.Filters;
using OrganizaDinDin.Application.Interfaces;
using OrganizaDinDin.Domain.Enums;

namespace OrganizaDinDin.Controllers
{
    [Authorize(Policy = "UserAccess")]
    public class GastosController(IGastoService gastoService) : AppController
    {
        private const int TamanhoPagina = 12;

        private readonly IGastoService _gastoService = gastoService;

        public Task<IActionResult> Index(string? descricao, List<int>? tipos, DateTime? dataInicio, DateTime? dataFim, string? compromissoId, int pagina = 1)
            => RenderListagem(this, _gastoService, EAreaGasto.Apartamento, descricao, tipos, dataInicio, dataFim, compromissoId, pagina);

        internal static async Task<IActionResult> RenderListagem(Controller controller, IGastoService gastoService, EAreaGasto area,
            string? descricao, List<int>? tipos, DateTime? dataInicio, DateTime? dataFim, string? compromissoId, int pagina)
        {
            pagina = Math.Max(1, pagina);
            var model = await gastoService.GetPaginaAsync(area, descricao, tipos, dataInicio, dataFim, compromissoId, pagina, TamanhoPagina);

            controller.ViewBag.Descricao = descricao;
            controller.ViewBag.Tipos = tipos ?? [];
            controller.ViewBag.DataInicio = dataInicio?.ToString("yyyy-MM-dd");
            controller.ViewBag.DataFim = dataFim?.ToString("yyyy-MM-dd");
            controller.ViewBag.CompromissoId = compromissoId;

            return controller.View("~/Views/Gastos/Index.cshtml", model);
        }

        [HttpPost]
        [ValidateModel]
        public Task<IActionResult> Create([FromBody] GastoDto dto) => ExecutarAsync(() => _gastoService.CreateGastoAsync(dto));

        [HttpPost]
        [ValidateModel]
        public Task<IActionResult> Update([FromBody] GastoDto dto) => ExecutarAsync(() => _gastoService.UpdateGastoAsync(dto));

        [HttpPost]
        public Task<IActionResult> Delete([FromBody] string id) => ExecutarAsync(() => _gastoService.DeleteGastoAsync(id));
    }
}
