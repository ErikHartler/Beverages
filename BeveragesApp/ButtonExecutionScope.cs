namespace BeveragesApp;

/// <summary>
/// Temporarily disables the clicked control and restores its previous state when
/// the event handler finishes, including early returns and exceptions.
/// </summary>
internal sealed class ButtonExecutionScope : IDisposable
{
    private readonly VisualElement? _control;
    private readonly bool _wasEnabled;

    public ButtonExecutionScope(object? sender)
    {
        _control = sender as VisualElement;
        if (_control is null)
        {
            return;
        }

        _wasEnabled = _control.IsEnabled;
        _control.IsEnabled = false;
    }

    public void Dispose()
    {
        if (_control is not null)
        {
            _control.IsEnabled = _wasEnabled;
        }
    }
}
