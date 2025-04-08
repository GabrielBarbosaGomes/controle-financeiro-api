using financeiroApi.Code.Business.Debt;
using financeiroApi.Model.Debt;
using Microsoft.AspNetCore.Mvc;

namespace financeiroApi.Controllers.Debt
{
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

        [HttpPost("fixed/insert")]
        public IActionResult InsertDebtfixed([FromBody] DebtFixedResponse data)
        {
            _bll.InsertDebtfixed(data);

            return Ok();
        }
    }
}
