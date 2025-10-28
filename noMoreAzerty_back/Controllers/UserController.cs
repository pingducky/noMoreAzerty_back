using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MyApiProject.UseCases.Users;
using noMoreAzerty_back.Models;

namespace MyApiProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly GetOrCreateCurrentUserUseCase _getOrCreateUserUseCase;

        public UsersController(GetOrCreateCurrentUserUseCase getOrCreateUserUseCase)
        {
            _getOrCreateUserUseCase = getOrCreateUserUseCase;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var oid = User.FindFirst("oid")?.Value;

            if (oid == null)
                return Unauthorized("Missing OID claim");

            if (!Guid.TryParse(oid, out var userGuid))
                return BadRequest("Invalid OID format");

            var user = await _getOrCreateUserUseCase.ExecuteAsync(userGuid);

            return Ok(user);
        }
    }
}
