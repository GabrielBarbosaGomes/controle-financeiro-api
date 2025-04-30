using financeiroApi.Model.Debt;
using financeiroApi.Model.Income;
using System.Text;

namespace financeiroApi.Code.Business.Income;

public class IncomeDALSQL
{
    public string GetAllIncome(IncomeRequest filtro)
    {
        StringBuilder query = new();
        query.AppendFormat(@"
                            SELECT faturamento.Cod_faturamento Id,
                            faturamento.Cod_usuario CodUsuario,
                            faturamento.Origem,
                            faturamento.Valor,
                            faturamento.Data,
                            faturamento.Comentario
                        FROM db_financeiro.faturamento
                        Where 1 = 1");

        //if (filtro.CodDispesaFixa != null)
        //    query.AppendLine("AND fix.Cod_dispesa_fixa = @CodDispesaFixa");

        //if (!string.IsNullOrWhiteSpace(filtro.NomeDispesaFixa))
        //    query.AppendLine("AND fix.Nome LIKE CONCAT('%', @NomeDispesaFixa, '%')");

        //if (filtro.DataDispesaFixa.HasValue)
        //    query.AppendLine("AND fix.Data = @DataDispesaFixa");

        //if (filtro.CodDispesaVariavel != null)
        //    query.AppendLine("AND fix.Cod_dispesa_fixa = @CodDispesaVariavel");

        //if (!string.IsNullOrWhiteSpace(filtro.NomeDispesaVariavel))
        //    query.AppendLine("AND fix.Nome LIKE CONCAT('%', @NomeDispesaVariavel, '%')");

        //if (filtro.DataDispesaVariavel.HasValue)
        //    query.AppendLine("AND fix.Data = @DataDispesaVariavel");

        return query.ToString();
    }

    public string InsertIncome()
    {
        return @"INSERT INTO db_financeiro.faturamento
                    (
                    Cod_usuario
                    ,Origem
                    ,Valor
                    ,Comentario
                    ,Data)
                     VALUES(
                      @CodUsuario
                     ,@Origem
                     ,@Valor
                     ,@Comentario
                     ,@Data)";
    }

    public string UpdateIncome()
    {
        return @"UPDATE db_financeiro.faturamento
                        SET     Origem = @Origem
                                ,Valor = @Valor
                                ,Comentario= @Comentario
                                ,Data = now()
                        WHERE Cod_faturamento = @Id
                        AND Cod_usuario = @CodUsuario";
    }

    public string DeleteIncome()
    {
        return @"DELETE FROM db_financeiro.faturamento
                     WHERE Cod_faturamento = @Id
                     AND Cod_usuario = @codUsuario";

    }
}
