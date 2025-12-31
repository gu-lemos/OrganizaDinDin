using OrganizaDinDin.Application.DTOs;
using OrganizaDinDin.Domain.Entities;

namespace OrganizaDinDin.Application.Interfaces
{
    public interface IGastoService
    {
        Task<List<Gasto>> GetAllGastosAsync();
        Task<List<Gasto>> GetGastosFilteredAsync(string? descricao, int? tipo, DateTime? dataInicio, DateTime? dataFim);
        Task<Gasto?> GetGastoByIdAsync(string id);
        Task<Gasto> CreateGastoAsync(GastoDto gastoDto);
        Task<Gasto> UpdateGastoAsync(GastoDto gastoDto);
        Task<bool> DeleteGastoAsync(string id);
    }
}
