using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using noMoreAzerty_back.UseCases.Users;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace noMoreAzerty_back.Middlewares
{
    public class EnsureUserProvisionedMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<EnsureUserProvisionedMiddleware> _logger;

        public EnsureUserProvisionedMiddleware(RequestDelegate next, ILogger<EnsureUserProvisionedMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, GetOrCreateCurrentUserUseCase getOrCreateUser)
        {
            // 1. Vérifie l'authentification
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: token manquant ou invalide.");
                return;
            }

            // 2. Récupère le GUID Entra ID
            var oidClaim = context.User.FindFirst("oid")?.Value
                ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(oidClaim) || !Guid.TryParse(oidClaim, out var userGuid))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: identifiant utilisateur invalide.");
                return;
            }

            try
            {
                // 3. Vérifie ou crée le user
                await getOrCreateUser.ExecuteAsync(userGuid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création/vérification de l'utilisateur {Oid}", oidClaim);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("Erreur interne lors de la vérification de l'utilisateur.");
                return;
            }

            // 4. Passe la main au middleware suivant
            await _next(context);
        }
    }
}
