namespace financeiroApi.Model.Income;

public class IncomeResponse
{
    public int? Id { get; set; }
    public int? CodUsuario {  get; set; }
    public string? Origem {  get; set; }
    public double? Valor { get; set; }
    public DateTime? Data { get; set; }
    public string? Comentario { get; set; }
}
