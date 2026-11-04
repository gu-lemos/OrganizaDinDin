using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizaDinDin.Application.Interfaces;
using OrganizaDinDin.Domain.Enums;

namespace OrganizaDinDin.Controllers
{
    [Authorize(Policy = "UserAccess")]
    public class CasamentoController(IGastoService gastoService) : Controller
    {
        private readonly IGastoService _gastoService = gastoService;

        public Task<IActionResult> Index(string? descricao, List<int>? tipos, DateTime? dataInicio, DateTime? dataFim, string? compromissoId, int pagina = 1)
            => GastosController.RenderListagem(this, _gastoService, EAreaGasto.Casamento, descricao, tipos, dataInicio, dataFim, compromissoId, pagina);
    }
}
