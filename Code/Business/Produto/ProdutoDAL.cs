using Dapper;
using financeiroApi.Code.Connection;
using financeiroApi.Model.Produto;


namespace financeiroApi.Code.Business.Produto;

public class ProdutoDAL : MySqlAccess
{
    public ProdutoDAL(IConfiguration configuration) : base(configuration)
    {
    }
    public List<ProdutoResponse> GetProduto(ProdutoResponse filtro)
    {
        ProdutoDALSQL dalSQL = new();
        DynamicParameters parameters = new DynamicParameters();

        if(filtro.Id != null)
        {
            parameters.Add("@Id", filtro.Id);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            parameters.Add("@Nome", filtro.Nome);
        }
        
        if (!string.IsNullOrWhiteSpace(filtro.Marca))
        {
            parameters.Add("@Marca", filtro.Marca);
        }
        
        if (filtro.DataCriacao.HasValue)
        {
            parameters.Add("@DataCriacao", filtro.DataCriacao);
        }

        return Db.Query<ProdutoResponse>(dalSQL.GetProduto(filtro), parameters).ToList();
    }

    public void InsertProduto(ProdutoInsert produto)
    {
        ProdutoDALSQL dalSQL = new();

        Db.Execute(dalSQL.InsertProduto(), produto);
    }

    public void UpdateProduto(ProdutoUpdate produto)
    {
        ProdutoDALSQL dalSQL = new();

        Db.Execute(dalSQL.UpdateProduto(), produto);
    }

    public void DeleteProduto(int produto)
    {
        ProdutoDALSQL dalSQL = new();
        Db.Execute(dalSQL.DeleteProduto(), new { Id = produto });
    }
}