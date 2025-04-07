using financeiroApi.Code.Business.Debt;
using financeiroApi.Code.Business.Produto;
using financeiroApi.Model.Produto;
using Microsoft.AspNetCore.Mvc;

namespace financeiroApi.Controllers.Produto
{
    [ApiController]
    [Route("[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly ProdutoBLL _bll;

        public ProdutoController(ProdutoBLL bll)
        {
            _bll = bll;
        }

        [HttpGet("get")]
        public IActionResult GetProduto([FromQuery] ProdutoResponse filtro)
        {

            return Ok(_bll.GetProduto(filtro));
        }

        [HttpPost("insert")]
        public IActionResult InsertProduto([FromBody] ProdutoInsert produto)
        {
            _bll.InsertProduto(produto);

            return Ok();
        }

        [HttpPut("update")]
        public IActionResult UpdateProduto([FromBody] ProdutoUpdate produto)
        {
            _bll.UpdateProduto(produto);

            return Ok("sucesso");
        }

        [HttpDelete("delete")]
        public IActionResult DeleteProduto([FromQuery] int produto)
        {
            _bll.DeleteProduto(produto);

            return Ok();
        }
    }
}
