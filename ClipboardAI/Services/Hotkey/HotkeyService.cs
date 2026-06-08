using System;
using System.Windows.Input;
using NHotkey;
using NHotkey.Wpf;

namespace ClipboardAI.Services.Hotkey;

public class HotkeyService : IHotkeyService
{
    public event EventHandler<string>? OnHotkeyPressed;

    public void Register(string name, Key key, ModifierKeys modifiers)
    {
        try
        {
            HotkeyManager.Current.AddOrReplace(name, key, modifiers, OnHotkeyEvent);
        }
        catch (HotkeyAlreadyRegisteredException)
        {
            // Ignore or log conflict
        }
    }

    public void Unregister(string name)
    {
        HotkeyManager.Current.Remove(name);
    }

    private void OnHotkeyEvent(object? sender, HotkeyEventArgs e)
    {
        e.Handled = true;
        OnHotkeyPressed?.Invoke(this, e.Name);
    }
}
