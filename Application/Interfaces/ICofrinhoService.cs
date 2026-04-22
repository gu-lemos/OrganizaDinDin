using OrganizaDinDin.Application.DTOs;
using OrganizaDinDin.Domain.Entities;

namespace OrganizaDinDin.Application.Interfaces
{
    public interface ICofrinhoService
    {
        Task<List<CofrinhoTransacao>> GetAllTransacoesAsync();
        Task<PagedResult<CofrinhoTransacao>> GetTransacoesFilteredAsync(List<int>? tipos, string? usuarioId, DateTime? dataInicio, DateTime? dataFim, int pagina, int tamanhoPagina);
        Task<long> GetSaldoAsync();
        Task<CofrinhoTransacao> DepositarAsync(CofrinhoDepositoDto dto);
        Task<CofrinhoTransacao> ResgatarAsync(CofrinhoResgateDto dto);
        Task<CofrinhoTransacao> EditarAsync(CofrinhoEditarDto dto);
        Task ExcluirAsync(string id);
        Task<long?> GetMetaAsync();
        Task SetMetaAsync(CofrinhoMetaDto dto);
        Task<List<Usuario>> GetUsuariosAsync();
    }
}
