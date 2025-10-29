using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using noMoreAzerty_back.UseCases.Vaults;

namespace noMoreAzerty_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VaultController : ControllerBase
    {
        private readonly GetAllVaultsUseCase _getAllVaultsUseCase;

        public VaultController(GetAllVaultsUseCase getAllVaultsUseCase)
        {
            _getAllVaultsUseCase = getAllVaultsUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vaults = await _getAllVaultsUseCase.ExecuteAsync();
            return Ok(vaults);
        }
    }
}
