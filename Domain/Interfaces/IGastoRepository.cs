using OrganizaDinDin.Domain.Entities;

namespace OrganizaDinDin.Domain.Interfaces
{
    public interface IGastoRepository
    {
        Task<List<Gasto>> GetAllAsync();
        Task<List<Gasto>> GetFilteredAsync(string? descricao, List<int>? tipos, DateTime? dataInicio, DateTime? dataFim);
        Task<Gasto?> GetByIdAsync(string id);
        Task<Gasto> CreateAsync(Gasto gasto);
        Task<Gasto> UpdateAsync(Gasto gasto);
        Task<bool> DeleteAsync(string id);
    }
}
