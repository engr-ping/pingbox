using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace PingBox.Views;

public partial class InputDialog : Window
{
    public bool Confirmed { get; private set; }
    public string InputText => txtInput.Text ?? string.Empty;

    public InputDialog() : this(string.Empty) { }
    public InputDialog(string defaultText)
    {
        InitializeComponent();
        txtInput.Text = defaultText;
        Opened += (_, _) => { txtInput.SelectAll(); txtInput.Focus(); };
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Return) { Confirmed = true; Close(); }
        else if (e.Key == Key.Escape) { Confirmed = false; Close(); }
    }

    private void Confirm_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) { Confirmed = true; Close(); }
    private void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) { Confirmed = false; Close(); }
}
