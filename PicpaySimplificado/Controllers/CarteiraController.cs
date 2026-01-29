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

        public CarteiraController(ICarteiraService carteiraService)
        {
            _carteiraService = carteiraService;
        }

        [HttpPost]
        public async Task<IActionResult> PostCarteira([FromBody] CarteiraRequest request)
        {
            var result = await _carteiraService.CriarCarteiraAsync(request);
            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result });
            }
            return Created();
        }   
    }
}
