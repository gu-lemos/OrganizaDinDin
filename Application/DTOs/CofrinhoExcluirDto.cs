using System.ComponentModel.DataAnnotations;

namespace OrganizaDinDin.Application.DTOs
{
    public class CofrinhoExcluirDto
    {
        [Required]
        public required string Id { get; set; }
    }
}
