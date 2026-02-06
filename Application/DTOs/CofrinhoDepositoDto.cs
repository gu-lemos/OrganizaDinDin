using System.ComponentModel.DataAnnotations;

namespace OrganizaDinDin.Application.DTOs
{
    public class CofrinhoDepositoDto
    {
        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(1, long.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public required long Valor { get; set; }

        [Required(ErrorMessage = "A data é obrigatória")]
        [DataType(DataType.Date, ErrorMessage = "Data inválida")]
        public required string Data { get; set; }

        [Required(ErrorMessage = "O usuário é obrigatório")]
        public required string UsuarioId { get; set; }
    }
}
