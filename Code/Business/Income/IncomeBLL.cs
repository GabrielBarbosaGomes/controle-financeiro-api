using financeiroApi.Code.Business.Debt;
using financeiroApi.Model.Debt;
using financeiroApi.Model.Income;

namespace financeiroApi.Code.Business.Income;

public class IncomeBLL
{
    private readonly IncomeDAL _dal;

    public IncomeBLL(IncomeDAL dal)
    {
        _dal = dal;
    }

    public List<IncomeResponse> GetAllIncome(IncomeRequest filtro)
    {
        //filtro.CodUsuario = 1;
        return _dal.GetAllIncome(filtro);
    }
    
    public void InsertIncome(IncomeResponse filtro)
    {
        _dal.InsertIncome(filtro);
    }

    public int UpdateIncome(IncomeResponse data)
    {
        var result = _dal.UpdateIncome(data);
        return result;
    }

    public void DeleteIncome(DeleteIncomeRequest data)
    {
            _dal.DeleteIncome(data);

    }
}
