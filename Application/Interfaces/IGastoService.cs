using OrganizaDinDin.Models;

namespace OrganizaDinDin.Application.Interfaces
{
    public interface IGastoService
    {
        Task<List<Gasto>> GetAllGastosAsync();
        Task<Gasto?> GetGastoByIdAsync(string id);
        Task<Gasto> CreateGastoAsync(GastoDto gastoDto);
        Task<Gasto> UpdateGastoAsync(GastoDto gastoDto);
        Task<bool> DeleteGastoAsync(string id);
    }
}
