using Dapper;
using financeiroApi.Code.Connection;
using financeiroApi.Model.Dashboard;

namespace financeiroApi.Code.Business.Dashboard
{
    public class DashboardDAL : MySqlAccess
    {
        public DashboardDAL(IConfiguration configuration) : base(configuration)
        {
        }

        public decimal GetSaldoAtual(int codUsuario)
        {
            DashboardDALSQL dalSQL = new();
            DynamicParameters parameters = new();
            parameters.Add("@CodUsuario", codUsuario);

            return Db.QuerySingle<decimal>(dalSQL.GetSaldoAtual(), parameters);
        }

        public List<SaldoPeriodo> GetSerie(int codUsuario, string periodo, int? ano)
        {
            DashboardDALSQL dalSQL = new();
            DynamicParameters parameters = new();
            parameters.Add("@CodUsuario", codUsuario);
            parameters.Add("@Ano", ano);

            return Db.Query<SaldoPeriodo>(dalSQL.GetSerie(periodo), parameters).ToList();
        }

        public List<int> GetAnosDisponiveis(int codUsuario)
        {
            DashboardDALSQL dalSQL = new();
            DynamicParameters parameters = new();
            parameters.Add("@CodUsuario", codUsuario);

            return Db.Query<int>(dalSQL.GetAnosDisponiveis(), parameters).ToList();
        }

        public (string? categoria, decimal valor) GetMaiorCategoriaGasto(int codUsuario, DateTime inicioMes, DateTime fimMes)
        {
            DashboardDALSQL dalSQL = new();
            DynamicParameters parameters = new();
            parameters.Add("@CodUsuario", codUsuario);
            parameters.Add("@InicioMes", inicioMes);
            parameters.Add("@FimMes", fimMes);

            var resultado = Db.QueryFirstOrDefault<(string? Categoria, decimal Valor)>(dalSQL.GetMaiorCategoriaGasto(), parameters);

            return resultado;
        }
    }
}
