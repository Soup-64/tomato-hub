using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ReactiveUI;

namespace avalonia_rider_test;


public partial class SmartMenu : UserControl, ActiveControl, IReactiveObject
{
    private Controls gridItems;
    object itemTest = new SmartUiItem(false, "device", "offline");
    private StackPanel panel = new();

    public SmartMenu()
    {
        InitializeComponent();
        //disable setting up the IoT backend for now
        //IotControl control = new();
        Control c = new();
        
        this.Panel.Children.Add();
        //this.Content = itemTest;
    }

    public void gridTest()
    {
        /**gridItems = controlGrid.Children;
        //hell
        
        IndexerBinding zero = new(controlGrid,
            new AttachedProperty<int>("Column", typeof(Grid), new StyledPropertyMetadata<int>(0)), BindingMode.Default);
        IndexerBinding one = new(controlGrid,
            new AttachedProperty<int>("Column", typeof(Grid), new StyledPropertyMetadata<int>(1)), BindingMode.Default);
        IndexerBinding two = new(controlGrid,
            new AttachedProperty<int>("Column", typeof(Grid), new StyledPropertyMetadata<int>(2)), BindingMode.Default);
            
        //gridItems.Clear();

        Grid testGrid = AvaloniaRuntimeXamlLoader.Parse<Grid>("""
<Grid xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
RowDefinitions="40,70" ColumnDefinitions="256,256" Name="devLight">
                <TextBlock Grid.Row="0" Grid.Column="0"
                                       Name="devName" 
                                       Text="button name">
                </TextBlock>
                <TextBlock Grid.Row="0" Grid.Column="1"
                         Name="devStatus"
                         Text="device status">
                </TextBlock>
                
                <Button Grid.Column="0" Grid.Row="1" Content="Advanced"
                        Name="devAdv">
                    <Button.Flyout>
                        <Flyout>
                            <TextBlock Text="some settings here" />
                        </Flyout>
                    </Button.Flyout>
                </Button>
                
                <ToggleButton Grid.Row="1" Grid.Column="1"
                              Name="devToggle"
                              Content="Toggle">
                              <!-- Click="buttonToggle" -->
                </ToggleButton>
            </Grid>
""");

        TextBlock t = new TextBlock
        {
            Text = "hi",
            [!Grid.RowProperty] = one,
            [!Grid.ColumnProperty] = one
        };
        testGrid[!Grid.RowProperty] = one;
        testGrid[!Grid.ColumnProperty] = zero;
        gridItems.Add(testGrid);
        gridItems.Add(t);
        */
    }

    public void changeActive(bool active)
    {
        this.IsVisible = active;
        //whatever else needs to be done here
    }

    private void buttonToggle(object? sender, RoutedEventArgs e)
    {
        ToggleButton? tog = sender as ToggleButton;
        if (tog != null) tog.Content = tog.IsChecked;
    }

    public event PropertyChangingEventHandler? PropertyChanging;
    public void RaisePropertyChanging(PropertyChangingEventArgs args)
    {
        throw new NotImplementedException();
    }

    public void RaisePropertyChanged(PropertyChangedEventArgs args)
    {
        throw new NotImplementedException();
    }
}