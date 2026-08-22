using financeiroApi.Code.Business.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace financeiroApi.Controllers.Dashboard;

[ApiController]
[Route("Api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly DashboardBLL _bll;

    public DashboardController(DashboardBLL bll)
    {
        _bll = bll;
    }

    [HttpGet("resumo")]
    public IActionResult GetResumo([FromQuery] int codUsuario)
    {
        return Ok(_bll.GetResumo(codUsuario));
    }

    [HttpGet("serie")]
    public IActionResult GetSerie(
        [FromQuery] int codUsuario,
        [FromQuery] string? periodo,
        [FromQuery] int? ano,
        [FromQuery] int? mes)
    {
        return Ok(_bll.GetSerie(codUsuario, periodo, ano, mes));
    }

    [HttpGet("anos-disponiveis")]
    public IActionResult GetAnosDisponiveis([FromQuery] int codUsuario)
    {
        return Ok(_bll.GetAnosDisponiveis(codUsuario));
    }
}
