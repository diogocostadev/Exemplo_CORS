using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Exemplo_CORS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CorsController : ControllerBase
    {
        private readonly ILogger<CorsController> _logger;

        public CorsController(ILogger<CorsController> logger)
        {
            _logger = logger;
        }

        // Esta rota usa a pol�tica CORS global definida em Program.cs
        [HttpGet("default")]
        public IActionResult GetDefault()
        {
            _logger.LogInformation("Requisi��o para rota com pol�tica CORS padr�o");
            return Ok(new
            {
                message = "Esta rota usa a pol�tica CORS global",
                origin = Request.Headers.Origin.ToString(),
                method = Request.Method,
                path = Request.Path.ToString(),
                timestamp = DateTime.Now
            });
        }

        // Esta rota usa uma pol�tica CORS espec�fica, sobrescrevendo a global
        [HttpGet("specific")]
        [EnableCors("AllowAll")]
        public IActionResult GetSpecific()
        {
            _logger.LogInformation("Requisi��o para rota com pol�tica CORS espec�fica");
            return Ok(new
            {
                message = "Esta rota usa uma pol�tica CORS espec�fica (AllowAll)",
                origin = Request.Headers.Origin.ToString(),
                method = Request.Method,
                path = Request.Path.ToString(),
                timestamp = DateTime.Now
            });
        }

        // Esta rota desativa CORS, mesmo que haja uma pol�tica global
        [HttpGet("disabled")]
        [DisableCors]
        public IActionResult GetDisabled()
        {
            _logger.LogInformation("Requisi��o para rota com CORS desativado");
            return Ok(new
            {
                message = "Esta rota tem CORS desativado explicitamente",
                origin = Request.Headers.Origin.ToString(),
                method = Request.Method,
                path = Request.Path.ToString(),
                timestamp = DateTime.Now
            });
        }

        // Esta rota demonstra o uso de CORS com m�todos HTTP espec�ficos
        [HttpPost("methodtest")]
        public IActionResult PostMethodTest([FromBody] TestModel model)
        {
            _logger.LogInformation("Requisi��o POST para testar CORS com diferentes m�todos");
            return Ok(new
            {
                message = "Requisi��o POST processada com sucesso",
                receivedData = model,
                origin = Request.Headers.Origin.ToString(),
                method = Request.Method,
                path = Request.Path.ToString(),
                timestamp = DateTime.Now
            });
        }

        // Esta rota demonstra o uso de CORS com requisi��es preflight (OPTIONS)
        [HttpOptions("preflight")]
        public IActionResult Preflight()
        {
            _logger.LogInformation("Requisi��o OPTIONS preflight recebida");
            return Ok(new
            {
                message = "Preflight OPTIONS processado com sucesso",
                origin = Request.Headers.Origin.ToString(),
                method = Request.Method,
                path = Request.Path.ToString(),
                corsHeaders = Request.Headers.Where(h => h.Key.StartsWith("Access-Control-"))
                    .ToDictionary(h => h.Key, h => h.Value.ToString()),
                timestamp = DateTime.Now
            });
        }

        // Esta rota testa cookies e credenciais com CORS
        [HttpGet("credentials")]
        [EnableCors("DevPolicy")] // Pol�tica que permite credentials
        public IActionResult GetWithCredentials()
        {
            _logger.LogInformation("Requisi��o para rota que testa credentials com CORS");

            // Verifica se h� cookies ou se define um para teste
            if (!Request.Cookies.ContainsKey("cors-test-cookie"))
            {
                Response.Cookies.Append("cors-test-cookie", "cors-cookie-value", new CookieOptions
                {
                    SameSite = SameSiteMode.None,
                    Secure = true,
                    HttpOnly = true,
                    MaxAge = TimeSpan.FromMinutes(5)
                });
            }

            return Ok(new
            {
                message = "Esta rota testa CORS com credenciais",
                hasCookie = Request.Cookies.ContainsKey("cors-test-cookie"),
                cookieValue = Request.Cookies.TryGetValue("cors-test-cookie", out var cookie) ? cookie : null,
                origin = Request.Headers.Origin.ToString(),
                method = Request.Method,
                path = Request.Path.ToString(),
                timestamp = DateTime.Now
            });
        }
    }

    public class TestModel
    {
        public string? Name { get; set; }
        public string? Message { get; set; }
    }
}
