using System.Windows.Controls;

namespace ClipboardAI.Views.Settings;

public partial class AISettingsPanel : UserControl
{
    public AISettingsPanel()
    {
        InitializeComponent();
    }

    private void TokenPasswordBox_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is ViewModels.Settings.AISettingsViewModel vm)
        {
            TokenPasswordBox.Password = vm.Token ?? string.Empty;
        }
    }

    private void TokenPasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is ViewModels.Settings.AISettingsViewModel vm)
        {
            vm.Token = TokenPasswordBox.Password;
        }
    }
}
