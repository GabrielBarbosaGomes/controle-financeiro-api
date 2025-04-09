using financeiroApi.Model.Produto;
using financeiroApi.Model.Debt;
using System.Text;

namespace financeiroApi.Code.Business.Debt
{
    public class DebtDALSQL
    {
        public string GetAllDebt(DebtRequest filtro)
        {
            StringBuilder query = new();
            query.AppendFormat(@"select 
                                     fix.Cod_dispesa_fixa CodDispesaFixa
	                                ,fix.Nome NomeDispesaFixa
	                                ,fix.Valor ValorDispesaFixa
	                                ,fix.Comentario ComentarioDispesaFixa
	                                ,var.Cod_dispesa_variavel CodDispesaVariavel
	                                ,var.Nome NomeDispesaVariavel
	                                ,var.Valor ValorDispesaVariavel
	                                ,var.Comentario ComentarioDispesaVariavel
                                from db_financeiro.dispesa_fixa fix
                                left join db_financeiro.dispesa_variavel var on var.Cod_usuario = fix.Cod_usuario
                                WHERE fix.Cod_usuario = @CodUsuario ");

            if (filtro.CodDispesaFixa != null)
                query.AppendLine("AND fix.Cod_dispesa_fixa = @CodDispesaFixa");

            if (!string.IsNullOrWhiteSpace(filtro.NomeDispesaFixa))
                query.AppendLine("AND fix.Nome LIKE CONCAT('%', @NomeDispesaFixa, '%')");

            if (filtro.DataDispesaFixa.HasValue)
                query.AppendLine("AND fix.Data = @DataDispesaFixa");

            if (filtro.CodDispesaVariavel != null)
                query.AppendLine("AND fix.Cod_dispesa_fixa = @CodDispesaVariavel");

            if (!string.IsNullOrWhiteSpace(filtro.NomeDispesaVariavel))
                query.AppendLine("AND fix.Nome LIKE CONCAT('%', @NomeDispesaVariavel, '%')");

            if (filtro.DataDispesaVariavel.HasValue)
                query.AppendLine("AND fix.Data = @DataDispesaVariavel");

            return query.ToString();
        }
        
        public string GetDebtFixed(DebtRequest filtro)
        {
            StringBuilder query = new();
            query.AppendFormat(@"SELECT fix.Cod_dispesa_fixa
                                    ,fix.Cod_usuario codUsuario
                                    ,fix.Nome
                                    ,fix.Valor
                                    ,fix.Valor_parcela ValorParcela
                                    ,fix.Quantidade_parcelas QuantidadeParcelas 
                                    ,fix.Tempo_indeterminado TempoIndeterminado
                                    ,fix.Finalizado
                                    ,fix.Comentario
                                    ,fix.`Data`
                                    ,fix.Data_atualizacao DataAtualizacao
                                FROM db_financeiro.dispesa_fixa fix
                                WHERE fix.Cod_usuario = @CodUsuario ");

            if (filtro.CodDispesaFixa != null)
                query.AppendLine("AND fix.Cod_dispesa_fixa = @CodDispesaFixa");

            if (!string.IsNullOrWhiteSpace(filtro.NomeDispesaFixa))
                query.AppendLine("AND fix.Nome = @NomeDispesaFixa");

            if (filtro.DataDispesaFixa.HasValue)
                query.AppendLine("AND fix.Data = @DataDispesaFixa");

            return query.ToString();
        }

        public string InsertDebtfixed()
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
                     ,@Comentario
                     ,@Data
                     ,@DataAtualizacao)";
        }

        public string UpdateDebtfixed()
        {
            return @"UPDATE db_financeiro.dispesa_fixa
                        SET     Nome= @Nome
                                ,Valor = @Valor
                                ,Valor_parcela = @ValorParcela
                                ,Quantidade_parcelas = @QuantidadeParcelas
                                ,Tempo_indeterminado= @TempoIndeterminado
                                ,Finalizado= @Finalizado
                                ,Comentario= @Comentario
                                ,`Data`= @Data
                                ,Data_Atualizacao = now()
                        WHERE Cod_dispesa_fixa = @CodDispesaFixa
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
