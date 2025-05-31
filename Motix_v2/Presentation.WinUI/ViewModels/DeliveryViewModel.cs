using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Input;
using Motix_v2.Domain.Entities;
using Motix_v2.Infraestructure.UnitOfWork;
using System.Collections.Generic;
using Motix_v2.Infraestructure.Repositories;

namespace Motix_v2.Presentation.WinUI.ViewModels
{
    public class DeliveryViewModel : INotifyPropertyChanged
    {
        private readonly IUnitOfWork _uow;

        public DeliveryViewModel(IUnitOfWork uow)
        {
            _uow = uow;
            DeliveryDocuments = new ObservableCollection<Document>();

            LoadCommand = new XamlUICommand();
            StartDeliveryCommand = new XamlUICommand();
            FinishDeliveryCommand = new XamlUICommand();

            LoadCommand.ExecuteRequested += async (s, e) => await LoadAsync();
            StartDeliveryCommand.ExecuteRequested += async (s, e) => await StartDeliveryAsync();
            FinishDeliveryCommand.ExecuteRequested += async (s, e) => await FinishDeliveryAsync();
        }

        public ObservableCollection<Document> DeliveryDocuments { get; }

        public XamlUICommand LoadCommand { get; }
        public XamlUICommand StartDeliveryCommand { get; }
        public XamlUICommand FinishDeliveryCommand { get; }

        private string _deliveryUserName = string.Empty;
        public string DeliveryUserName
        {
            get => _deliveryUserName;
            private set { _deliveryUserName = value; OnPropertyChanged(); }
        }

        private bool _showStart;
        public bool ShowStartDeliveryButton
        {
            get => _showStart;
            private set { _showStart = value; OnPropertyChanged(); }
        }

        private bool _showFinish;
        public bool ShowFinishDeliveryButton
        {
            get => _showFinish;
            private set { _showFinish = value; OnPropertyChanged(); }
        }

        public async Task LoadAsync()
        {
            // 1) Obtener repartidor (rolid = 2)
            var reps = await _uow.Users.FindAsync(u => u.RolId == 2);
            var rep = reps.FirstOrDefault();
            DeliveryUserName = rep?.Nombre ?? string.Empty;

            // 2) Mostrar primero EnReparto o, si no hay, Pendiente
            var enReparto = await _uow.Documents.FindAsync(d =>
                d.EstadoReparto == EstadoReparto.EnReparto.ToString());
            if (enReparto.Any())
            {
                Populate(enReparto);
                ShowFinishDeliveryButton = true;
                ShowStartDeliveryButton = false;
            }
            else
            {
                var pendientes = await _uow.Documents.FindAsync(d =>
                    d.EstadoReparto == EstadoReparto.Pendiente.ToString());
                Populate(pendientes);
                ShowStartDeliveryButton = pendientes.Any();
                ShowFinishDeliveryButton = false;
            }
        }

        private void Populate(IEnumerable<Document> docs)
        {
            DeliveryDocuments.Clear();
            foreach (var d in docs)
                DeliveryDocuments.Add(d);
        }

        private async Task StartDeliveryAsync()
        {
            var repo = (DocumentRepository)_uow.Documents;

            foreach (var doc in DeliveryDocuments)
                repo.UpdateEstadoReparto(doc.Id, EstadoReparto.EnReparto.ToString());

            await _uow.SaveChangesAsync();

            await LoadAsync();
        }

        private async Task FinishDeliveryAsync()
        {
            var repo = (DocumentRepository)_uow.Documents;

            foreach (var doc in DeliveryDocuments)
                repo.UpdateEstadoReparto(doc.Id, EstadoReparto.Entregado.ToString());

            await _uow.SaveChangesAsync();
            await LoadAsync();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
