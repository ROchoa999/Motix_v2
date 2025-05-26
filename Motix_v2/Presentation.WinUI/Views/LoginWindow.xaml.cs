using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Motix_v2.Infraestructure.Repositories;
using System.Runtime.InteropServices;
using Motix_v2.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Motix_v2.Infraestructure.Security;
using System.Threading.Tasks;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Motix_v2.Presentation.WinUI.Views
{
    public sealed partial class LoginWindow : Window
    {
        private readonly IServiceProvider _services;
        private readonly IRepository<User> _userRepo;


        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_MAXIMIZE = 3;

        public LoginWindow()
        {
            this.InitializeComponent();
            AppWindow.SetIcon("Assets\\IconoV1.ico");

            _services = App.Host.Services;
            _userRepo = _services.GetRequiredService<IRepository<User>>();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string usuario = UsernameTextBox.Text.Trim();
            string contraseña = PasswordBox.Password;

            try
            {
                // 1) Buscar en la BD al usuario por nombre de usuario
                var usuarios = await _userRepo.FindAsync(u => u.Usuario == usuario);
                var usuarioEnt = usuarios.SingleOrDefault();

                // 2) Verificar el hash de la contraseña
                if (usuarioEnt != null
                    && PasswordSecurity.VerifyPassword(contraseña, usuarioEnt.ContrasenaHash))
                {
                    // Login correcto: abrir ventana principal
                    var main = new SalesWindow();
                    main.Activate();

                    // Maximizar la ventana
                    IntPtr hWndMain = WindowNative.GetWindowHandle(main);
                    ShowWindow(hWndMain, SW_MAXIMIZE);

                    // Cerrar la ventana de login
                    this.Close();
                }
                else
                {
                    await MostrarErrorAsync("Usuario y/o contraseña incorrectos");
                }
            }
            catch (Exception ex)
            {
                await MostrarErrorAsync($"Error al iniciar sesión: {ex.Message}");
            }
        }


        private async Task MostrarErrorAsync(string mensaje)
        {
            var dialog = new ContentDialog
            {
                Title = "Error de autenticación",
                Content = mensaje,
                CloseButtonText = "Aceptar",
                XamlRoot = this.Content.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }
}
