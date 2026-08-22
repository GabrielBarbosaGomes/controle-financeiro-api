namespace financeiroApi.Model.Debt
{
    public class DebtResponse
    {
        public DateTime MesAno { get; set; }
        public double TotalGasto { get; set; }
    }

    public class DebtFixedResponse
    {
        public int? Id { get; set; }
        public int? CodUsuario { get; set; }
        public string? Nome { get; set; }
        public double? Valor { get; set; }
        public double? ValorParcela { get; set; }
        public int? QuantidadeParcelas { get; set; }
        public bool? TempoIndeterminado { get; set; }
        public bool? Finalizado { get; set; }
        public string? Categoria { get; set; }
        public string? Comentario { get; set; }
        public DateTime? Data { get; set; }
        public DateTime? DataAtualizacao { get; set; }

    }

    public class DebtVariableResponse
    {
        public int? Id { get; set; }
        public int? CodUsuario { get; set; }
        public string? Nome { get; set; }
        public double? Valor { get; set; }
        public string? Categoria { get; set; }
        public string? Comentario { get; set; }
        public DateTime? Data { get; set; }

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
