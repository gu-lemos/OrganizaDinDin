using System.ComponentModel.DataAnnotations;
using OrganizaDinDin.Domain.Enums;

namespace OrganizaDinDin.Application.DTOs
{
    public class CofrinhoEditarDto
    {
        [Required]
        public required string Id { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(1, long.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public required long Valor { get; set; }

        [Required(ErrorMessage = "A data é obrigatória")]
        [DataType(DataType.Date, ErrorMessage = "Data inválida")]
        public required string Data { get; set; }

        [Required(ErrorMessage = "O usuário é obrigatório")]
        public required string UsuarioId { get; set; }

        [Required]
        public required ETipoTransacaoCofrinho Tipo { get; set; }

        public string? Motivo { get; set; }
    }
}
