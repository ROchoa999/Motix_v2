using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.EntityFrameworkCore;
using Motix_v2.Domain.Entities;
using Motix_v2.Infraestructure.Data;
using Motix_v2.Infraestructure.Repositories;
using Motix_v2.Presentation.WinUI.Views;
using System;
using System.Runtime.InteropServices;
using WinRT.Interop;
using Motix_v2.Infraestructure.UnitOfWork;
using Motix_v2.Presentation.WinUI.ViewModels;

namespace Motix_v2
{

    public partial class App : Application
    {

        public static IHost Host { get; private set; } = null!;

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_MAXIMIZE = 3;

        public App()
        {
            Host = Microsoft.Extensions.Hosting.Host
                .CreateDefaultBuilder()
                .ConfigureAppConfiguration(cfg =>
                {
                    cfg.SetBasePath(AppContext.BaseDirectory)
                       .AddJsonFile("appsettings.local.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((ctx, services) =>
                {
                    services.AddSingleton<Infraestructure.Services.AuthenticationService>();

                    services.AddDbContext<AppDbContext>(opt =>
                        opt.UseNpgsql(ctx.Configuration.GetConnectionString("MotixDb")));

                    services.AddScoped<IRepository<Customer>, CustomerRepository>();
                    services.AddScoped<IRepository<User>, UserRepository>();
                    services.AddScoped<IRepository<Rol>, RolRepository>();
                    services.AddScoped<IRepository<PaymentMethod>, PaymentMethodRepository>();
                    services.AddScoped<IRepository<Part>, PartRepository>();
                    services.AddScoped<IRepository<Document>, DocumentRepository>();

                    services.AddScoped<IUnitOfWork, UnitOfWork>();

                    services.AddTransient<SalesViewModel>();
                    services.AddTransient<AddLineWindowViewModel>();
                    services.AddTransient<StockViewModel>();
                    services.AddTransient<DocumentViewModel>();
                    services.AddTransient<SearchResultsViewModel>();
                    services.AddTransient<DeliveryViewModel>();
                    services.AddTransient<StockItemViewModel>();
                })
                .Build();

            Host.Start();

            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _salesWindow = new SalesWindow();
            _salesWindow.Activate();

            IntPtr hWnd = WindowNative.GetWindowHandle(_salesWindow);
            ShowWindow(hWnd, SW_MAXIMIZE);
        }

        private SalesWindow? _salesWindow;
    }
}
