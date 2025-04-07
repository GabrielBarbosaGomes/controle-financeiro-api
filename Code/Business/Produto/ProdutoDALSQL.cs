using financeiroApi.Model.Produto;
using System.Text;

namespace financeiroApi.Code.Business.Produto
{
    public class ProdutoDALSQL
    {
        public string GetProduto(ProdutoResponse filtro)
        {
			StringBuilder query = new();
			query.AppendFormat("SELECT * FROM sys.produto ");

			query.AppendLine("WHERE 1 = 1");

			if(filtro.Id != null)
			{
				query.AppendLine("AND Id = @Id");
			}

            if (!string.IsNullOrWhiteSpace(filtro.Nome))
            {
				query.AppendLine("AND Nome = @Nome");
            }
			
			if (!string.IsNullOrWhiteSpace(filtro.Marca))
            {
				query.AppendLine("AND Marca = @Marca");
            }
			
			if (filtro.DataCriacao.HasValue)
            {
				query.AppendLine("AND DataCriacao = @DataCriacao");
            }

            return query.ToString();
        }

        public string InsertProduto() {
            return @"INSERT INTO sys.produto (
					 Nome
					,Marca
					,DataCriacao
					,Descricao
					)
				VALUES (
					 @Nome
					,@Marca
					,now()
					,@Descricao
					)";
        }

		public string UpdateProduto()
		{
			return @"UPDATE sys.produto
					 SET Nome= @Nome
						,Marca= @Marca
						,DataAtualizacao= now()
						,Descricao= @Descricao
					 WHERE Id= @id";
		}

		public string DeleteProduto()
		{
			return @"DELETE FROM sys.produto
					  WHERE Id= @Id";

        }
    }
}
