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

        [HttpGet("meses/get")]
        public IActionResult GetAllExpenses([FromQuery] DebtResponse filtro)
        {

            return Ok(_bll.GetAllExpenses(filtro));
        }

        [HttpPost("insert")]
        public IActionResult InsertExpenses([FromBody] DebtInsert expense)
        {
            _bll.InsertExpenses(expense);

            return Ok();
        }
    }
}
