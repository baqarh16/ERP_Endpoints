using ERP_AuthService.Services.AuthService;
using ERP_Models.DTOs.ERP_AuthService;
using Microsoft.AspNetCore.Mvc;

namespace ERP_AuthService.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(request);
            return result.IsSuccess
                ? Ok(result)
                : Unauthorized(result);
        }
    }
}