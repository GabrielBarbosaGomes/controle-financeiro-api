using ClosedXML.Excel;
using financeiroApi.Code.Business.Import;
using Xunit;

namespace financeiroApi.Tests;

public class ImportBLLTests
{
    [Theory]
    [InlineData("R$ 1,234.56", 1234.56)]
    [InlineData("R$ 263,04", 263.04)]
    [InlineData("R$ 0.00", 0)]
    [InlineData("R$ 730.00", 730)]
    public void TentarParsearValor_deve_interpretar_formatos_validos(string texto, decimal esperado)
    {
        var resultado = ImportBLL.TentarParsearValor(texto);

        Assert.Equal(esperado, resultado);
    }

    [Fact]
    public void TentarParsearValor_deve_interpretar_valor_negativo()
    {
        var resultado = ImportBLL.TentarParsearValor("-R$ 15,000.00");

        Assert.Equal(-15000m, resultado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void TentarParsearValor_deve_retornar_null_para_texto_vazio(string? texto)
    {
        var resultado = ImportBLL.TentarParsearValor(texto!);

        Assert.Null(resultado);
    }

    [Fact]
    public void TentarParsearValor_deve_retornar_null_para_texto_nao_numerico()
    {
        var resultado = ImportBLL.TentarParsearValor("Média: R$ 5.600,00");

        Assert.Null(resultado);
    }

    [Theory]
    [InlineData("janeiro2025", 1, 2025)]
    [InlineData("dezembro2025", 12, 2025)]
    [InlineData("May2022", 5, 2022)]
    [InlineData("December2022", 12, 2022)]
    [InlineData("março2025", 3, 2025)]
    public void ParsearMesAno_deve_interpretar_meses_em_portugues_e_ingles(string texto, int mesEsperado, int anoEsperado)
    {
        var resultado = ImportBLL.ParsearMesAno(texto);

        Assert.NotNull(resultado);
        Assert.Equal(mesEsperado, resultado!.Value.mes);
        Assert.Equal(anoEsperado, resultado!.Value.ano);
    }

    [Theory]
    [InlineData("Categorias")]
    [InlineData("Total:")]
    [InlineData("")]
    [InlineData("2025")]
    public void ParsearMesAno_deve_retornar_null_para_texto_invalido(string texto)
    {
        var resultado = ImportBLL.ParsearMesAno(texto);

        Assert.Null(resultado);
    }

    [Theory]
    [InlineData("Gastos 2024", 2024)]
    [InlineData("Gastos 2026", 2026)]
    public void ExtrairAnoDoNomeDaAba_deve_extrair_ano_quando_presente(string nomeAba, int anoEsperado)
    {
        var resultado = ImportBLL.ExtrairAnoDoNomeDaAba(nomeAba);

        Assert.Equal(anoEsperado, resultado);
    }

    [Fact]
    public void ExtrairAnoDoNomeDaAba_deve_retornar_null_quando_aba_nao_tem_ano()
    {
        var resultado = ImportBLL.ExtrairAnoDoNomeDaAba("Dívidas");

        Assert.Null(resultado);
    }

    [Theory]
    [InlineData("Parc. Apt")]
    [InlineData("Luz")]
    [InlineData("Academia")]
    [InlineData("Das")]
    public void ItensFixos_deve_conter_itens_recorrentes_conhecidos(string item)
    {
        Assert.Contains(item, ImportBLL.ItensFixos);
    }

    [Theory]
    [InlineData("Restaurante")]
    [InlineData("Uber")]
    [InlineData("Eletronicos")]
    public void ItensFixos_nao_deve_conter_itens_esporadicos(string item)
    {
        Assert.DoesNotContain(item, ImportBLL.ItensFixos);
    }

    // Regressão do bug real encontrado: as abas "Dívidas" e "Gastos 2024" guardam o
    // cabeçalho de mês como célula de DATA do Excel (não texto), por causa de um
    // autofill que incrementou mês e dia juntos (ex: 5/1/2022, 6/2/2022, 7/3/2022).
    [Fact]
    public void MapearColunasDeMes_deve_ler_cabecalho_de_data_e_de_texto()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Planilha");

        ws.Cell(1, 1).Value = "Categorias";
        ws.Cell(1, 2).Value = "Gasto";
        ws.Cell(1, 3).Value = new DateTime(2022, 5, 1); // célula de data (bug real)
        ws.Cell(1, 4).Value = new DateTime(2022, 6, 2); // idem, dia incrementado junto
        ws.Cell(1, 5).Value = "julho2022"; // célula de texto

        var colunas = ImportBLL.MapearColunasDeMes(ws, headerRow: 1, anoDaAba: null, limiteMes: new DateTime(2030, 1, 1));

        Assert.Equal(3, colunas.Count);
        Assert.Equal(new DateTime(2022, 5, 1), colunas[0].data);
        Assert.Equal(new DateTime(2022, 6, 1), colunas[1].data); // dia normalizado para 1, só mês/ano importam
        Assert.Equal(new DateTime(2022, 7, 1), colunas[2].data);
    }

    [Fact]
    public void MapearColunasDeMes_deve_usar_ano_da_aba_quando_informado_ignorando_ano_do_cabecalho()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Planilha");

        ws.Cell(1, 1).Value = "Categorias";
        ws.Cell(1, 2).Value = "Gasto";
        ws.Cell(1, 3).Value = new DateTime(2022, 5, 1);

        var colunas = ImportBLL.MapearColunasDeMes(ws, headerRow: 1, anoDaAba: 2024, limiteMes: new DateTime(2030, 1, 1));

        Assert.Single(colunas);
        Assert.Equal(new DateTime(2024, 5, 1), colunas[0].data);
    }

    [Fact]
    public void MapearColunasDeMes_deve_ignorar_meses_futuros_ao_mes_limite()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Planilha");

        ws.Cell(1, 1).Value = "Categorias";
        ws.Cell(1, 2).Value = "Gasto";
        ws.Cell(1, 3).Value = "junho2026";
        ws.Cell(1, 4).Value = "julho2026";
        ws.Cell(1, 5).Value = "agosto2026";

        var colunas = ImportBLL.MapearColunasDeMes(ws, headerRow: 1, anoDaAba: null, limiteMes: new DateTime(2026, 7, 1));

        Assert.Equal(2, colunas.Count);
        Assert.DoesNotContain(colunas, c => c.data == new DateTime(2026, 8, 1));
    }

    [Fact]
    public void EncontrarLinhaCabecalho_deve_encontrar_linha_com_categorias()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Planilha");
        ws.Cell(1, 1).Value = "Planilha de gastos";
        ws.Cell(6, 1).Value = "Categorias";

        var linha = ImportBLL.EncontrarLinhaCabecalho(ws);

        Assert.Equal(6, linha);
    }

    [Fact]
    public void EncontrarLinhaCabecalho_deve_retornar_null_quando_nao_encontrado()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Planilha");
        ws.Cell(1, 1).Value = "Planilha de gastos";

        var linha = ImportBLL.EncontrarLinhaCabecalho(ws);

        Assert.Null(linha);
    }
}
