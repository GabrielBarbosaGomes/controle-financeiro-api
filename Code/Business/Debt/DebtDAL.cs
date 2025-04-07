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

        public List<DebtResponse> GetAllExpenses(DebtResponse filtro)
        {
            DebtDALSQL dalSQL = new();
            DynamicParameters parameters = new DynamicParameters();

            if (filtro.Id != null)
            {
                parameters.Add("@Id", filtro.Id);
            }

            if (!string.IsNullOrWhiteSpace(filtro.Mes))
            {
                parameters.Add("@Mes", filtro.Mes);
            }

            if (filtro.Saldo.HasValue)
            {
                parameters.Add("@Saldo", filtro.Saldo);
            }

            if (filtro.DataCriacao.HasValue)
            {
                parameters.Add("@DataCriacao", filtro.DataCriacao);
            }

            if (filtro.MesDispesa.HasValue)
            {
                parameters.Add("@MesDispesa", filtro.MesDispesa);
            }

            return Db.Query<DebtResponse>(dalSQL.GetAllDebt(filtro), parameters).ToList();
        }

        public void InsertExpenses(DebtInsert expense)
        {
            DebtDALSQL dalSQL = new();

            Db.Execute(dalSQL.InsertDebt(), expense);
        }
    }
}
