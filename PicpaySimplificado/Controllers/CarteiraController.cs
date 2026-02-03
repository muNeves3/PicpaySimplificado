using Microsoft.AspNetCore.Mvc;
using PicpaySimplificado.Models.Request;
using PicpaySimplificado.Services.Carteiras;

namespace PicpaySimplificado.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarteiraController : ControllerBase
    {
        private readonly ICarteiraService _carteiraService;
        private readonly ILogger<CarteiraController> _logger;

        public CarteiraController(ICarteiraService carteiraService, ILogger<CarteiraController> logger)
        {
            _carteiraService = carteiraService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PostCarteira([FromBody] CarteiraRequest request)
        {
            var result = await _carteiraService.CriarCarteiraAsync(request);
            _logger.LogInformation("Iniciando criação de carteira para o CPF/CNPJ: {CpfCnpj}", request.CPFCNPJ);

            if (!result.IsSuccess)
            {
                _logger.LogError("Erro ao criar carteira para o CPF/CNPJ: {CpfCnpj}. Erro: {ErrorMessage}", request.CPFCNPJ, result.ErrorMessage);
                return BadRequest(new { message = result });
            }

            _logger.LogInformation("Carteira criada com sucesso para o CPF/CNPJ: {CpfCnpj}", request.CPFCNPJ);
            return Created();

        }   
    }
}
