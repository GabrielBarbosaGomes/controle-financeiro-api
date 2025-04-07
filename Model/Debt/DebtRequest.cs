namespace financeiroApi.Model.Debt
{
    public class DebtRequest
    {
        public int CodUsuario { get; set; }
        public int? codDispesaFixa { get; set; }
        public string? NomeDispesaFixa { get; set; }
        public DateTime? DataDispesaFixa { get; set; }
        public int? codDispesaVariavel { get; set; }
        public string? NomeDispesaVariavel { get; set; }
        public DateTime? DataDispesaVariavel { get; set; }
    }
}
