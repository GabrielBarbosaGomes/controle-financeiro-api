namespace financeiroApi.Model.Debt
{
    public class DebtResponse
    {
        public int? CodDispesaFixa { get; set; }
        public string? NomeDispesaFixa { get; set; }
        public double? ValorDispesaFixa { get; set; }
        public string? ComentarioDispesaFixa { get; set; }
        public DateTime? DataDispesaFixa { get; set; }
        public int? CodDispesaVariavel {  get; set; }
        public string? NomeDispesaVariavel { get; set; }
        public double? ValorDispesaVariavel { get; set; }
        public string? ComentarioDispesaVariavel { get; set; }
        public DateTime? DataDispesaVariavel { get; set; }
    }

    public class DebtFixedResponse
    {
        public int? Cod_dispesa_fixa { get; set; }
        public int? CodUsuario { get; set; }
        public string? Nome { get; set; }
        public double? Valor { get; set; }
        public double? ValorParcela { get; set; }
        public int? QuantidadeParcelas { get; set; }
        public bool? TempoIndeterminado { get; set; }
        public bool? Finalizado { get; set; }
        public string? Comentario { get; set; }
        public DateTime? Data { get; set; }
        public DateTime? DataAtualizacao { get; set; }

    }

    //public class DebtInsert
    //{
    //    public string? Mes { get; set; }
    //    public DateTime? MesDispesa { get; set; }
    //}

    //public class DebtUpdate : DebtInsert
    //{
    //    public int? Id { get; set; }
    //}
}
