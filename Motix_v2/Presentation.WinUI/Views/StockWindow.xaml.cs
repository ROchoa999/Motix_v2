using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Motix_v2.Presentation.WinUI.ViewModels;
using System.Linq;
using System;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Windows.Graphics;
using WinRT.Interop;

namespace Motix_v2.Presentation.WinUI.Views
{
    public sealed partial class StockWindow : Window
    {
        public StockWindow()
        {
            InitializeComponent();
            AppWindow.SetIcon("Assets\\IconoV1.ico");

            ViewModel = App.Host.Services.GetRequiredService<StockViewModel>();
            ViewModel.LoadCommand.Execute(null);

            ViewModel.SaveCommand.ExecuteRequested += OnSaveCommandExecuteRequested;

            var hwnd = WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new SizeInt32(1000, 700));
            if (appWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.IsResizable = false;
                presenter.IsMaximizable = false;
                presenter.IsMinimizable = false;
            }
        }

        public StockViewModel ViewModel { get; }

        private void OnVolverClicked(object sender, RoutedEventArgs e)
            => Close();

        private async void OnGuardarClicked(object sender, RoutedEventArgs e)
        {
            // 1) Prepara el contenido del diálogo
            var resumen = ViewModel.GetModificationSummaries().ToList();
            var itemsControl = new ItemsControl { ItemsSource = resumen };
            var panel = new StackPanel();
            panel.Children.Add(new TextBlock { Text = "Se van a aplicar estos cambios:" });
            panel.Children.Add(itemsControl);

            // 2) Muestra el ContentDialog
            var dialog = new ContentDialog
            {
                Title = "Confirmar cambios de stock",
                Content = panel,
                PrimaryButtonText = "Confirmar",
                CloseButtonText = "Cancelar"
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // 3) Solo al confirmar, guardamos
                await ViewModel.SaveAsync();
            }
        }

        private async void OnSaveCommandExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            // 1) Prepara el resumen igual que en OnGuardarClicked
            var resumen = ViewModel.GetModificationSummaries().ToList();
            var itemsControl = new ItemsControl { ItemsSource = resumen };
            var panel = new StackPanel();
            panel.Children.Add(new TextBlock { Text = "Se van a aplicar estos cambios:" });
            panel.Children.Add(itemsControl);

            // 2) Muestra el dialog
            var dialog = new ContentDialog
            {
                Title = "Confirmar cambios de stock",
                Content = panel,
                PrimaryButtonText = "Confirmar",
                CloseButtonText = "Cancelar"
            };
            dialog.XamlRoot = this.Content.XamlRoot;
            var result = await dialog.ShowAsync();

            // 3) Si confirman, guardamos
            if (result == ContentDialogResult.Primary)
                await ViewModel.SaveAsync();
        }
    }
}
