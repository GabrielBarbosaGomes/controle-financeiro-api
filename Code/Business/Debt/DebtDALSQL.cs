using financeiroApi.Model.Produto;
using financeiroApi.Model.Debt;
using System.Text;

namespace financeiroApi.Code.Business.Debt
{
    public class DebtDALSQL
    {
        public string GetAllDebt(DebtResponse filtro)
        {
            StringBuilder query = new();
            query.AppendFormat("SELECT * FROM financeiro.dadosmensais ");

            query.AppendLine("WHERE 1 = 1");

            if (filtro.Id != null)
            {
                query.AppendLine("AND Id = @Id");
            }

            if (!string.IsNullOrWhiteSpace(filtro.Mes))
            {
                query.AppendLine("AND Mes = @Mes");
            }

            if (filtro.Saldo.HasValue)
            {
                query.AppendLine("AND Saldo = @Saldo");
            }

            if (filtro.DataCriacao.HasValue)
            {
                query.AppendLine("AND DataCriacao = @DataCriacao");
            }

            if (filtro.MesDispesa.HasValue)
            {
                query.AppendLine("AND MesDispesa = @MesDispesa");
            }

            return query.ToString();
        }

        public string InsertDebt()
        {
            return @"INSERT INTO financeiro.dadosMensais (
					 Mes
					,DataCriacao
					,MesDispesa
					)
				VALUES (
					 @Mes
					,now()
					,@MesDispesa
					)";
        }
    }
}
