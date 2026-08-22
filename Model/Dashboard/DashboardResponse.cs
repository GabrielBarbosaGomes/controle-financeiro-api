namespace financeiroApi.Model.Dashboard
{
    public class DashboardResponse
    {
        public decimal SaldoAtual { get; set; }
        public string? MaiorCategoriaGasto { get; set; }
        public decimal MaiorCategoriaValor { get; set; }
        public decimal SaudeFinanceiraPercentual { get; set; }
        public string SaudeFinanceiraStatus { get; set; } = "";
    }

    public class SaldoPeriodo
    {
        public DateTime Periodo { get; set; }
        public decimal Receita { get; set; }
        public decimal Despesa { get; set; }
        public decimal Saldo => Receita - Despesa;
    }
}
