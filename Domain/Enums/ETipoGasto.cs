using System.ComponentModel;

namespace OrganizaDinDin.Domain.Enums
{
    public enum ETipoGasto
    {
        [Description("Ato")]
        Entrada = 0,

        [Description("Entrada parcelada")]
        ParcelaEntrada = 1,

        [Description("Evolução de obra")]
        EvolucaoObra = 2,

        [Description("Financiamento")]
        Financiamento = 3,

        [Description("Reforma")]
        Reforma = 4,

        [Description("Documentação")]
        Documentacao = 5,

        [Description("Móveis")]
        Moveis = 6,

        [Description("Eletrodomésticos")]
        Eletrodomesticos = 7,

        [Description("Outros")]
        Outros = 8,

        [Description("Buffet"), AreaGasto(EAreaGasto.Casamento)]
        Buffet = 9,

        [Description("Espaço / Local"), AreaGasto(EAreaGasto.Casamento)]
        Espaco = 10,

        [Description("Serviços adicionais"), AreaGasto(EAreaGasto.Casamento)]
        ServicosAdicionais = 11,

        [Description("Doces finos"), AreaGasto(EAreaGasto.Casamento)]
        DocesFinos = 12,

        [Description("Bolo"), AreaGasto(EAreaGasto.Casamento)]
        Bolo = 13,

        [Description("Bebidas"), AreaGasto(EAreaGasto.Casamento)]
        Bebidas = 14,

        [Description("Decoração e flores"), AreaGasto(EAreaGasto.Casamento)]
        Decoracao = 15,

        [Description("Foto e vídeo"), AreaGasto(EAreaGasto.Casamento)]
        FotoVideo = 16,

        [Description("Música / DJ"), AreaGasto(EAreaGasto.Casamento)]
        Musica = 17,

        [Description("Assessoria / Cerimonial"), AreaGasto(EAreaGasto.Casamento)]
        Assessoria = 18,

        [Description("Vestido e trajes"), AreaGasto(EAreaGasto.Casamento)]
        Trajes = 19,

        [Description("Beleza (cabelo e make)"), AreaGasto(EAreaGasto.Casamento)]
        Beleza = 20,

        [Description("Alianças"), AreaGasto(EAreaGasto.Casamento)]
        Aliancas = 21,

        [Description("Convites e papelaria"), AreaGasto(EAreaGasto.Casamento)]
        Convites = 22,

        [Description("Lembrancinhas"), AreaGasto(EAreaGasto.Casamento)]
        Lembrancinhas = 23,

        [Description("Cerimônia / Igreja / Cartório"), AreaGasto(EAreaGasto.Casamento)]
        Cerimonia = 24,

        [Description("Lua de mel"), AreaGasto(EAreaGasto.Casamento)]
        LuaDeMel = 25,

        [Description("Outros"), AreaGasto(EAreaGasto.Casamento)]
        OutrosCasamento = 26
    }
}
