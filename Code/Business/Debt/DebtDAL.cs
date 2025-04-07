using financeiroApi.Code.Business.Produto;
using financeiroApi.Code.Connection;
using financeiroApi.Model.Produto;
using Dapper;
using financeiroApi.Model.Debt;
using Microsoft.Extensions.Configuration;

namespace financeiroApi.Code.Business.Debt
{
    public class DebtDAL : MySqlAccess
    {
        public DebtDAL(IConfiguration configuration) : base(configuration)
        {
        }

        public List<DebtResponse> GetAllDebts(DebtRequest filtro)
        {
            DebtDALSQL dalSQL = new();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@CodUsuario", filtro.CodUsuario);

            if (filtro.codDispesaFixa != null)
                parameters.Add("@codDispesaFixa", filtro.codDispesaFixa);

            if (!string.IsNullOrWhiteSpace(filtro.NomeDispesaFixa))
                parameters.Add("@NomeDispesaFixa", filtro.NomeDispesaFixa);

            if (filtro.DataDispesaFixa.HasValue)
                parameters.Add("@DataDispesaFixa", filtro.DataDispesaFixa);

            if (filtro.codDispesaVariavel != null)
                parameters.Add("@codDispesaVariavel", filtro.codDispesaVariavel);

            if (!string.IsNullOrWhiteSpace(filtro.NomeDispesaVariavel))
                parameters.Add("@NomeDispesaVariavel", filtro.NomeDispesaVariavel);

            if (filtro.DataDispesaVariavel.HasValue)
                parameters.Add("@DataDispesaVariavel", filtro.DataDispesaVariavel);

            return Db.Query<DebtResponse>(dalSQL.GetAllDebt(filtro), parameters).ToList();
        }

        public List<DebtFixedResponse> GetDebtFixed(DebtRequest filtro)
        {
            DebtDALSQL dalSQL = new();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@CodUsuario", filtro.CodUsuario);

            if (filtro.codDispesaFixa != null)
                parameters.Add("@codDispesaFixa", filtro.codDispesaFixa);

            if (!string.IsNullOrWhiteSpace(filtro.NomeDispesaFixa))
                parameters.Add("@NomeDispesaFixa", filtro.NomeDispesaFixa);

            if (filtro.DataDispesaFixa.HasValue)
                parameters.Add("@DataDispesaFixa", filtro.DataDispesaFixa);

            return Db.Query<DebtFixedResponse>(dalSQL.GetDebtFixed(filtro), parameters).ToList();
        }

        public void InsertDebtfixed(DebtFixedResponse data)
        {
            DebtDALSQL dalSQL = new();

            Db.Execute(dalSQL.InsertDebtfixed(), data);
        }
    }
}
