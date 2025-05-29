namespace Motix_v2.Infraestructure.Services
{
    /// <summary>
    /// Mantiene el estado de sesión en memoria (usuario logueado).
    /// </summary>
    public class AuthenticationService
    {
        public string CurrentUserName { get; set; } = string.Empty;
        public bool IsAuthenticated => !string.IsNullOrEmpty(CurrentUserName);
    }
}
