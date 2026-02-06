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

        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            return await _usuarioRepository.GetAllAsync();
        }
    }
}
