using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Motix_v2.Domain.Entities;
using Motix_v2.Infraestructure.UnitOfWork;

namespace Motix_v2.Presentation.WinUI.ViewModels
{
    public class SalesViewModel
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Colección enlazada al DataGrid en la vista.
        /// </summary>
        public ObservableCollection<Customer> Items { get; }
            = new ObservableCollection<Customer>();

        public SalesViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Carga todos los clientes desde la base de datos.
        /// </summary>
        public async Task LoadCustomersAsync(CancellationToken ct = default)
        {
            var customers = await _unitOfWork.Customers.GetAllAsync(ct);
            Items.Clear();
            foreach (var customer in customers)
            {
                Items.Add(customer);
            }
        }
    }
}
