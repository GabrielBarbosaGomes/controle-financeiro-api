using financeiroApi.Model.Debt;
using System.Text;

namespace financeiroApi.Code.Business.Debt
{
    public class DebtDALSQL
    {
        public string GetAllDebt(DebtRequest filtro)
        {
            StringBuilder query = new();
            query.AppendFormat(@"SELECT MesAno, SUM(Total) TotalGasto
                                FROM (
                                    SELECT DATE_SUB(fix.Data, INTERVAL (DAY(fix.Data) - 1) DAY) MesAno, fix.Valor Total
                                    FROM db_financeiro.dispesa_fixa fix
                                    WHERE fix.Cod_usuario = @CodUsuario AND fix.Data IS NOT NULL

                                    UNION ALL

                                    SELECT DATE_SUB(var.Data, INTERVAL (DAY(var.Data) - 1) DAY) MesAno, var.Valor Total
                                    FROM db_financeiro.dispesa_variavel var
                                    WHERE var.Cod_usuario = @CodUsuario AND var.Data IS NOT NULL
                                ) gastos
                                GROUP BY MesAno
                                ORDER BY MesAno DESC");

            return query.ToString();
        }

        public string GetDebtFixed(DebtRequest filtro)
        {
            StringBuilder query = new();
            query.AppendFormat(@"SELECT fix.Cod_dispesa_fixa Id
                                    ,fix.Cod_usuario codUsuario
                                    ,fix.Nome
                                    ,fix.Valor
                                    ,fix.Valor_parcela ValorParcela
                                    ,fix.Quantidade_parcelas QuantidadeParcelas 
                                    ,fix.Tempo_indeterminado TempoIndeterminado
                                    ,fix.Finalizado
                                    ,fix.Categoria
                                    ,fix.Comentario
                                    ,fix.`Data`
                                    ,fix.Data_atualizacao DataAtualizacao
                                FROM db_financeiro.dispesa_fixa fix
                                WHERE fix.Cod_usuario = @CodUsuario ");

            if (filtro.CodDispesaFixa != null)
                query.AppendLine("AND fix.Cod_dispesa_fixa = @CodDispesaFixa");

            if (!string.IsNullOrWhiteSpace(filtro.NomeDispesaFixa))
                query.AppendLine("AND fix.Nome LIKE CONCAT('%', @NomeDispesaFixa, '%')");

            if (filtro.DataDispesaFixa.HasValue)
                query.AppendLine("AND fix.Data = @DataDispesaFixa");

            return query.ToString();
        }

        public string GetDebtVariable(DebtRequest filtro)
        {
            StringBuilder query = new();
            query.AppendFormat(@"SELECT var.Cod_dispesa_variavel Id
                                    ,var.Cod_usuario codUsuario
                                    ,var.Nome
                                    ,var.Valor
                                    ,var.Categoria
                                    ,var.Comentario
                                    ,var.Data
                                FROM db_financeiro.dispesa_variavel var
                                WHERE var.Cod_usuario = @CodUsuario ");

            if (filtro.CodDispesaVariavel != null)
                query.AppendLine("AND var.Cod_dispesa_variavel = @CodDispesaVariavel");

            if (!string.IsNullOrWhiteSpace(filtro.NomeDispesaVariavel))
                query.AppendLine("AND var.Nome LIKE CONCAT('%', @NomeDispesaVariavel, '%')");

            if (filtro.DataDispesaVariavel.HasValue)
                query.AppendLine("AND var.Data = @DataDispesaVariavel");

            return query.ToString();
        }

        public string InsertDebtFixed()
        {
            return @"INSERT INTO db_financeiro.dispesa_fixa
                    (
                    Cod_usuario
                    ,Nome
                    ,Valor
                    ,Valor_parcela
                    ,Quantidade_parcelas
                    ,Tempo_indeterminado
                    ,Finalizado
                    ,Categoria
                    ,Comentario
                    ,`Data`
                    ,Data_atualizacao)
                     VALUES(
                     @CodUsuario
                     ,@Nome
                     ,@Valor
                     ,@ValorParcela
                     ,@QuantidadeParcelas
                     ,@TempoIndeterminado
                     ,@Finalizado
                     ,@Categoria
                     ,@Comentario
                     ,@Data
                     ,@DataAtualizacao)";
        }
        public string InsertDebtVariable()
        {
            return @"INSERT INTO db_financeiro.dispesa_variavel
                    (
                    Cod_usuario
                    ,Nome
                    ,Valor
                    ,Categoria
                    ,Comentario
                    ,Data)
                     VALUES(
                      @CodUsuario
                     ,@Nome
                     ,@Valor
                     ,@Categoria
                     ,@Comentario
                     ,@Data)";
        }

        public string UpdateDebtfixed()
        {
            return @"UPDATE db_financeiro.dispesa_fixa
                        SET     Nome = @Nome
                                ,Valor = @Valor
                                ,Valor_parcela = @ValorParcela
                                ,Quantidade_parcelas = @QuantidadeParcelas
                                ,Tempo_indeterminado= @TempoIndeterminado
                                ,Finalizado= @Finalizado
                                ,Categoria= @Categoria
                                ,Comentario= @Comentario
                                ,Data_Atualizacao = now()
                        WHERE Cod_dispesa_fixa = @Id
                        AND Cod_usuario = @CodUsuario";
        }
        public string UpdateDebtVariable()
        {
            return @"UPDATE db_financeiro.dispesa_variavel
                        SET     Nome = @Nome
                                ,Valor = @Valor
                                ,Categoria= @Categoria
                                ,Comentario= @Comentario
                                ,Data = now()
                        WHERE Cod_dispesa_variavel = @Id
                        AND Cod_usuario = @CodUsuario";
        }


        public string DeleteDebtfixed()
        {
            return @"DELETE FROM db_financeiro.dispesa_fixa
                     WHERE Cod_dispesa_fixa = @CodDispesaFixa
                     AND Cod_usuario = @codUsuario";

        }

        public string DeleteDebtVariable()
        {
            return @"DELETE FROM db_financeiro.dispesa_variavel
                     WHERE Cod_dispesa_variavel = @CodDispesaVariavel;
                     AND Cod_usuario = @codUsuario";

        }

    }
}
