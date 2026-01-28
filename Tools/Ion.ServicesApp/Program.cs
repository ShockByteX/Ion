using Ion.ServicesApp.MainWindow;

namespace Ion.ServicesApp;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        var app = new App();

        var view = new MainView();
        view.DataContext = new MainViewModel();

        return app.Run(view);
    }
}