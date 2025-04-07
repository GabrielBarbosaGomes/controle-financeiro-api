namespace financeiroApi.Model.Debt
{
    public class DebtResponse
    {
        public int? Id { get; set; }
        public string? Mes { get; set; }
        public decimal? Saldo { get; set; }
        public DateTime? MesDispesa { get; set; }
        public DateTime? DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataDesativacao { get; set; }
    }

    public class DebtInsert
    {
        public string? Mes { get; set; }
        public DateTime? MesDispesa { get; set; }
    }

    public class DebtUpdate : DebtInsert
    {
        public int? Id { get; set; }
    }
}
