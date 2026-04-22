using OrganizaDinDin.Application.DTOs;
using OrganizaDinDin.Domain.Entities;

namespace OrganizaDinDin.Application.Interfaces
{
    public interface IGastoService
    {
        Task<List<Gasto>> GetAllGastosAsync();
        Task<PagedResult<Gasto>> GetGastosFilteredAsync(string? descricao, List<int>? tipos, DateTime? dataInicio, DateTime? dataFim, int pagina, int tamanhoPagina);
        Task<Gasto?> GetGastoByIdAsync(string id);
        Task<Gasto> CreateGastoAsync(GastoDto gastoDto);
        Task<Gasto> UpdateGastoAsync(GastoDto gastoDto);
        Task<bool> DeleteGastoAsync(string id);
    }
}
