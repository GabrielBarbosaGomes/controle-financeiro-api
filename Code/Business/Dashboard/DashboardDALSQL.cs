using System.Text;

namespace financeiroApi.Code.Business.Dashboard
{
    public class DashboardDALSQL
    {
        public string GetSaldoAtual()
        {
            return @"SELECT
                        (SELECT COALESCE(SUM(Valor), 0) FROM db_financeiro.faturamento WHERE Cod_usuario = @CodUsuario)
                        - (SELECT COALESCE(SUM(Valor), 0) FROM db_financeiro.dispesa_fixa WHERE Cod_usuario = @CodUsuario)
                        - (SELECT COALESCE(SUM(Valor), 0) FROM db_financeiro.dispesa_variavel WHERE Cod_usuario = @CodUsuario)
                    AS SaldoAtual";
        }

        /// <summary>
        /// periodo: "semana", "mes" (padrão) ou "ano" — controla o agrupamento das datas.
        /// Quando @Ano é informado (não nulo), filtra só os lançamentos daquele ano.
        /// </summary>
        public string GetSerie(string periodo)
        {
            var agrupamento = periodo switch
            {
                "ano" => "DATE_SUB(Data, INTERVAL (DAYOFYEAR(Data) - 1) DAY)",
                "semana" => "DATE_SUB(Data, INTERVAL WEEKDAY(Data) DAY)",
                _ => "DATE_SUB(Data, INTERVAL (DAY(Data) - 1) DAY)",
            };

            StringBuilder query = new();
            query.AppendFormat(@"SELECT Periodo,
                                    SUM(CASE WHEN Tipo = 'R' THEN Valor ELSE 0 END) Receita,
                                    SUM(CASE WHEN Tipo = 'D' THEN Valor ELSE 0 END) Despesa
                                FROM (
                                    SELECT {0} Periodo, Valor, 'R' Tipo
                                    FROM db_financeiro.faturamento
                                    WHERE Cod_usuario = @CodUsuario AND Data IS NOT NULL
                                        AND (@Ano IS NULL OR YEAR(Data) = @Ano)

                                    UNION ALL

                                    SELECT {0} Periodo, Valor, 'D' Tipo
                                    FROM db_financeiro.dispesa_fixa
                                    WHERE Cod_usuario = @CodUsuario AND Data IS NOT NULL
                                        AND (@Ano IS NULL OR YEAR(Data) = @Ano)

                                    UNION ALL

                                    SELECT {0} Periodo, Valor, 'D' Tipo
                                    FROM db_financeiro.dispesa_variavel
                                    WHERE Cod_usuario = @CodUsuario AND Data IS NOT NULL
                                        AND (@Ano IS NULL OR YEAR(Data) = @Ano)
                                ) t
                                GROUP BY Periodo
                                ORDER BY Periodo", agrupamento);

            return query.ToString();
        }

        public string GetAnosDisponiveis()
        {
            return @"SELECT DISTINCT YEAR(Data) FROM (
                        SELECT Data FROM db_financeiro.faturamento WHERE Cod_usuario = @CodUsuario AND Data IS NOT NULL
                        UNION ALL
                        SELECT Data FROM db_financeiro.dispesa_fixa WHERE Cod_usuario = @CodUsuario AND Data IS NOT NULL
                        UNION ALL
                        SELECT Data FROM db_financeiro.dispesa_variavel WHERE Cod_usuario = @CodUsuario AND Data IS NOT NULL
                    ) t
                    ORDER BY 1";
        }

        public string GetMaiorCategoriaGasto()
        {
            StringBuilder query = new();
            query.AppendFormat(@"SELECT Categoria, SUM(Valor) Valor
                                FROM (
                                    SELECT Categoria, Valor, Data FROM db_financeiro.dispesa_fixa WHERE Cod_usuario = @CodUsuario

                                    UNION ALL

                                    SELECT Categoria, Valor, Data FROM db_financeiro.dispesa_variavel WHERE Cod_usuario = @CodUsuario
                                ) t
                                WHERE Categoria IS NOT NULL AND Data >= @InicioMes AND Data < @FimMes
                                GROUP BY Categoria
                                ORDER BY Valor DESC
                                LIMIT 1");

            return query.ToString();
        }
    }
}
