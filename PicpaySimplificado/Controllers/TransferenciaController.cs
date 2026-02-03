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
        private readonly ILogger<TransferenciaController> logger;
        public TransferenciaController(ITransferenciaService transferenciaService, ILogger<TransferenciaController> logger)
        {
            this.transferenciaService = transferenciaService;
            this.logger = logger;
        }
        [HttpPost]
        public async Task<IActionResult> PostTransfer([FromBody] TransferenciaRequest request)
        {
            var result = await transferenciaService.ExecuteAsync(request);

            logger.LogInformation("Transferência realizada: {@Request} - Resultado: {@Result}", request, result);
            if (!result.IsSuccess)
            {
                logger.LogError("Erro ao criar transferência {errorMessage}", result.ErrorMessage);
                return BadRequest(result);
            }
            
            logger.LogInformation("Transferência criada com sucesso: {@Result}", result);
            return Ok(result);
        }
    }
}
