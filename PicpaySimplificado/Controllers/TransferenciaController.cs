using Microsoft.AspNetCore.Mvc;
using PicpaySimplificado.Models.Request;
using PicpaySimplificado.Services.Transferencias;

namespace PicpaySimplificado.Controllers
{
    [ApiController]
    [Route("transfer")]
    public class TransferenciaController : ControllerBase
    {
        private readonly ITransferenciaService transferenciaService;
        public TransferenciaController(ITransferenciaService transferenciaService)
        {
            this.transferenciaService = transferenciaService;
        }
        [HttpPost]
        public async Task<IActionResult> PostTransfer([FromBody] TransferenciaRequest request)
        {
            var result = await transferenciaService.ExecuteAsync(request);
            if (!result.IsSuccess)
                return BadRequest(result);
            
            return Ok(result);
        }
    }
}
