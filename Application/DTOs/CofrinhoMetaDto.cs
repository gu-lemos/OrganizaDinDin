using System.ComponentModel.DataAnnotations;

namespace OrganizaDinDin.Application.DTOs
{
    public class CofrinhoMetaDto
    {
        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(1, long.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public required long Valor { get; set; }
    }
}
