using financeiroApi.Code.Business.Produto;
using financeiroApi.Code.Connection;
using financeiroApi.Model.Produto;
using Dapper;
using financeiroApi.Model.Debt;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

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

            if (filtro.CodDispesaFixa != null)
                parameters.Add("@CodDispesaFixa", filtro.CodDispesaFixa);

            if (!string.IsNullOrWhiteSpace(filtro.NomeDispesaFixa))
                parameters.Add("@NomeDispesaFixa", filtro.NomeDispesaFixa);

            if (filtro.DataDispesaFixa.HasValue)
                parameters.Add("@DataDispesaFixa", filtro.DataDispesaFixa);

            if (filtro.CodDispesaVariavel != null)
                parameters.Add("@CodDispesaVariavel", filtro.CodDispesaVariavel);

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

            if (filtro.CodDispesaFixa != null)
                parameters.Add("@CodDispesaFixa", filtro.CodDispesaFixa);

            if (!string.IsNullOrWhiteSpace(filtro.NomeDispesaFixa))
                parameters.Add("@NomeDispesaFixa", filtro.NomeDispesaFixa);

            if (filtro.DataDispesaFixa.HasValue)
                parameters.Add("@DataDispesaFixa", filtro.DataDispesaFixa);

            return Db.Query<DebtFixedResponse>(dalSQL.GetDebtFixed(filtro), parameters).ToList();
        }
        
        public List<DebtVariableResponse> GetDebtVariable(DebtRequest filtro)
        {
            DebtDALSQL dalSQL = new();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@CodUsuario", filtro.CodUsuario);

            if (filtro.CodDispesaVariavel != null)
                parameters.Add("@CodDispesaVariavel", filtro.CodDispesaVariavel);

            if (!string.IsNullOrWhiteSpace(filtro.NomeDispesaVariavel))
                parameters.Add("@NomeDispesaVariavel", filtro.NomeDispesaVariavel);

            if (filtro.DataDispesaVariavel.HasValue)
                parameters.Add("@DataDispesaVariavel", filtro.DataDispesaVariavel);

            return Db.Query<DebtVariableResponse>(dalSQL.GetDebtVariable(filtro), parameters).ToList();
        }

        public void InsertDebtFixed(DebtFixedResponse data)
        {
            DebtDALSQL dalSQL = new();

            Db.Execute(dalSQL.InsertDebtFixed(), data);
        }
        
        public void InsertDebtVariable(DebtVariableResponse data)
        {
            DebtDALSQL dalSQL = new();

            Db.Execute(dalSQL.InsertDebtVariable(), data);
        }
        
        public int UpdateDebtfixed(DebtFixedResponse data)
        {
            DebtDALSQL dalSQL = new();

            return Db.Execute(dalSQL.UpdateDebtfixed(), data);
        }
        public int UpdateDebtVariable(DebtFixedResponse data)
        {
            DebtDALSQL dalSQL = new();

            return Db.Execute(dalSQL.UpdateDebtVariable(), data);
        }
        
        public void DeleteDebtfixed(DeleteDebtRequest data)
        {
            DebtDALSQL dalSQL = new();

            Db.Execute(dalSQL.DeleteDebtfixed(), data);
        }
        
        public void DeleteDebtVariable(DeleteDebtRequest data)
        {
            DebtDALSQL dalSQL = new();

            Db.Execute(dalSQL.DeleteDebtVariable(), data);
        }
        
        public void DeleteDebtAll(DeleteDebtRequest data)
        {
            DebtDALSQL dalSQL = new();

            Db.Open();

            using (var tran = Db.BeginTransaction())
            {
                try
                {
                    Db.Execute(dalSQL.DeleteDebtfixed(), data, transaction: tran);
                    Db.Execute(dalSQL.DeleteDebtVariable(), data, transaction: tran);

                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
    }
}
