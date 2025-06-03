using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Motix_v2.Domain.Entities;
using Motix_v2.Infraestructure.UnitOfWork;
using Microsoft.UI.Xaml.Input;
using System;
using System.Linq;
using System.Collections.Generic;


namespace Motix_v2.Presentation.WinUI.ViewModels
{
    public class DocumentViewModel : INotifyPropertyChanged
    {
        private readonly IUnitOfWork _uow;

        private DateTimeOffset? _startDate;
        public DateTimeOffset? StartDate
        {
            get => _startDate;
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTimeOffset? _endDate;
        public DateTimeOffset? EndDate
        {
            get => _endDate;
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnPropertyChanged();
                }
            }
        }


        public DocumentViewModel(IUnitOfWork uow)
        {
            _uow = uow;
            DocumentItems = new ObservableCollection<Document>();
            LoadCommand = new XamlUICommand();
            SearchCommand = new XamlUICommand();

            EstadoRepartoItems = new ObservableCollection<KeyValuePair<EstadoReparto, string>>()
            {
                new KeyValuePair<EstadoReparto, string>(EstadoReparto.Pendiente, "Pendiente"),
                new KeyValuePair<EstadoReparto, string>(EstadoReparto.EnReparto, "En Reparto"),
                new KeyValuePair<EstadoReparto, string>(EstadoReparto.Entregado, "Entregado")
            };

            LoadCommand.ExecuteRequested += async (s, e) => await LoadAsync();
            SearchCommand.ExecuteRequested += async (s, e) => await SearchAsync();
        }

        public ObservableCollection<Document> DocumentItems { get; }

        private string _searchDocumentId = string.Empty;
        public string SearchDocumentId
        {
            get => _searchDocumentId;
            set { _searchDocumentId = value; OnPropertyChanged(); }
        }

        private string _searchClientName = string.Empty;
        public string SearchClientName
        {
            get => _searchClientName;
            set { _searchClientName = value; OnPropertyChanged(); }
        }

        public XamlUICommand LoadCommand { get; }
        public XamlUICommand SearchCommand { get; }

        public async Task LoadAsync()
        {
            var items = await _uow.Documents.GetAllAsync();
            DocumentItems.Clear();
            foreach (var d in items)
                DocumentItems.Add(d);

            UpdateClientNames();
        }

        public async Task SearchAsync()
        {
            DateTime? fromUtc = StartDate.HasValue 
                ? DateTime.SpecifyKind(StartDate.Value.Date, DateTimeKind.Utc)
                : (DateTime?)null;

            DateTime? toUtc = EndDate.HasValue
                ? DateTime.SpecifyKind(EndDate.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc)
                : (DateTime?)null;

            var results = await _uow.Documents.FindAsync(d =>
                (!fromUtc.HasValue || d.Fecha >= fromUtc.Value)
                && (!toUtc.HasValue || d.Fecha <= toUtc.Value)
            );

            DocumentItems.Clear();
            foreach (var d in results)
                DocumentItems.Add(d);

            UpdateClientNames();
        }


        private readonly ObservableCollection<string> _clientNames
            = new ObservableCollection<string>();
        public ObservableCollection<string> ClientNames => _clientNames;

        public ObservableCollection<KeyValuePair<EstadoReparto, string>> EstadoRepartoItems { get; }
        public EstadoReparto? SelectedEstadoReparto { get; set; }

        private string? _selectedClientName;
        public string? SelectedClientName
        {
            get => _selectedClientName;
            set { _selectedClientName = value; OnPropertyChanged();}
        }


        private void UpdateClientNames()
        {
        var nombres = DocumentItems
                    .Select(d => d.Cliente.Nombre)
                    .Distinct()
                    .OrderBy(n => n);
        _clientNames.Clear();
                foreach (var n in nombres)
            _clientNames.Add(n);
        }

        private string _selectedDocumentId = string.Empty;
        public string SelectedDocumentId
        {
            get => _selectedDocumentId;
            private set
            {
                if (_selectedDocumentId != value)
                {
                    _selectedDocumentId = value;
                    OnPropertyChanged();
                }
            }
        }

        public void SelectDocument(string documentId)
        {
            SelectedDocumentId = documentId;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }


}
