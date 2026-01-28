using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceProcess;
using System.Windows.Input;

namespace Ion.ServicesApp.MainWindow;

internal sealed class ServiceViewModel : BindableBase
{
    private readonly ServiceController _controller;
    private readonly DelegateCommand _startCommand;
    private readonly DelegateCommand _stopCommand;

    private bool _signed;
    private bool _exists;

    public ServiceViewModel(ServiceController controller)
    {
        _controller = controller;
        _startCommand = new DelegateCommand(() => DoWithRefresh(_controller.Start, ServiceControllerStatus.Running), () => CanStart);
        _stopCommand = new DelegateCommand(() => DoWithRefresh(_controller.Stop, ServiceControllerStatus.Stopped), () => CanStop);
        ExploreCommand = new DelegateCommand(() => Process.Start("explorer.exe", $"/select, \"{Path}\""), () => CanExplore);
        Path = GetServicePath(controller);
        Created = new FileInfo(Path).CreationTime.ToString("yyyy.MM.dd");
    }

    public ICommand StartCommand => _startCommand;
    public ICommand StopCommand => _stopCommand;
    public ICommand ExploreCommand { get; }

    public string DisplayName => _controller.DisplayName;
    public string ServiceName => _controller.ServiceName;
    public string Type => _controller.ServiceType.ToString();
    public ServiceControllerStatus Status => _controller.Status;
    public string Path { get; }
    public string Created { get; }

    public bool Signed { get => _signed; set => SetProperty(ref _signed, value); }
    public bool Exists { get => _exists; set => SetProperty(ref _exists, value); }
    public bool CanStart => _controller.Status is ServiceControllerStatus.Stopped or ServiceControllerStatus.Paused;
    public bool CanStop => _controller.Status is ServiceControllerStatus.Running;
    public bool CanExplore => File.Exists(Path);

    public void Refresh()
    {
        _startCommand.RaiseCanExecuteChanged();
        _stopCommand.RaiseCanExecuteChanged();

        OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(Status)));
    }

    private void DoWithRefresh(Action action, ServiceControllerStatus status)
    {
        action();

        do
        {
            Thread.Sleep(1000);
            _controller.Refresh();
            Refresh();
        } while (_controller.Status != status);
    }

    public async Task InitializeAsync()
    {
        await Task.Run(() =>
        {
            Exists = File.Exists(Path);

            try
            {
                using var certificate = X509Certificate2.CreateFromSignedFile(Path);
                Signed = true;
            }
            catch (CryptographicException e)
            {
                Signed = false;
            }
        });
    }

    private static string GetServicePath(ServiceController controller)
    {
        var path = ReadServiceRegistryKey(controller.ServiceName, "ImagePath")?.Replace(@"\SystemRoot", @"C:\Windows");

        if (string.IsNullOrEmpty(path))
        {
            path = @$"C:\Windows\System32\drivers\{controller.ServiceName}.sys";
            return path;
        }

        if (path.StartsWith("System32", StringComparison.OrdinalIgnoreCase))
        {
            path = path.Replace("System32", @"C:\Windows\System32", StringComparison.OrdinalIgnoreCase);
        }

        return path.Replace(@"\??\", string.Empty);
    }

    private static string? ReadServiceRegistryKey(string serviceName, string keyName)
    {
        string registryPath = @$"SYSTEM\CurrentControlSet\Services\{serviceName}";

        using var key = Registry.LocalMachine.OpenSubKey(registryPath);
        var imagePath = key?.GetValue(keyName)?.ToString();
        key?.Close();
        return imagePath;
    }
}