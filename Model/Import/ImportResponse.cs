namespace financeiroApi.Model.Import
{
    public class ImportResponse
    {
        public int DespesasFixasInseridas { get; set; }
        public int DespesasVariaveisInseridas { get; set; }
        public int ReceitasInseridas { get; set; }
        public int LinhasIgnoradas { get; set; }
        public List<string> Erros { get; set; } = new();
    }
}
