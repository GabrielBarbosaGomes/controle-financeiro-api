using System.Globalization;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using financeiroApi.Code.Business.Debt;
using financeiroApi.Model.Debt;
using financeiroApi.Model.Import;

namespace financeiroApi.Code.Business.Import
{
    public class ImportBLL
    {
        private readonly DebtDAL _debtDal;

        internal static readonly string[] AbasSuportadas =
        {
            "Dívidas", "Gastos 2024", "Gastos 2025", "Gastos 2026"
        };

        internal static readonly HashSet<string> LinhasIgnoradas = new(StringComparer.OrdinalIgnoreCase)
        {
            "Planilha de gastos", "Reserva dos sonhos e projetos",
            "Reserva de emergência", "Reserva de emergencia",
            "Salário", "Salario", "Lucro", "Categorias", "Total:", "Total"
        };

        internal static readonly HashSet<string> ItensFixos = new(StringComparer.OrdinalIgnoreCase)
        {
            "Parc. Apt", "Parc. Apt Ex.", "Luz", "Condominio", "Internet fixa e movel",
            "Mercado", "Curso ingles", "Curso RD", "Facudade", "Academia", "Plano de saude", "BJJ", "Das"
        };

        private static readonly Dictionary<string, int> Meses = new(StringComparer.OrdinalIgnoreCase)
        {
            { "janeiro", 1 }, { "january", 1 },
            { "fevereiro", 2 }, { "february", 2 },
            { "março", 3 }, { "marco", 3 }, { "march", 3 },
            { "abril", 4 }, { "april", 4 },
            { "maio", 5 }, { "may", 5 },
            { "junho", 6 }, { "june", 6 },
            { "julho", 7 }, { "july", 7 },
            { "agosto", 8 }, { "august", 8 },
            { "setembro", 9 }, { "september", 9 },
            { "outubro", 10 }, { "october", 10 },
            { "novembro", 11 }, { "november", 11 },
            { "dezembro", 12 }, { "december", 12 },
        };

        public ImportBLL(DebtDAL debtDal)
        {
            _debtDal = debtDal;
        }

        public ImportResponse ImportarPlanilha(Stream arquivo, int codUsuario)
        {
            var resultado = new ImportResponse();
            var hoje = DateTime.Today;
            var limiteMes = new DateTime(hoje.Year, hoje.Month, 1);

            using var workbook = new XLWorkbook(arquivo);

            foreach (var nomeAba in AbasSuportadas)
            {
                if (!workbook.Worksheets.TryGetWorksheet(nomeAba, out var worksheet))
                    continue;

                var anoDaAba = ExtrairAnoDoNomeDaAba(nomeAba);
                var headerRow = EncontrarLinhaCabecalho(worksheet);

                if (headerRow == null)
                {
                    resultado.Erros.Add($"Aba '{nomeAba}': não encontrei a linha de cabeçalho ('Categorias'), aba ignorada.");
                    continue;
                }

                var colunasDeMes = MapearColunasDeMes(worksheet, headerRow.Value, anoDaAba, limiteMes);
                var ultimaLinha = worksheet.LastRowUsed()?.RowNumber() ?? headerRow.Value;

                for (var linha = headerRow.Value + 1; linha <= ultimaLinha; linha++)
                {
                    var categoria = worksheet.Cell(linha, 1).GetString().Trim();
                    var item = worksheet.Cell(linha, 2).GetString().Trim();

                    if (string.IsNullOrWhiteSpace(categoria) || LinhasIgnoradas.Contains(categoria))
                        continue;

                    if (string.IsNullOrWhiteSpace(item))
                        continue;

                    foreach (var (coluna, data) in colunasDeMes)
                    {
                        var textoValor = worksheet.Cell(linha, coluna).GetString();
                        var valor = TentarParsearValor(textoValor);

                        if (valor is null || valor <= 0)
                            continue;

                        try
                        {
                            if (ItensFixos.Contains(item))
                            {
                                _debtDal.InsertDebtFixed(new DebtFixedResponse
                                {
                                    CodUsuario = codUsuario,
                                    Nome = item,
                                    Valor = (double)valor.Value,
                                    ValorParcela = (double)valor.Value,
                                    QuantidadeParcelas = 1,
                                    TempoIndeterminado = true,
                                    Finalizado = false,
                                    Categoria = categoria,
                                    Data = data,
                                });
                                resultado.DespesasFixasInseridas++;
                            }
                            else
                            {
                                _debtDal.InsertDebtVariable(new DebtVariableResponse
                                {
                                    CodUsuario = codUsuario,
                                    Nome = item,
                                    Valor = (double)valor.Value,
                                    Categoria = categoria,
                                    Data = data,
                                });
                                resultado.DespesasVariaveisInseridas++;
                            }
                        }
                        catch (Exception ex)
                        {
                            resultado.Erros.Add($"Aba '{nomeAba}', linha {linha} ({categoria}/{item}, {data:yyyy-MM}): {ex.Message}");
                        }
                    }
                }
            }

            return resultado;
        }

        internal static int? ExtrairAnoDoNomeDaAba(string nomeAba)
        {
            var match = Regex.Match(nomeAba, @"(\d{4})");
            return match.Success ? int.Parse(match.Groups[1].Value) : null;
        }

        internal static int? EncontrarLinhaCabecalho(IXLWorksheet worksheet)
        {
            for (var linha = 1; linha <= 15; linha++)
            {
                if (string.Equals(worksheet.Cell(linha, 1).GetString().Trim(), "Categorias", StringComparison.OrdinalIgnoreCase))
                    return linha;
            }

            return null;
        }

        internal static List<(int coluna, DateTime data)> MapearColunasDeMes(
            IXLWorksheet worksheet, int headerRow, int? anoDaAba, DateTime limiteMes)
        {
            var colunas = new List<(int, DateTime)>();
            var ultimaColuna = worksheet.Row(headerRow).LastCellUsed()?.Address.ColumnNumber ?? 2;

            for (var coluna = 3; coluna <= ultimaColuna; coluna++)
            {
                var celula = worksheet.Cell(headerRow, coluna);
                var mesAno = celula.DataType == XLDataType.DateTime
                    ? (celula.GetDateTime().Month, celula.GetDateTime().Year)
                    : ParsearMesAno(celula.GetString().Trim());

                if (mesAno is null)
                    continue;

                var ano = anoDaAba ?? mesAno.Value.ano;
                var data = new DateTime(ano, mesAno.Value.mes, 1);

                if (data > limiteMes)
                    continue;

                colunas.Add((coluna, data));
            }

            return colunas;
        }

        internal static (int mes, int ano)? ParsearMesAno(string texto)
        {
            var match = Regex.Match(texto, @"^([A-Za-zçÇãÃéÉ]+)\s*(\d{4})$");
            if (!match.Success)
                return null;

            if (!Meses.TryGetValue(match.Groups[1].Value, out var mes))
                return null;

            return (mes, int.Parse(match.Groups[2].Value));
        }

        internal static decimal? TentarParsearValor(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return null;

            var negativo = texto.Contains('-');
            var limpo = texto.Replace("R$", "").Replace("-", "").Trim();

            if (string.IsNullOrEmpty(limpo))
                return null;

            if (limpo.Contains(',') && limpo.Contains('.'))
                limpo = limpo.Replace(",", "");
            else if (limpo.Contains(','))
                limpo = limpo.Replace(",", ".");

            if (!decimal.TryParse(limpo, NumberStyles.Any, CultureInfo.InvariantCulture, out var valor))
                return null;

            return negativo ? -valor : valor;
        }
    }
}
