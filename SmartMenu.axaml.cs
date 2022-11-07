using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace avalonia_rider_test;

public partial class SmartMenu : UserControl, ActiveControl
{
    public SmartMenu()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        IotControl control = new();

#if !DEBUG

#endif
    }
    
    public void changeActive(bool active)
    {
        this.IsVisible = active;
        //whatever else needs to be done here
    }

    private void ButtonToggle(object? sender, RoutedEventArgs e)
    {
        ToggleButton tog = sender as ToggleButton;
        tog.Content = tog.IsChecked;
    }
}