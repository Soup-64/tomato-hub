using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using ReactiveUI;

namespace avalonia_rider_test;


public partial class SmartMenu : UserControl, ActiveControl, IReactiveObject
{
    private StackPanel panel = new();

    private Nodes nodeList;
    private IotControl control;

    public SmartMenu()
    {
        InitializeComponent();
        control = new();
        nodeList = control.getNodes();
        
        control.NewNodeAdded += ControlOnNewNodeAdded;
        control.NodeDataChanged += ControlOnNodeDataChanged;
        
        //set internal list as source
        this.SmartItems.ItemsSource = nodeList.NodeList;

        control.start(); //actually start the listener
    }

    private void ControlOnNodeDataChanged(Node n)
    {
        Console.WriteLine($"Node {n.DevName} changed!");
        //edit entry in ui
    }

    private void ControlOnNewNodeAdded(Node n)
    {
        Console.WriteLine($"New node {n.DevName} added!");
        //add entry in ui
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
        Console.WriteLine("prop changing");
    }

    public void RaisePropertyChanged(PropertyChangedEventArgs args)
    {
        Console.WriteLine("prop change");
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        //for testing if the popups work at all
        ToggleSwitch? tog = sender as ToggleSwitch;
        Console.WriteLine(tog.Name);
        
    }
}