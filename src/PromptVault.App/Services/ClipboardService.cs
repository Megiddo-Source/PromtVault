using System.Windows;

namespace PromptVault.App.Services;

public sealed class ClipboardService : IClipboardService
{
    public void SetText(string text)
    {
        Clipboard.SetText(text ?? string.Empty);
    }
}
