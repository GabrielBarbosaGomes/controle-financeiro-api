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
            //filtro.CodUsuario = 1;
            return _dal.GetAllDebts(filtro);
        }
        
        public List<DebtFixedResponse> GetDebtFixed(DebtRequest filtro)
        {
            return _dal.GetDebtFixed(filtro);
        }
        
        public List<DebtVariableResponse> GetDebtVariable(DebtRequest filtro)
        {
            filtro.CodUsuario = 1;
            return _dal.GetDebtVariable(filtro);
        }

        public void InsertDebtFixed(DebtFixedResponse data)
        {

            _dal.InsertDebtFixed(data);
        }
        
        public void InsertDebtVariable(DebtVariableResponse data)
        {

            _dal.InsertDebtVariable(data);
        }
        
        public int UpdateDebtfixed(DebtFixedResponse data)
        {
            var result = _dal.UpdateDebtfixed(data);
            return result;
        }

        public int UpdateDebtVariable(DebtFixedResponse data)
        {
            var result = _dal.UpdateDebtVariable(data);
            return result;
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
