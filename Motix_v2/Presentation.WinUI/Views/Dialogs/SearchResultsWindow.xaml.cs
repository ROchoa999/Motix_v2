using Microsoft.UI.Xaml.Controls;
using Motix_v2.Domain.Entities;
using System.Collections.Generic;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Motix_v2.Presentation.WinUI.Views.Dialogs
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchResultsWindow : ContentDialog
    {
        public SearchResultsWindow(IEnumerable<Customer> customers)
        {
            this.InitializeComponent();
            DataGridResults.ItemsSource = customers;
        }

    }
}
