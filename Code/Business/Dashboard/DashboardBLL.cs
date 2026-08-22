using financeiroApi.Model.Dashboard;

namespace financeiroApi.Code.Business.Dashboard
{
    public class DashboardBLL
    {
        internal static readonly HashSet<string> PeriodosValidos = new(StringComparer.OrdinalIgnoreCase)
        {
            "semana", "mes", "ano"
        };

        private readonly DashboardDAL _dal;

        public DashboardBLL(DashboardDAL dal)
        {
            _dal = dal;
        }

        public DashboardResponse GetResumo(int codUsuario)
        {
            var hoje = DateTime.Today;
            var inicioMesAtual = new DateTime(hoje.Year, hoje.Month, 1);
            var fimMesAtual = inicioMesAtual.AddMonths(1);

            var saldoAtual = _dal.GetSaldoAtual(codUsuario);
            var serieMensal = _dal.GetSerie(codUsuario, "mes", null);
            var (categoria, valorCategoria) = _dal.GetMaiorCategoriaGasto(codUsuario, inicioMesAtual, fimMesAtual);

            var mesAtual = serieMensal.FirstOrDefault(s => s.Periodo == inicioMesAtual);
            var (percentual, status) = CalcularSaudeFinanceira(mesAtual?.Receita ?? 0, mesAtual?.Despesa ?? 0);

            return new DashboardResponse
            {
                SaldoAtual = saldoAtual,
                MaiorCategoriaGasto = categoria,
                MaiorCategoriaValor = valorCategoria,
                SaudeFinanceiraPercentual = percentual,
                SaudeFinanceiraStatus = status,
            };
        }

        /// <summary>
        /// periodo "ano": 1 ponto por ano; se `ano` for informado, restringe àquele ano só.
        /// periodo "mes": 1 ponto por mês; se `ano` for informado, mostra jan-dez daquele ano,
        ///                senão mostra os últimos 12 meses (janela corrente).
        /// periodo "semana": 1 ponto por semana; se `ano`+`mes` forem informados, mostra as
        ///                    últimas 4 semanas daquele mês, senão as últimas 4 semanas mais atuais.
        /// </summary>
        public List<SaldoPeriodo> GetSerie(int codUsuario, string? periodo, int? ano, int? mes)
        {
            var periodoValido = NormalizarPeriodo(periodo);
            // no modo "semana" o filtro de mês é aplicado depois de buscar (precisa da série toda pra pegar as últimas 4)
            var anoParaBusca = periodoValido == "semana" ? null : ano;

            var serieCompleta = _dal.GetSerie(codUsuario, periodoValido, anoParaBusca);

            return AplicarJanela(serieCompleta, periodoValido, ano, mes);
        }

        internal static List<SaldoPeriodo> AplicarJanela(List<SaldoPeriodo> serie, string periodoValido, int? ano, int? mes)
        {
            if (periodoValido == "semana")
            {
                var filtrada = serie;

                if (ano.HasValue && mes.HasValue)
                {
                    var inicioMes = new DateTime(ano.Value, mes.Value, 1);
                    var fimMes = inicioMes.AddMonths(1);
                    // uma semana (7 dias a partir de Periodo) "pertence" ao mês se tiver qualquer
                    // sobreposição com ele — não só se começar dentro do mês (uma semana pode
                    // começar no mês anterior e ainda assim conter dias do mês selecionado)
                    filtrada = serie.Where(s => s.Periodo < fimMes && s.Periodo.AddDays(6) >= inicioMes).ToList();
                }

                return filtrada.TakeLast(4).ToList();
            }

            if (periodoValido == "mes" && !ano.HasValue)
                return serie.TakeLast(12).ToList();

            return serie;
        }

        public List<int> GetAnosDisponiveis(int codUsuario)
        {
            return _dal.GetAnosDisponiveis(codUsuario);
        }

        internal static string NormalizarPeriodo(string? periodo)
        {
            return periodo != null && PeriodosValidos.Contains(periodo)
                ? periodo.ToLowerInvariant()
                : "mes";
        }

        internal static (decimal percentual, string status) CalcularSaudeFinanceira(decimal receitaMes, decimal despesaMes)
        {
            if (receitaMes <= 0)
                return (0, despesaMes > 0 ? "Crítico" : "Sem dados");

            var percentual = Math.Round((receitaMes - despesaMes) / receitaMes * 100, 1);
            var status = percentual switch
            {
                >= 20 => "Saudável",
                >= 0 => "Atenção",
                _ => "Crítico",
            };

            return (percentual, status);
        }
    }
}
