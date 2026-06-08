using System.Windows;
using ClipboardAI.Infrastructure;
using ClipboardAI.Views.Windows;

namespace ClipboardAI;

public partial class App : Application
{
    private AppBootstrapper? _bootstrapper;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _bootstrapper = new AppBootstrapper();
        await _bootstrapper.StartAsync();

        var mainWindow = ServiceLocator.GetService<MainWindow>();
        mainWindow.DataContext = ServiceLocator.GetService<ViewModels.MainViewModel>();
        this.MainWindow = mainWindow;
        mainWindow.Show();
        
        // Tải dữ liệu lịch sử từ database
        var historyVm = ServiceLocator.GetService<ViewModels.History.HistoryViewModel>();
        await historyVm.LoadItemsAsync();

        // Phase 2: Không hiển thị MainWindow khi khởi động nữa, chỉ chạy ngầm với TrayIcon.
        // Người dùng sẽ bấm Ctrl+Shift+V để gọi Popup lên.
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _bootstrapper?.Stop();
        base.OnExit(e);
    }
}
