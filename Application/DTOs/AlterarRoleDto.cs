using System.ComponentModel.DataAnnotations;

namespace OrganizaDinDin.Application.DTOs
{
    public class AlterarRoleDto
    {
        [Required(ErrorMessage = "O ID é obrigatório")]
        public required string Id { get; set; }

        [Required(ErrorMessage = "A permissão é obrigatória")]
        [RegularExpression("^(Admin|User)$", ErrorMessage = "Permissão inválida")]
        public required string Role { get; set; }
    }
}
