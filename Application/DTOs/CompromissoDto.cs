using System.ComponentModel.DataAnnotations;
using System.Globalization;
using OrganizaDinDin.Domain.Enums;

namespace OrganizaDinDin.Application.DTOs
{
    public class CompromissoDto : IValidatableObject
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "A descrição deve ter entre 2 e 100 caracteres")]
        public required string Descricao { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "O valor total não pode ser negativo")]
        public long ValorTotal { get; set; }

        [Range(1, 600, ErrorMessage = "O número de parcelas deve estar entre 1 e 600")]
        public int? TotalParcelas { get; set; }

        public string? DataFim { get; set; }

        [Required(ErrorMessage = "O tipo é obrigatório")]
        [EnumDataType(typeof(ETipoGasto), ErrorMessage = "Tipo inválido")]
        public required ETipoGasto Tipo { get; set; }

        public DateTime? GetDataFim()
        {
            if (string.IsNullOrWhiteSpace(DataFim)) return null;
            return DateTime.TryParseExact(DataFim, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d)
                ? d
                : null;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(DataFim) && GetDataFim() is null)
                yield return new ValidationResult("Data final inválida", [nameof(DataFim)]);

            if (ValorTotal <= 0 && string.IsNullOrWhiteSpace(DataFim))
                yield return new ValidationResult("Informe o valor total ou a data final", [nameof(ValorTotal)]);
        }
    }
}
