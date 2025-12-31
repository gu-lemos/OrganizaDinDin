using System.ComponentModel;

namespace OrganizaDinDin.Domain.Enums
{
    public enum ETipoGasto
    {
        [Description("Entrada")]
        Entrada,

        [Description("Parcela da entrada")]
        ParcelaEntrada,

        [Description("Evolução de obra")]
        EvolucaoObra,

        [Description("Financiamento")]
        Financiamento,

        [Description("Reforma")]
        Reforma,

        [Description("Documentação")]
        Documentacao,

        [Description("Móveis")]
        Moveis,

        [Description("Eletrodomésticos")]
        Eletrodomesticos,

        [Description("Outros")]
        Outros
    }
}
