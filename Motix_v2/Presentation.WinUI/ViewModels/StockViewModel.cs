using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Motix_v2.Domain.Entities;
using Motix_v2.Infraestructure.UnitOfWork;
using Microsoft.UI.Xaml.Input;

namespace Motix_v2.Presentation.WinUI.ViewModels
{
    public class StockViewModel
    {
        private readonly IUnitOfWork _uow;

        public StockViewModel(IUnitOfWork uow)
        {
            _uow = uow;
            StockItems = new ObservableCollection<Part>();
            LoadCommand = new XamlUICommand();
            LoadCommand.ExecuteRequested += async (s, e) => await LoadAsync();
        }

        /// <summary>
        /// Colección enlazada al DataGrid.
        /// </summary>
        public ObservableCollection<Part> StockItems { get; }

        /// <summary>
        /// Carga todo el stock.
        /// </summary>
        public async Task LoadAsync()
        {
            var items = await _uow.Parts.GetAllAsync();
            StockItems.Clear();
            foreach (var p in items)
                StockItems.Add(p);
        }

        /// <summary>
        /// Comando para disparar la carga (puedes llamarlo al abrir la ventana).
        /// </summary>
        public XamlUICommand LoadCommand { get; }
    }
}
