using Microsoft.Extensions.DependencyInjection;
using ClipboardAI.Data;
using ClipboardAI.Data.Repositories;
using ClipboardAI.Services.Clipboard;
using ClipboardAI.Services.Hotkey;
using ClipboardAI.Services.Tray;
using ClipboardAI.Services.Settings;
using ClipboardAI.ViewModels;
using ClipboardAI.ViewModels.History;
using ClipboardAI.Views.Windows;
using ClipboardAI.Views.Popups;

namespace ClipboardAI.Infrastructure;

public static class ServiceRegistration
{
    public static void ConfigureServices(IServiceCollection services)
    {
        // Data
        services.AddSingleton<DatabaseContext>();
        services.AddTransient<DatabaseInitializer>();
        services.AddSingleton<IClipboardRepository, ClipboardRepository>();

        // Services
        services.AddSingleton<SettingsManager>();
        services.AddSingleton<ITrayIconService, TrayIconService>();
        services.AddSingleton<IClipboardService, ClipboardService>();
        
        // Hotkey Services
        services.AddSingleton<IHotkeyService, HotkeyService>();
        services.AddSingleton<HotkeyRegistrar>();
        services.AddSingleton<HotkeyActionDispatcher>();

        // ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<HistoryViewModel>();
        services.AddSingleton<ClipboardAI.ViewModels.Settings.SettingsViewModel>();
        services.AddSingleton<ClipboardAI.ViewModels.Settings.GeneralSettingsViewModel>();
        services.AddSingleton<ClipboardAI.ViewModels.Settings.HotkeySettingsViewModel>();

        // Views
        services.AddSingleton<MainWindow>();
        services.AddSingleton<ClipboardPopup>();
    }
}
