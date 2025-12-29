namespace OrganizaDinDin.Application.DTOs
{
    using OrganizaDinDin.Enum;

    public class GastoDto
    {
        public string? Id { get; set; }
        public required string Descricao { get; set; }
        public required long Valor { get; set; }
        public required string Data { get; set; }
        public required ETipoGasto Tipo { get; set; }
    }
}
