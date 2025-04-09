using financeiroApi.Code.Business.Produto;
using financeiroApi.Model.Produto;
using financeiroApi.Model.Debt;

namespace financeiroApi.Code.Business.Debt
{
    public class DebtBLL
    {
        private readonly DebtDAL _dal;

        public DebtBLL(DebtDAL dal)
        {
            _dal = dal;
        }

        public List<DebtResponse> GetAllDebts(DebtRequest filtro)
        {
            filtro.CodUsuario = 1;
            return _dal.GetAllDebts(filtro);
        }
        
        public DebtFixedResponse GetDebtFixed(DebtRequest filtro)
        {
            return _dal.GetDebtFixed(filtro);
        }

        public void InsertDebtfixed(DebtFixedResponse data)
        {

            _dal.InsertDebtfixed(data);
        }
        
        public void UpdateDebtfixed(DebtFixedResponse data)
        {

            _dal.UpdateDebtfixed(data);
        }

        public void DeleteDebt(DeleteDebtRequest data)
        {
            if(data.NomeDispesa == "fixed")
                _dal.DeleteDebtfixed(data);

            if (data.NomeDispesa == "variable")
                _dal.DeleteDebtVariable(data);

            if(data.NomeDispesa == "all")
                _dal.DeleteDebtAll(data);

        }
    }
}
