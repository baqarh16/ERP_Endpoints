using ERP_Models.Entities.Common.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ERP_AuthService.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult Response<T>(ApiResponse<T> res)
        {
            if (res.Success) return Ok(res);
            return BadRequest(res);
        }
    }
}
