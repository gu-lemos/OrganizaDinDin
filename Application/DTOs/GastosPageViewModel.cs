using OrganizaDinDin.Domain.Entities;
using OrganizaDinDin.Domain.Enums;

namespace OrganizaDinDin.Application.DTOs
{
    public class CompromissoResumo
    {
        public const int PagamentosParaMedia = 3;

        public required Compromisso Compromisso { get; set; }
        public long ValorPago { get; set; }
        public int QuantidadePagamentos { get; set; }
        public DateTime? DataUltimoPagamento { get; set; }
        public long MediaRecente { get; set; }

        public bool TemValorTotal => Compromisso.ValorTotal > 0;

        public bool Estimado => !TemValorTotal;

        public int? ParcelasRestantesPorData
        {
            get
            {
                if (Compromisso.DataFim is not DateTime fim) return null;

                var hoje = DateTime.UtcNow;
                var mesAtual = hoje.Year * 12 + hoje.Month;
                var ultimoPago = DataUltimoPagamento is DateTime u ? u.Year * 12 + u.Month : 0;
                var primeiroAPagar = Math.Max(mesAtual, ultimoPago + 1);
                var ultimoAPagar = fim.Year * 12 + fim.Month;

                return Math.Max(0, ultimoAPagar - primeiroAPagar + 1);
            }
        }

        public long ValorRestante => TemValorTotal
            ? Math.Max(0, Compromisso.ValorTotal - ValorPago)
            : (ParcelasRestantesPorData ?? 0) * MediaRecente;

        public long ValorTotalEfetivo => TemValorTotal ? Compromisso.ValorTotal : ValorPago + ValorRestante;

        public bool Quitado => TemValorTotal
            ? ValorPago >= Compromisso.ValorTotal
            : ParcelasRestantesPorData == 0;

        public int? TotalParcelasEfetivo => Compromisso.TotalParcelas
            ?? (ParcelasRestantesPorData is int restantes ? QuantidadePagamentos + restantes : (int?)null)
            ?? (Quitado ? QuantidadePagamentos : (int?)null);

        public bool ParcelasEstimadas => Compromisso.TotalParcelas is null && ParcelasRestantesPorData.HasValue;

        public int Percentual
        {
            get
            {
                if (TemValorTotal)
                    return (int)Math.Min(100, Math.Floor(ValorPago * 100.0 / Compromisso.ValorTotal));

                var total = TotalParcelasEfetivo ?? 0;
                return total <= 0 ? 100 : (int)Math.Min(100, Math.Floor(QuantidadePagamentos * 100.0 / total));
            }
        }
    }

    public class ParcelaInfo
    {
        public required string CompromissoDescricao { get; set; }
        public int Numero { get; set; }
        public int? Total { get; set; }
        public bool TotalEstimado { get; set; }

        public string Rotulo => Total.HasValue
            ? $"{Numero}/{(TotalEstimado ? "~" : "")}{Total}"
            : $"{Numero}ª";
    }

    public class GastosPageViewModel
    {
        public required EAreaGasto Area { get; set; }
        public required PagedResult<Gasto> Gastos { get; set; }
        public required List<CompromissoResumo> Compromissos { get; set; }
        public required Dictionary<string, ParcelaInfo> Parcelas { get; set; }

        public long TotalContratado => Compromissos.Sum(c => c.ValorTotalEfetivo);
        public long TotalPagoContratos => Compromissos.Sum(c => Math.Min(c.ValorPago, c.ValorTotalEfetivo));
        public long TotalRestante => Compromissos.Sum(c => c.ValorRestante);
        public bool TemEstimativa => Compromissos.Any(c => c.Estimado && !c.Quitado);
    }
}
