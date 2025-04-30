using financeiroApi.Code.Business.Income;
using financeiroApi.Model.Debt;
using financeiroApi.Model.Income;
using Microsoft.AspNetCore.Mvc;

namespace financeiroApi.Controllers.Income;

[ApiController]
[Route("Api/[controller]")]
public class IncomeController : ControllerBase
{
    private readonly IncomeBLL _bll;

    public IncomeController(IncomeBLL bll)
    {
        _bll = bll;
    }

    [HttpGet("all/get")]
    public IActionResult GetAllIncome([FromQuery] IncomeRequest filtro)
    {

        return Ok(_bll.GetAllIncome(filtro));
    }

    [HttpPost("insert")]
    public IActionResult InsertIncome([FromBody] IncomeResponse data) 
    {
        _bll.InsertIncome(data);

        return Ok(new { message = "Criado com sucesso" });
    }

    [HttpPut("update")]
    public IActionResult UpdateIncome([FromBody] IncomeResponse data)
    {
        _bll.UpdateIncome(data);

        return Ok(new { message = "Atualizado com sucesso" });
    }

    [HttpDelete("delete")]
    public IActionResult DeleteIncome([FromBody] DeleteIncomeRequest data)
    {
        _bll.DeleteIncome(data);

        return Ok();
    }
}
