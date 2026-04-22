using OrganizaDinDin.Domain.Entities;

namespace OrganizaDinDin.Domain.Interfaces
{
    public interface ICofrinhoRepository
    {
        Task<List<CofrinhoTransacao>> GetAllAsync();
        Task<List<CofrinhoTransacao>> GetFilteredAsync(List<int>? tipos, string? usuarioId, DateTime? dataInicio, DateTime? dataFim);
        Task<CofrinhoTransacao> CreateAsync(CofrinhoTransacao transacao);
        Task<CofrinhoTransacao> UpdateAsync(string id, CofrinhoTransacao transacao);
        Task DeleteAsync(string id);
        Task<long?> GetMetaAsync();
        Task SetMetaAsync(long valor);
    }
}
