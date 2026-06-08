using System;
using System.Windows.Input;

namespace ClipboardAI.Services.Hotkey;

public interface IHotkeyService
{
    event EventHandler<string>? OnHotkeyPressed;
    void Register(string name, Key key, ModifierKeys modifiers);
    void Unregister(string name);
}
