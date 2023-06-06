using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using MessageBox.Avalonia.BaseWindows.Base;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using NetCoreAudio;

namespace avalonia_rider_test;

public partial class MainMenu : UserControl, ActiveControl
{
    public MainMenu()
    {
        this.InitializeComponent();
    }

    public void changeActive(bool active)
    {
        this.IsVisible = active;
    }

    //UI EVENTS
        
    private void quit_Click(object? sender, RoutedEventArgs e)
    {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow.Close();
        }
    }

    private void sound_Click(object? sender, RoutedEventArgs e)
    {
        Player fxWav = new();

        fxWav.Play("./output.wav").Wait();
    }
        
    private void weather_Click(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("switch to weather");
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Carousel switcher = desktop.MainWindow.Find<Carousel>("Switcher");
            ((ActiveControl) switcher.SelectedItem!).changeActive(false);
            switcher.SelectedIndex = 1;
            ((ActiveControl) switcher.SelectedItem!).changeActive(true);
        }
    }

    private void Wifi_OnClick(object? sender, RoutedEventArgs e)
    {
        Button btn = this.Find<Button>("wifi");
        String ip = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
        btn.Content = ip;
    }

    private void Smarts_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Carousel switcher = desktop.MainWindow.Find<Carousel>("Switcher");
            ((ActiveControl) switcher.SelectedItem!).changeActive(false);
            switcher.SelectedIndex = 2;
            ((ActiveControl) switcher.SelectedItem!).changeActive(true);
        }
    }
}