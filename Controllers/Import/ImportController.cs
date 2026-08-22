using financeiroApi.Code.Business.Import;
using Microsoft.AspNetCore.Mvc;

namespace financeiroApi.Controllers.Import;

[ApiController]
[Route("Api/[controller]")]
public class ImportController : ControllerBase
{
    private readonly ImportBLL _bll;

    public ImportController(ImportBLL bll)
    {
        _bll = bll;
    }

    [HttpPost("planilha")]
    public IActionResult ImportarPlanilha(IFormFile arquivo, [FromForm] int codUsuario = 1)
    {
        if (arquivo == null || arquivo.Length == 0)
            return BadRequest(new { message = "Nenhum arquivo enviado." });

        using var stream = arquivo.OpenReadStream();
        var resultado = _bll.ImportarPlanilha(stream, codUsuario);

        return Ok(resultado);
    }
}
