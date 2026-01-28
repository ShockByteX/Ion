using System.Collections.ObjectModel;
using System.ServiceProcess;
using System.Windows.Input;

namespace Ion.ServicesApp.MainWindow;

internal sealed class MainViewModel : BindableBase
{
    public MainViewModel()
    {
        Services = new ObservableCollection<ServiceViewModel>();

        var services = ServiceController.GetDevices();

        foreach (var service in services.OrderBy(x => x.ServiceName))
        {
            Services.Add(new ServiceViewModel(service));
        }

        Task.Run(async () =>
        {
            var tasks = Services.Select(x => x.InitializeAsync());
            await Task.WhenAll(tasks);
        });
    }

    public ObservableCollection<ServiceViewModel> Services { get; }
    public ObservableCollection<ServiceViewModel> Selected { get; set; }
}