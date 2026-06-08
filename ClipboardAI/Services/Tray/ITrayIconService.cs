namespace ClipboardAI.Services.Tray;

public interface ITrayIconService
{
    void Show();
    void Hide();
    void UpdateTooltip(string text);
    void ShowNotification(string title, string message);
}
