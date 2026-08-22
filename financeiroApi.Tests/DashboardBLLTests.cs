using financeiroApi.Code.Business.Dashboard;
using financeiroApi.Model.Dashboard;
using Xunit;

namespace financeiroApi.Tests;

public class DashboardBLLTests
{
    [Theory]
    [InlineData(5000, 3000, 40, "Saudável")] // poupou 40%
    [InlineData(5000, 4500, 10, "Atenção")]  // poupou 10%
    [InlineData(5000, 4000, 20, "Saudável")] // exatamente no limiar de 20%
    [InlineData(5000, 5000, 0, "Atenção")]   // poupou 0%
    [InlineData(5000, 6000, -20, "Crítico")] // gastou mais que ganhou
    public void CalcularSaudeFinanceira_deve_classificar_corretamente(
        decimal receita, decimal despesa, decimal percentualEsperado, string statusEsperado)
    {
        var (percentual, status) = DashboardBLL.CalcularSaudeFinanceira(receita, despesa);

        Assert.Equal(percentualEsperado, percentual);
        Assert.Equal(statusEsperado, status);
    }

    [Fact]
    public void CalcularSaudeFinanceira_deve_retornar_critico_quando_ha_despesa_sem_receita()
    {
        var (percentual, status) = DashboardBLL.CalcularSaudeFinanceira(0, 500);

        Assert.Equal(0, percentual);
        Assert.Equal("Crítico", status);
    }

    [Fact]
    public void CalcularSaudeFinanceira_deve_retornar_sem_dados_quando_nao_ha_receita_nem_despesa()
    {
        var (percentual, status) = DashboardBLL.CalcularSaudeFinanceira(0, 0);

        Assert.Equal(0, percentual);
        Assert.Equal("Sem dados", status);
    }

    [Theory]
    [InlineData("semana", "semana")]
    [InlineData("mes", "mes")]
    [InlineData("ano", "ano")]
    [InlineData("SEMANA", "semana")]
    [InlineData("Ano", "ano")]
    public void NormalizarPeriodo_deve_aceitar_periodos_validos_case_insensitive(string entrada, string esperado)
    {
        Assert.Equal(esperado, DashboardBLL.NormalizarPeriodo(entrada));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("diario")]
    [InlineData("mensal")] // nome antigo, não é mais válido
    public void NormalizarPeriodo_deve_usar_mes_como_padrao_para_valores_invalidos(string? entrada)
    {
        Assert.Equal("mes", DashboardBLL.NormalizarPeriodo(entrada));
    }

    private static List<SaldoPeriodo> Meses(params (int ano, int mes)[] pontos) =>
        pontos.Select(p => new SaldoPeriodo { Periodo = new DateTime(p.ano, p.mes, 1), Receita = 100, Despesa = 50 }).ToList();

    private static List<SaldoPeriodo> Semanas(params DateTime[] datas) =>
        datas.Select(d => new SaldoPeriodo { Periodo = d, Receita = 100, Despesa = 50 }).ToList();

    [Fact]
    public void AplicarJanela_mes_sem_ano_deve_retornar_so_os_ultimos_12_pontos()
    {
        var serie = Meses(
            (2024, 1), (2024, 2), (2024, 3), (2024, 4), (2024, 5), (2024, 6),
            (2024, 7), (2024, 8), (2024, 9), (2024, 10), (2024, 11), (2024, 12),
            (2025, 1), (2025, 2)
        );

        var resultado = DashboardBLL.AplicarJanela(serie, "mes", ano: null, mes: null);

        Assert.Equal(12, resultado.Count);
        Assert.Equal(new DateTime(2024, 3, 1), resultado.First().Periodo);
        Assert.Equal(new DateTime(2025, 2, 1), resultado.Last().Periodo);
    }

    [Fact]
    public void AplicarJanela_mes_com_ano_deve_retornar_a_serie_inteira_sem_cortar()
    {
        var serie = Meses((2025, 1), (2025, 2), (2025, 3));

        var resultado = DashboardBLL.AplicarJanela(serie, "mes", ano: 2025, mes: null);

        Assert.Equal(3, resultado.Count);
    }

    [Fact]
    public void AplicarJanela_ano_deve_retornar_a_serie_inteira_sem_cortar()
    {
        var serie = Meses((2022, 1), (2024, 1), (2025, 1), (2026, 1));

        var resultado = DashboardBLL.AplicarJanela(serie, "ano", ano: null, mes: null);

        Assert.Equal(4, resultado.Count);
    }

    [Fact]
    public void AplicarJanela_semana_sem_mes_selecionado_deve_retornar_as_ultimas_4_semanas()
    {
        var serie = Semanas(
            new DateTime(2026, 7, 6), new DateTime(2026, 7, 13), new DateTime(2026, 7, 20),
            new DateTime(2026, 7, 27), new DateTime(2026, 8, 3), new DateTime(2026, 8, 10)
        );

        var resultado = DashboardBLL.AplicarJanela(serie, "semana", ano: null, mes: null);

        Assert.Equal(4, resultado.Count);
        Assert.Equal(new DateTime(2026, 7, 20), resultado.First().Periodo);
        Assert.Equal(new DateTime(2026, 8, 10), resultado.Last().Periodo);
    }

    [Fact]
    public void AplicarJanela_semana_com_mes_selecionado_deve_filtrar_so_aquele_mes()
    {
        var serie = Semanas(
            new DateTime(2026, 6, 29), new DateTime(2026, 7, 6), new DateTime(2026, 7, 13),
            new DateTime(2026, 7, 20), new DateTime(2026, 7, 27), new DateTime(2026, 8, 3)
        );

        var resultado = DashboardBLL.AplicarJanela(serie, "semana", ano: 2026, mes: 7);

        Assert.Equal(4, resultado.Count);
        Assert.All(resultado, s => Assert.Equal(7, s.Periodo.Month));
    }

    [Fact]
    public void AplicarJanela_semana_deve_incluir_semana_que_comeca_no_mes_anterior_mas_tem_dias_no_mes_filtrado()
    {
        // regressão: semana começando 27/07 (termina 02/08) tem que aparecer no filtro de agosto,
        // mesmo com Periodo (início da semana) sendo em julho
        var serie = Semanas(new DateTime(2026, 6, 29), new DateTime(2026, 7, 27));

        var resultado = DashboardBLL.AplicarJanela(serie, "semana", ano: 2026, mes: 8);

        Assert.Single(resultado);
        Assert.Equal(new DateTime(2026, 7, 27), resultado[0].Periodo);
    }

    [Fact]
    public void AplicarJanela_semana_com_mes_de_5_semanas_deve_pegar_so_as_ultimas_4()
    {
        var serie = Semanas(
            new DateTime(2026, 3, 2), new DateTime(2026, 3, 9), new DateTime(2026, 3, 16),
            new DateTime(2026, 3, 23), new DateTime(2026, 3, 30)
        );

        var resultado = DashboardBLL.AplicarJanela(serie, "semana", ano: 2026, mes: 3);

        Assert.Equal(4, resultado.Count);
        Assert.Equal(new DateTime(2026, 3, 9), resultado.First().Periodo);
    }
}
