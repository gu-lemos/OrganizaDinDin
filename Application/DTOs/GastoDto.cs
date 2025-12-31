using System.ComponentModel.DataAnnotations;
using OrganizaDinDin.Domain.Enums;

namespace OrganizaDinDin.Application.DTOs
{
    public class GastoDto
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "A descrição deve ter entre 5 e 200 caracteres")]
        public required string Descricao { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(1, long.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public required long Valor { get; set; }

        [Required(ErrorMessage = "A data é obrigatória")]
        [DataType(DataType.Date, ErrorMessage = "Data inválida")]
        public required string Data { get; set; }

        [Required(ErrorMessage = "O tipo é obrigatório")]
        [EnumDataType(typeof(ETipoGasto), ErrorMessage = "Tipo de gasto inválido")]
        public required ETipoGasto Tipo { get; set; }
    }
}
