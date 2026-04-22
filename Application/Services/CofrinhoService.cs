using OrganizaDinDin.Application.DTOs;
using OrganizaDinDin.Application.Interfaces;
using OrganizaDinDin.Domain.Entities;
using OrganizaDinDin.Domain.Enums;
using OrganizaDinDin.Domain.Interfaces;

namespace OrganizaDinDin.Application.Services
{
    public class CofrinhoService(ICofrinhoRepository cofrinhoRepository, IUsuarioRepository usuarioRepository) : ICofrinhoService
    {
        private readonly ICofrinhoRepository _cofrinhoRepository = cofrinhoRepository;
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;

        public async Task<List<CofrinhoTransacao>> GetAllTransacoesAsync()
        {
            return await _cofrinhoRepository.GetAllAsync();
        }

        public async Task<PagedResult<CofrinhoTransacao>> GetTransacoesFilteredAsync(List<int>? tipos, string? usuarioId, DateTime? dataInicio, DateTime? dataFim, int pagina, int tamanhoPagina)
        {
            var transacoes = await _cofrinhoRepository.GetFilteredAsync(tipos, usuarioId, dataInicio, dataFim);

            var totalDepositos = transacoes.Where(t => t.Tipo == ETipoTransacaoCofrinho.Deposito).Sum(t => t.Valor);
            var totalResgates = transacoes.Where(t => t.Tipo == ETipoTransacaoCofrinho.Resgate).Sum(t => t.Valor);

            return new PagedResult<CofrinhoTransacao>
            {
                Itens = transacoes.Skip((pagina - 1) * tamanhoPagina).Take(tamanhoPagina).ToList(),
                TotalItens = transacoes.Count,
                Pagina = pagina,
                TamanhoPagina = tamanhoPagina,
                ValorTotal = totalDepositos - totalResgates
            };
        }

        public async Task<long> GetSaldoAsync()
        {
            var transacoes = await _cofrinhoRepository.GetAllAsync();
            var depositos = transacoes.Where(t => t.Tipo == ETipoTransacaoCofrinho.Deposito).Sum(t => t.Valor);
            var resgates = transacoes.Where(t => t.Tipo == ETipoTransacaoCofrinho.Resgate).Sum(t => t.Valor);
            return depositos - resgates;
        }

        public async Task<CofrinhoTransacao> DepositarAsync(CofrinhoDepositoDto dto)
        {
            var transacao = new CofrinhoTransacao
            {
                Valor = dto.Valor,
                Data = DateTime.Parse(dto.Data),
                UsuarioId = dto.UsuarioId,
                Tipo = ETipoTransacaoCofrinho.Deposito
            };

            return await _cofrinhoRepository.CreateAsync(transacao);
        }

        public async Task<CofrinhoTransacao> ResgatarAsync(CofrinhoResgateDto dto)
        {
            var saldo = await GetSaldoAsync();

            if (dto.Valor > saldo)
                throw new InvalidOperationException("Saldo insuficiente no cofrinho");

            var transacao = new CofrinhoTransacao
            {
                Valor = dto.Valor,
                Data = DateTime.Parse(dto.Data),
                UsuarioId = dto.UsuarioId,
                Tipo = ETipoTransacaoCofrinho.Resgate,
                Motivo = dto.Motivo
            };

            return await _cofrinhoRepository.CreateAsync(transacao);
        }

        public async Task<CofrinhoTransacao> EditarAsync(CofrinhoEditarDto dto)
        {
            var transacao = new CofrinhoTransacao
            {
                Id = dto.Id,
                Valor = dto.Valor,
                Data = DateTime.Parse(dto.Data),
                UsuarioId = dto.UsuarioId,
                Tipo = dto.Tipo,
                Motivo = dto.Motivo
            };

            return await _cofrinhoRepository.UpdateAsync(dto.Id, transacao);
        }

        public async Task ExcluirAsync(string id)
        {
            await _cofrinhoRepository.DeleteAsync(id);
        }

        public async Task<long?> GetMetaAsync()
        {
            return await _cofrinhoRepository.GetMetaAsync();
        }

        public async Task SetMetaAsync(CofrinhoMetaDto dto)
        {
            await _cofrinhoRepository.SetMetaAsync(dto.Valor);
        }

        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            return await _usuarioRepository.GetAllAsync();
        }
    }
}
