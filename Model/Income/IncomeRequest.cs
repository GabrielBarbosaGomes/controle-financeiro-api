namespace financeiroApi.Model.Income;

public class IncomeRequest
{
    public int? Id { get; set; }
    public int CodUsuario { get; set; }
    public string? Origem { get; set; }
}

public class DeleteIncomeRequest
{
    public int Id { get; set; }
    public int CodUsuario { get; set; }
}
