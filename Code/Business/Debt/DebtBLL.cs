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
            return _dal.GetAllDebts(filtro);
        }
        
        public List<DebtFixedResponse> GetDebtFixed(DebtRequest filtro)
        {
            return _dal.GetDebtFixed(filtro);
        }

        public void InsertDebtfixed(DebtFixedResponse data)
        {

            _dal.InsertDebtfixed(data);
        }
    }
}
