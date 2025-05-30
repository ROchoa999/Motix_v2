using Motix_v2.Domain.Entities;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Motix_v2.Infraestructure.Services
{
    /// <summary>
    /// Mantiene el estado de sesión en memoria (usuario logueado).
    /// </summary>
    public class AuthenticationService : INotifyPropertyChanged
    {
        private User? _currentUser;
        public User? CurrentUser
        {
            get => _currentUser;
            set
            {
                if (_currentUser != value)
                {
                    _currentUser = value;
                    OnPropertyChanged();
                }
            }
        }
        public string CurrentUserName { get; set; } = string.Empty;
        public bool IsAuthenticated => !string.IsNullOrEmpty(CurrentUserName);

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
