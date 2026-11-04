using OrganizaDinDin.Application.DTOs;
using OrganizaDinDin.Application.Interfaces;
using OrganizaDinDin.Domain.Entities;
using OrganizaDinDin.Domain.Enums;
using OrganizaDinDin.Domain.Interfaces;
using OrganizaDinDin.Helpers;

namespace OrganizaDinDin.Application.Services
{
    public class GastoService(IGastoRepository gastoRepository, ICompromissoRepository compromissoRepository) : IGastoService
    {
        private readonly IGastoRepository _gastoRepository = gastoRepository;
        private readonly ICompromissoRepository _compromissoRepository = compromissoRepository;

        public async Task<GastosPageViewModel> GetPaginaAsync(EAreaGasto area, string? descricao, List<int>? tipos, DateTime? dataInicio, DateTime? dataFim, string? compromissoId, int pagina, int tamanhoPagina)
        {
            var gastosTask = _gastoRepository.GetAllAsync();
            var compromissosTask = _compromissoRepository.GetAllAsync();
            await Task.WhenAll(gastosTask, compromissosTask);

            var gastosDaArea = gastosTask.Result.Where(g => g.Tipo.GetArea() == area).ToList();
            var compromissos = compromissosTask.Result.Where(c => c.Tipo.GetArea() == area).ToList();

            var (resumos, parcelas) = CalcularPagamentos(compromissos, gastosDaArea);
            var filtrados = Filtrar(gastosDaArea, descricao, tipos, dataInicio, dataFim, compromissoId);

            return new GastosPageViewModel
            {
                Area = area,
                Compromissos = resumos,
                Parcelas = parcelas,
                Gastos = new PagedResult<Gasto>
                {
                    Itens = [.. filtrados.Skip((pagina - 1) * tamanhoPagina).Take(tamanhoPagina)],
                    TotalItens = filtrados.Count,
                    Pagina = pagina,
                    TamanhoPagina = tamanhoPagina,
                    ValorTotal = filtrados.Sum(g => g.Valor)
                }
            };
        }

        public async Task<Gasto> CreateGastoAsync(GastoDto dto) => await _gastoRepository.CreateAsync(await ToEntityAsync(dto));

        public async Task<Gasto> UpdateGastoAsync(GastoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Id) || await _gastoRepository.GetByIdAsync(dto.Id) is null)
                throw new InvalidOperationException("Gasto não encontrado");

            return await _gastoRepository.UpdateAsync(await ToEntityAsync(dto));
        }

        public Task DeleteGastoAsync(string id) => _gastoRepository.DeleteAsync(id);

        private static (List<CompromissoResumo>, Dictionary<string, ParcelaInfo>) CalcularPagamentos(List<Compromisso> compromissos, List<Gasto> gastos)
        {
            var resumos = new List<CompromissoResumo>();
            var parcelas = new Dictionary<string, ParcelaInfo>();

            var pagamentosPorConta = gastos
                .Where(g => !string.IsNullOrEmpty(g.CompromissoId))
                .GroupBy(g => g.CompromissoId!)
                .ToDictionary(g => g.Key, g => g.OrderBy(x => x.Data).ThenBy(x => x.Id).ToList());

            foreach (var compromisso in compromissos)
            {
                var pagamentos = pagamentosPorConta.GetValueOrDefault(compromisso.Id!) ?? [];
                var recentes = pagamentos.TakeLast(CompromissoResumo.PagamentosParaMedia).ToList();

                var resumo = new CompromissoResumo
                {
                    Compromisso = compromisso,
                    ValorPago = pagamentos.Sum(p => p.Valor),
                    QuantidadePagamentos = pagamentos.Count,
                    DataUltimoPagamento = pagamentos.Count > 0 ? pagamentos[^1].Data : null,
                    MediaRecente = recentes.Count > 0 ? (long)Math.Round(recentes.Average(p => p.Valor)) : 0
                };
                resumos.Add(resumo);

                for (int i = 0; i < pagamentos.Count; i++)
                {
                    parcelas[pagamentos[i].Id!] = new ParcelaInfo
                    {
                        CompromissoDescricao = compromisso.Descricao,
                        Numero = i + 1,
                        Total = resumo.TotalParcelasEfetivo,
                        TotalEstimado = resumo.ParcelasEstimadas
                    };
                }
            }

            return ([.. resumos.OrderBy(r => r.Quitado).ThenByDescending(r => r.ValorRestante)], parcelas);
        }

        private static List<Gasto> Filtrar(List<Gasto> gastos, string? descricao, List<int>? tipos, DateTime? dataInicio, DateTime? dataFim, string? compromissoId)
        {
            IEnumerable<Gasto> query = gastos;

            if (tipos is { Count: > 0 })
                query = query.Where(g => tipos.Contains((int)g.Tipo));

            if (dataInicio.HasValue)
            {
                var inicio = DataHelper.InicioDoDiaUtc(dataInicio.Value);
                query = query.Where(g => g.Data >= inicio);
            }

            if (dataFim.HasValue)
            {
                var fim = DataHelper.FimDoDiaUtc(dataFim.Value);
                query = query.Where(g => g.Data <= fim);
            }

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(g => g.Descricao.Contains(descricao, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(compromissoId))
                query = query.Where(g => g.CompromissoId == compromissoId);

            return [.. query.OrderByDescending(g => g.Data)];
        }

        private async Task<Gasto> ToEntityAsync(GastoDto dto)
        {
            var compromissoId = string.IsNullOrWhiteSpace(dto.CompromissoId) ? null : dto.CompromissoId;

            if (compromissoId != null)
            {
                var compromisso = await _compromissoRepository.GetByIdAsync(compromissoId)
                    ?? throw new InvalidOperationException("A conta vinculada não foi encontrada");

                if (compromisso.Tipo.GetArea() != dto.Tipo.GetArea())
                    throw new InvalidOperationException("A conta vinculada pertence a outra área");
            }

            return new Gasto
            {
                Id = dto.Id,
                Descricao = dto.Descricao.Trim(),
                Valor = dto.Valor,
                Data = DataHelper.ParseData(dto.Data),
                Tipo = dto.Tipo,
                CompromissoId = compromissoId
            };
        }
    }
}
