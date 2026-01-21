namespace noMoreAzerty_back.Interfaces.Services
{
    public interface IAdminAuthorizationService
    {
        Task<bool> IsAdminAuthorizedAsync(HttpContext context);
        Task<bool> IsAdminAuthorizedAsync(Guid userId);
    }
}