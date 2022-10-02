using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace avalonia_rider_test;

public partial class simpleButton : UserControl
{
    public simpleButton()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}