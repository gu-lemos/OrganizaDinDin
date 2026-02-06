using System.ComponentModel.DataAnnotations;

namespace OrganizaDinDin.Application.DTOs
{
    public class ResetSenhaDto
    {
        [Required(ErrorMessage = "O ID é obrigatório")]
        public required string Id { get; set; }

        [Required(ErrorMessage = "A nova senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
        public required string NovaSenha { get; set; }
    }
}
