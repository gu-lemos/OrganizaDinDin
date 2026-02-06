using OrganizaDinDin.Domain.Entities;

namespace OrganizaDinDin.Domain.Interfaces
{
    public interface ICofrinhoRepository
    {
        Task<List<CofrinhoTransacao>> GetAllAsync();
        Task<CofrinhoTransacao> CreateAsync(CofrinhoTransacao transacao);
    }
}
