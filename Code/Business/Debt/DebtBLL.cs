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

        public List<DebtResponse> GetAllExpenses(DebtResponse filtro)
        {

            return _dal.GetAllExpenses(filtro);
        }

        public void InsertExpenses(DebtInsert expense)
        {

            _dal.InsertExpenses(expense);
        }
    }
}
