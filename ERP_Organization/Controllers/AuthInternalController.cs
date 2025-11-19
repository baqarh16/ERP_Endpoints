using ERP_Models.DTOs.ERP_AuthService;
using ERP_Organization.Services.UserService;
using Microsoft.AspNetCore.Mvc;

namespace ERP_OrganizationService.Controllers
{
    [ApiController]
    [Route("api/organization/auth")]
    public class AuthInternalController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthInternalController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("validate-credentials")]
        public async Task<IActionResult> ValidateCredentials([FromBody] LoginValidationRequest request)
        {
            var result = await _userService.GetUserWithHashByEmailAsync(request.Email);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> SaveRefreshToken([FromBody] SaveRefreshTokenRequest request)
        {
            var result = await _userService.SaveRefreshTokenAsync(request.UserId, request.Token, request.JwtId, request.ExpiryDate);
            return Ok(result);
        }

        [HttpPost("refresh-token/validate")]
        public async Task<IActionResult> ValidateRefreshToken([FromBody] ValidateRefreshTokenRequest request)
        {
            var result = await _userService.ValidateAndUseRefreshTokenAsync(request.Token);
            return Ok(result);
        }
    }
}