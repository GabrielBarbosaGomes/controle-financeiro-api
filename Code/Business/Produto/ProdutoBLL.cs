using financeiroApi.Model.Produto;
using financeiroApi.Code.Business.Produto;
using financeiroApi.Code.Business.Debt;

namespace financeiroApi.Code.Business.Produto
{
    public class ProdutoBLL
    {
        private readonly ProdutoDAL _dal;

        public ProdutoBLL(ProdutoDAL dal)
        {
            _dal = dal;
        }

        public List<ProdutoResponse> GetProduto(ProdutoResponse filtro)
        {

            return _dal.GetProduto(filtro);
        }

        public void InsertProduto(ProdutoInsert produto)
        {

            _dal.InsertProduto(produto);
        }

        public void UpdateProduto(ProdutoUpdate produto)
        {
            _dal.UpdateProduto(produto);
        }

        public void DeleteProduto(int produto)
        {
            _dal.DeleteProduto(produto);
        }
    }
}
