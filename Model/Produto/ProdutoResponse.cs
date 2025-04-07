namespace financeiroApi.Model.Produto
{
    public class ProdutoResponse
    {
        public int? Id { get; set; }
        public string? Nome { get; set; }
        public string? Marca { get; set; }
        public DateTime? DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataDesativacao { get; set; }
        public string? Descricao { get; set; }
    }

    public class ProdutoInsert
    {
        public string? Nome { get; set; }
        public string? Marca { get; set; }
        public string? Descricao { get; set; }
    }

    public class ProdutoUpdate : ProdutoInsert
    {
        public int? Id { get; set; }
    }
}
