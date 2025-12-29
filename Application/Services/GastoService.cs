using OrganizaDinDin.Application.Interfaces;
using OrganizaDinDin.Application.DTOs;
using OrganizaDinDin.Domain.Interfaces;
using OrganizaDinDin.Domain.Entities;

namespace OrganizaDinDin.Application.Services
{
    public class GastoService(IGastoRepository gastoRepository) : IGastoService
    {
        private readonly IGastoRepository _gastoRepository = gastoRepository;

        public async Task<List<Gasto>> GetAllGastosAsync()
        {
            return await _gastoRepository.GetAllAsync();
        }

        public async Task<Gasto?> GetGastoByIdAsync(string id)
        {
            return await _gastoRepository.GetByIdAsync(id);
        }

        public async Task<Gasto> CreateGastoAsync(GastoDto gastoDto)
        {
            var gasto = new Gasto
            {
                Descricao = gastoDto.Descricao,
                Valor = gastoDto.Valor,
                Data = DateTime.Parse(gastoDto.Data),
                Tipo = gastoDto.Tipo
            };

            return await _gastoRepository.CreateAsync(gasto);
        }

        public async Task<Gasto> UpdateGastoAsync(GastoDto gastoDto)
        {
            var gasto = new Gasto
            {
                Id = gastoDto.Id,
                Descricao = gastoDto.Descricao,
                Valor = gastoDto.Valor,
                Data = DateTime.Parse(gastoDto.Data),
                Tipo = gastoDto.Tipo
            };

            return await _gastoRepository.UpdateAsync(gasto);
        }

        public async Task<bool> DeleteGastoAsync(string id)
        {
            return await _gastoRepository.DeleteAsync(id);
        }
    }
}
