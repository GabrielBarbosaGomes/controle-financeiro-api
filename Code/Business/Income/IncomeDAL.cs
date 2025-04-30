using Dapper;
using financeiroApi.Code.Business.Debt;
using financeiroApi.Code.Connection;
using financeiroApi.Model.Debt;
using financeiroApi.Model.Income;

namespace financeiroApi.Code.Business.Income;

public class IncomeDAL : MySqlAccess
{
    public IncomeDAL(IConfiguration configuration) : base(configuration)
    {
    }
    public List<IncomeResponse> GetAllIncome(IncomeRequest filtro)
    {
        IncomeDALSQL dalSQL = new();
        DynamicParameters parameters = new DynamicParameters();
        //parameters.Add("@CodUsuario", filtro.CodUsuario);

        //if (filtro.CodDispesaFixa != null)
        //    parameters.Add("@CodDispesaFixa", filtro.CodDispesaFixa);

        //if (!string.IsNullOrWhiteSpace(filtro.NomeDispesaFixa))
        //    parameters.Add("@NomeDispesaFixa", filtro.NomeDispesaFixa);

        //if (filtro.DataDispesaFixa.HasValue)
        //    parameters.Add("@DataDispesaFixa", filtro.DataDispesaFixa);

        //if (filtro.CodDispesaVariavel != null)
        //    parameters.Add("@CodDispesaVariavel", filtro.CodDispesaVariavel);

        //if (!string.IsNullOrWhiteSpace(filtro.NomeDispesaVariavel))
        //    parameters.Add("@NomeDispesaVariavel", filtro.NomeDispesaVariavel);

        //if (filtro.DataDispesaVariavel.HasValue)
        //    parameters.Add("@DataDispesaVariavel", filtro.DataDispesaVariavel);

        return Db.Query<IncomeResponse>(dalSQL.GetAllIncome(filtro), parameters).ToList();
    }

    public void InsertIncome(IncomeResponse data)
    {
        IncomeDALSQL dalSQL = new();

        Db.Execute(dalSQL.InsertIncome(), data);
    }

    public int UpdateIncome(IncomeResponse data)
    {
        IncomeDALSQL dalSQL = new();

        return Db.Execute(dalSQL.UpdateIncome(), data);
    }

    public void DeleteIncome(DeleteIncomeRequest data)
    {
        IncomeDALSQL dalSQL = new();

        Db.Execute(dalSQL.DeleteIncome(), data);
    }
}
