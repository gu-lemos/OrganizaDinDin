using OrganizaDinDin.Application.DTOs;
using OrganizaDinDin.Domain.Entities;
using OrganizaDinDin.Domain.Enums;

namespace OrganizaDinDin.Application.Interfaces
{
    public interface IGastoService
    {
        Task<GastosPageViewModel> GetPaginaAsync(EAreaGasto area, string? descricao, List<int>? tipos, DateTime? dataInicio, DateTime? dataFim, string? compromissoId, int pagina, int tamanhoPagina);
        Task<Gasto> CreateGastoAsync(GastoDto dto);
        Task<Gasto> UpdateGastoAsync(GastoDto dto);
        Task DeleteGastoAsync(string id);
    }
}
