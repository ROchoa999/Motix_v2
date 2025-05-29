using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Input;
using Motix_v2.Domain.Entities;
using Motix_v2.Infraestructure.UnitOfWork;
using static Dapper.SqlMapper;

namespace Motix_v2.Presentation.WinUI.ViewModels
{
    public class StockViewModel : INotifyPropertyChanged
    {
        private readonly IUnitOfWork _uow;
        private readonly Dictionary<int, int> _originalStocks = new();
        private readonly HashSet<StockItemViewModel> _modifiedItems = new();

        public StockViewModel(IUnitOfWork uow)
        {
            _uow = uow;
            StockItems = new ObservableCollection<StockItemViewModel>();

            LoadCommand = new XamlUICommand();
            LoadCommand.ExecuteRequested += async (_, __) => await LoadAsync();

            SaveCommand = new XamlUICommand();
            SaveCommand.CanExecuteRequested += (_, e) => e.CanExecute = HasModifications;
        }

        // Colección de wrappers, enlazada al DataGrid
        public ObservableCollection<StockItemViewModel> StockItems { get; }

        public XamlUICommand LoadCommand { get; }
        public XamlUICommand SaveCommand { get; }

        public bool HasModifications => _modifiedItems.Any();

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string prop)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public async Task LoadAsync()
        {
            var parts = await _uow.Parts.GetAllAsync();
            StockItems.Clear();
            _originalStocks.Clear();
            _modifiedItems.Clear();

            foreach (var p in parts)
            {
                var vm = new StockItemViewModel(p);
                _originalStocks[p.Id] = p.Stock;
                vm.PropertyChanged += OnItemPropertyChanged;
                StockItems.Add(vm);
            }
            OnPropertyChanged(nameof(HasModifications));
        }

        private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StockItemViewModel.Stock) && sender is StockItemViewModel vm)
            {
                // Sólo añadimos una vez cada ítem
                _modifiedItems.Add(vm);
                OnPropertyChanged(nameof(HasModifications));
                SaveCommand.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        /// Genera un resumen legible de las modificaciones: referencia interna y stock original→nuevo.
        /// </summary>
        public IEnumerable<string> GetModificationSummaries()
            => _modifiedItems.Select(vm =>
                $"{vm.ReferenciaInterna}: {_originalStocks[vm.Id]} → {vm.Stock}");

        /// <summary>
        /// Aplica sólo los cambios de stock en _modifiedItems y persiste.
        /// </summary>
        public async Task SaveAsync()
        {
            foreach (var vm in _modifiedItems)
            {
                vm.Entity.Stock = vm.Stock;
                _uow.Parts.Update(vm.Entity);
            }
            await _uow.SaveChangesAsync();
            await LoadAsync();  // recarga todo y limpia el estado de modificaciones
        }
    }
}
