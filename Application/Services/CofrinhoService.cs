using OrganizaDinDin.Application.DTOs;
using OrganizaDinDin.Application.Interfaces;
using OrganizaDinDin.Domain.Entities;
using OrganizaDinDin.Domain.Enums;
using OrganizaDinDin.Domain.Interfaces;
using OrganizaDinDin.Helpers;

namespace OrganizaDinDin.Application.Services
{
    public class CofrinhoService(ICofrinhoRepository cofrinhoRepository, IUsuarioRepository usuarioRepository) : ICofrinhoService
    {
        private readonly ICofrinhoRepository _cofrinhoRepository = cofrinhoRepository;
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;

        public async Task<PagedResult<CofrinhoTransacao>> GetTransacoesFilteredAsync(List<int>? tipos, string? usuarioId, DateTime? dataInicio, DateTime? dataFim, int pagina, int tamanhoPagina)
        {
            var transacoes = await _cofrinhoRepository.GetFilteredAsync(tipos, usuarioId, dataInicio, dataFim);

            return new PagedResult<CofrinhoTransacao>
            {
                Itens = [.. transacoes.Skip((pagina - 1) * tamanhoPagina).Take(tamanhoPagina)],
                TotalItens = transacoes.Count,
                Pagina = pagina,
                TamanhoPagina = tamanhoPagina,
                ValorTotal = CalcularSaldo(transacoes)
            };
        }

        public async Task<long> GetSaldoAsync() => CalcularSaldo(await _cofrinhoRepository.GetAllAsync());

        public Task<CofrinhoTransacao> DepositarAsync(CofrinhoDepositoDto dto) => _cofrinhoRepository.CreateAsync(new CofrinhoTransacao
        {
            Valor = dto.Valor,
            Data = DataHelper.ParseData(dto.Data),
            UsuarioId = dto.UsuarioId,
            Tipo = ETipoTransacaoCofrinho.Deposito
        });

        public async Task<CofrinhoTransacao> ResgatarAsync(CofrinhoResgateDto dto)
        {
            if (dto.Valor > await GetSaldoAsync())
                throw new InvalidOperationException("Saldo insuficiente no cofrinho");

            return await _cofrinhoRepository.CreateAsync(new CofrinhoTransacao
            {
                Valor = dto.Valor,
                Data = DataHelper.ParseData(dto.Data),
                UsuarioId = dto.UsuarioId,
                Tipo = ETipoTransacaoCofrinho.Resgate,
                Motivo = dto.Motivo
            });
        }

        public Task<CofrinhoTransacao> EditarAsync(CofrinhoEditarDto dto) => _cofrinhoRepository.UpdateAsync(dto.Id, new CofrinhoTransacao
        {
            Id = dto.Id,
            Valor = dto.Valor,
            Data = DataHelper.ParseData(dto.Data),
            UsuarioId = dto.UsuarioId,
            Tipo = dto.Tipo,
            Motivo = dto.Tipo == ETipoTransacaoCofrinho.Resgate ? dto.Motivo : null
        });

        public Task ExcluirAsync(string id) => _cofrinhoRepository.DeleteAsync(id);

        public Task<long?> GetMetaAsync() => _cofrinhoRepository.GetMetaAsync();

        public Task SetMetaAsync(CofrinhoMetaDto dto) => _cofrinhoRepository.SetMetaAsync(dto.Valor);

        public Task<List<Usuario>> GetUsuariosAsync() => _usuarioRepository.GetAllAsync();

        private static long CalcularSaldo(IEnumerable<CofrinhoTransacao> transacoes) =>
            transacoes.Sum(t => t.Tipo == ETipoTransacaoCofrinho.Deposito ? t.Valor : -t.Valor);
    }
}
