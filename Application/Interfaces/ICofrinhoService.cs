using OrganizaDinDin.Application.DTOs;
using OrganizaDinDin.Domain.Entities;

namespace OrganizaDinDin.Application.Interfaces
{
    public interface ICofrinhoService
    {
        Task<List<CofrinhoTransacao>> GetAllTransacoesAsync();
        Task<long> GetSaldoAsync();
        Task<CofrinhoTransacao> DepositarAsync(CofrinhoDepositoDto dto);
        Task<CofrinhoTransacao> ResgatarAsync(CofrinhoResgateDto dto);
        Task<List<Usuario>> GetUsuariosAsync();
    }
}
