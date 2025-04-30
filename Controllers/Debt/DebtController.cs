using financeiroApi.Code.Business.Debt;
using financeiroApi.Model.Debt;
using Microsoft.AspNetCore.Mvc;

namespace financeiroApi.Controllers.Debt;

[ApiController]
[Route("Api/[controller]")]
public class DebtController : ControllerBase
{
    private readonly DebtBLL _bll;

    public DebtController(DebtBLL bll)
    {
        _bll = bll;
    }

    [HttpGet("all/get")]
    public IActionResult GetAllDebts([FromQuery] DebtRequest filtro)
    {

        return Ok(_bll.GetAllDebts(filtro));
    }

    [HttpGet("fixed/get")]
    public IActionResult GetDebtFixed([FromQuery] DebtRequest filtro)
    {

        return Ok(_bll.GetDebtFixed(filtro));
    }

    [HttpGet("variable/get")]
    public IActionResult GetDebtVariable([FromQuery] DebtRequest filtro)
    {

        return Ok(_bll.GetDebtVariable(filtro));
    }

    [HttpPost("fixed/insert")]
    public IActionResult InsertDebtFixed([FromBody] DebtFixedResponse data)
    {
        _bll.InsertDebtFixed(data);

        return Ok(new { message = "Criado com sucesso" });
    }

    [HttpPost("variable/insert")]
    public IActionResult InsertDebtvariable([FromBody] DebtVariableResponse data)
    {
        _bll.InsertDebtVariable(data);

        return Ok(new { message = "Criado com sucesso" });
    }

    [HttpPut("fixed/update")]
    public IActionResult UpdateDebtfixed([FromBody] DebtFixedResponse data)
    {
        _bll.UpdateDebtfixed(data);

        return Ok(new { message = "Atualizado com sucesso" });
    }

    [HttpPut("variable/update")]
    public IActionResult UpdateDebtVariable([FromBody] DebtFixedResponse data)
    {
        _bll.UpdateDebtVariable(data);

        return Ok(new { message = "Atualizado com sucesso" });
    }

    [HttpDelete("delete")]
    public IActionResult DeleteDebt([FromBody] DeleteDebtRequest data)
    {
        _bll.DeleteDebt(data);

        return Ok();
    }
}
