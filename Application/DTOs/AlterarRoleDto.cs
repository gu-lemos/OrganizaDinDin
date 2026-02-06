using System.ComponentModel.DataAnnotations;

namespace OrganizaDinDin.Application.DTOs
{
    public class AlterarRoleDto
    {
        [Required(ErrorMessage = "O ID é obrigatório")]
        public required string Id { get; set; }

        [Required(ErrorMessage = "A role é obrigatória")]
        public required string Role { get; set; }
    }
}
