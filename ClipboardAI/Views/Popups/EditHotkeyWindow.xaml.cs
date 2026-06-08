using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ClipboardAI.Views.Popups;

public partial class EditHotkeyWindow : Window
{
    public string HotkeyString { get; private set; } = string.Empty;
    private bool _hasModifiers;
    private bool _hasKey;

    public EditHotkeyWindow(string currentHotkey)
    {
        InitializeComponent();
        HotkeyDisplay.Text = currentHotkey;
        HotkeyString = currentHotkey;
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        e.Handled = true;
        UpdateHotkeyDisplay(e);
    }

    private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
    {
        e.Handled = true;
        // Optional: you could reset if all keys are released without a valid combination,
        // but updating on KeyDown is usually enough.
    }

    private void UpdateHotkeyDisplay(KeyEventArgs e)
    {
        var key = e.Key == Key.System ? e.SystemKey : e.Key;
        
        // Ignore standalone modifiers
        if (key == Key.LeftCtrl || key == Key.RightCtrl ||
            key == Key.LeftShift || key == Key.RightShift ||
            key == Key.LeftAlt || key == Key.RightAlt ||
            key == Key.LWin || key == Key.RWin)
        {
            return;
        }

        var modifiers = Keyboard.Modifiers;
        _hasModifiers = modifiers != ModifierKeys.None;
        _hasKey = key != Key.None;

        if (_hasModifiers && _hasKey)
        {
            var sb = new StringBuilder();
            if (modifiers.HasFlag(ModifierKeys.Control)) sb.Append("Ctrl+");
            if (modifiers.HasFlag(ModifierKeys.Alt)) sb.Append("Alt+");
            if (modifiers.HasFlag(ModifierKeys.Shift)) sb.Append("Shift+");
            if (modifiers.HasFlag(ModifierKeys.Windows)) sb.Append("Win+");
            
            sb.Append(key.ToString());
            
            HotkeyString = sb.ToString();
            HotkeyDisplay.Text = HotkeyString;
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (_hasModifiers && _hasKey)
        {
            DialogResult = true;
            Close();
        }
        else if (!string.IsNullOrEmpty(HotkeyString) && HotkeyString != "Press a key combination...")
        {
            DialogResult = true; // allow keeping existing if no new one pressed
            Close();
        }
        else
        {
            MessageBox.Show("Please press a valid key combination (e.g., Ctrl + Shift + V).", "Invalid Hotkey");
        }
    }
}
