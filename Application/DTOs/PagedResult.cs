namespace OrganizaDinDin.Application.DTOs
{
    public class PagedResult<T>
    {
        public required List<T> Itens { get; set; }
        public int TotalItens { get; set; }
        public int Pagina { get; set; }
        public int TamanhoPagina { get; set; }
        public long ValorTotal { get; set; }
        public int TotalPaginas => (int)Math.Ceiling(TotalItens / (double)TamanhoPagina);
        public bool TemPaginaAnterior => Pagina > 1;
        public bool TemProximaPagina => Pagina < TotalPaginas;
    }
}
