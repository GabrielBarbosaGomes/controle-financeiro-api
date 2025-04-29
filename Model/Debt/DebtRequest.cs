namespace financeiroApi.Model.Debt
{
    public class DebtRequest
    {
        public int CodUsuario { get; set; }
        public int? CodDispesaFixa { get; set; }
        public string? NomeDispesaFixa { get; set; }
        public DateTime? DataDispesaFixa { get; set; }
        public int? CodDispesaVariavel { get; set; }
        public string? NomeDispesaVariavel { get; set; }
        public DateTime? DataDispesaVariavel { get; set; }
    }

    public class DeleteDebtRequest
    {
        public int CodUsuario { get; set; }
        public int? CodDispesaFixa { get; set; }
        public int? CodDispesaVariavel { get; set; }
        public string? NomeDispesa { get; set;}
    }
}
