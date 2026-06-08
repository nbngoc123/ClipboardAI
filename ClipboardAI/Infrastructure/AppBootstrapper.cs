using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ClipboardAI.Data;
using ClipboardAI.Data.Repositories;
using ClipboardAI.Services.Clipboard;
using ClipboardAI.Services.Hotkey;

namespace ClipboardAI.Infrastructure;

public class AppBootstrapper
{
    public IServiceProvider ServiceProvider { get; private set; }

    public AppBootstrapper()
    {
        var services = new ServiceCollection();
        ServiceRegistration.ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();
        
        ServiceLocator.Initialize(ServiceProvider);
    }

    public async Task StartAsync()
    {
        // 1. Init Database
        var dbInit = ServiceProvider.GetRequiredService<DatabaseInitializer>();
        await dbInit.InitializeAsync();

        // 2. Start Clipboard Service
        var clipboardService = ServiceProvider.GetRequiredService<IClipboardService>();
        var repository = ServiceProvider.GetRequiredService<IClipboardRepository>();
        var historyVm = ServiceProvider.GetRequiredService<ViewModels.History.HistoryViewModel>();
        var settingsManager = ServiceProvider.GetRequiredService<ClipboardAI.Services.Settings.SettingsManager>();
        
        clipboardService.ClipboardChanged += async (s, item) => 
        {
            await repository.InsertAsync(item);
            historyVm.AddNewItem(item);
            historyVm.EnsureLimit(settingsManager.CurrentSettings.MaxHistoryItems);
        };
        
        clipboardService.Start();

        // 3. Register Hotkeys
        var hotkeyRegistrar = ServiceProvider.GetRequiredService<HotkeyRegistrar>();
        hotkeyRegistrar.RegisterDefaultHotkeys();
        
        // Initialize Dispatcher
        _ = ServiceProvider.GetRequiredService<HotkeyActionDispatcher>();
    }

    public void Stop()
    {
        var clipboardService = ServiceProvider.GetRequiredService<IClipboardService>();
        clipboardService.Stop();
        
        var hotkeyRegistrar = ServiceProvider.GetRequiredService<HotkeyRegistrar>();
        hotkeyRegistrar.UnregisterAll();
    }
}
