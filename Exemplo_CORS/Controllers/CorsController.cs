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

        // Esta rota usa a política CORS global definida em Program.cs
        [HttpGet("default")]
        public IActionResult GetDefault()
        {
            _logger.LogInformation("Requisição para rota com política CORS padrão");
            return Ok(new
            {
                message = "Esta rota usa a política CORS global",
                origin = Request.Headers.Origin.ToString(),
                method = Request.Method,
                path = Request.Path.ToString(),
                timestamp = DateTime.Now
            });
        }

        // Esta rota usa uma política CORS específica, sobrescrevendo a global
        [HttpGet("specific")]
        [EnableCors("AllowAll")]
        public IActionResult GetSpecific()
        {
            _logger.LogInformation("Requisição para rota com política CORS específica");
            return Ok(new
            {
                message = "Esta rota usa uma política CORS específica (AllowAll)",
                origin = Request.Headers.Origin.ToString(),
                method = Request.Method,
                path = Request.Path.ToString(),
                timestamp = DateTime.Now
            });
        }

        // Esta rota desativa CORS, mesmo que haja uma política global
        [HttpGet("disabled")]
        [DisableCors]
        public IActionResult GetDisabled()
        {
            _logger.LogInformation("Requisição para rota com CORS desativado");
            return Ok(new
            {
                message = "Esta rota tem CORS desativado explicitamente",
                origin = Request.Headers.Origin.ToString(),
                method = Request.Method,
                path = Request.Path.ToString(),
                timestamp = DateTime.Now
            });
        }

        // Esta rota demonstra o uso de CORS com métodos HTTP específicos
        [HttpPost("methodtest")]
        public IActionResult PostMethodTest([FromBody] TestModel model)
        {
            _logger.LogInformation("Requisição POST para testar CORS com diferentes métodos");
            return Ok(new
            {
                message = "Requisição POST processada com sucesso",
                receivedData = model,
                origin = Request.Headers.Origin.ToString(),
                method = Request.Method,
                path = Request.Path.ToString(),
                timestamp = DateTime.Now
            });
        }

        // Esta rota demonstra o uso de CORS com requisições preflight (OPTIONS)
        [HttpOptions("preflight")]
        public IActionResult Preflight()
        {
            _logger.LogInformation("Requisição OPTIONS preflight recebida");
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
        [EnableCors("DevPolicy")] // Política que permite credentials
        public IActionResult GetWithCredentials()
        {
            _logger.LogInformation("Requisição para rota que testa credentials com CORS");

            // Verifica se há cookies ou se define um para teste
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
