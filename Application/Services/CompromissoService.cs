using OrganizaDinDin.Application.DTOs;
using OrganizaDinDin.Application.Interfaces;
using OrganizaDinDin.Domain.Entities;
using OrganizaDinDin.Domain.Interfaces;

namespace OrganizaDinDin.Application.Services
{
    public class CompromissoService(ICompromissoRepository compromissoRepository, IGastoRepository gastoRepository) : ICompromissoService
    {
        private readonly ICompromissoRepository _compromissoRepository = compromissoRepository;
        private readonly IGastoRepository _gastoRepository = gastoRepository;

        public Task<Compromisso> CreateAsync(CompromissoDto dto) => _compromissoRepository.CreateAsync(ToEntity(dto));

        public async Task<Compromisso> UpdateAsync(CompromissoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Id) || await _compromissoRepository.GetByIdAsync(dto.Id) is null)
                throw new InvalidOperationException("Conta não encontrada");

            return await _compromissoRepository.UpdateAsync(ToEntity(dto));
        }

        public async Task DeleteAsync(string id)
        {
            await _gastoRepository.DesvincularCompromissoAsync(id);
            await _compromissoRepository.DeleteAsync(id);
        }

        private static Compromisso ToEntity(CompromissoDto dto) => new()
        {
            Id = dto.Id,
            Descricao = dto.Descricao.Trim(),
            ValorTotal = dto.ValorTotal,
            TotalParcelas = dto.TotalParcelas,
            Tipo = dto.Tipo,
            DataFim = dto.GetDataFim()
        };
    }
}
