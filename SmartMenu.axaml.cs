using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ReactiveUI;



namespace avalonia_rider_test;



public partial class SmartMenu : UserControl, ActiveControl, IReactiveObject
{
    //private List<Node> nodeList;
    private IotControl control;

    public SmartMenu()
    {
        InitializeComponent();


        control = new();
        //nodeList = control.getNodes().NodeList;

        control.NewNodeAdded += ControlOnNewNodeAdded;
        control.NodeDataChanged += ControlOnNodeDataChanged;

        //set internal list as source
        foreach(Node n in control.getNodes().NodeList){
            int index = this.SmartItems.Items.Add(n);
            n.index = index;
        }
        //this.SmartItems.ItemsSource = control.getNodes().NodeList;

        control.start(); //actually start the listener
    }

    private void ControlOnNodeDataChanged(Node n)
    {
        Console.WriteLine($"Node {n.DevName} changed!");
        if(n.index < 0){
            Console.WriteLine("unable to update ui");
            return;
        }
        this.SmartItems.Items.RemoveAt(n.index);
        this.SmartItems.Items.Insert(n.index, n);
    }

    private void ControlOnNewNodeAdded(Node n)
    {
        Console.WriteLine($"New node {n.DevName} was added!");
        int index = SmartItems.Items.Add(n);
        n.index = index;


        //nodeList = control.getNodes().NodeList;
        // Dispatcher.UIThread.InvokeAsync(() =>
        //         {
        //             this.SmartItems.ItemsSource = nodeList;

        //         }, DispatcherPriority.MaxValue);
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
        for (int i = 0; i < control.getNodes().NodeList.Count; i++)
        {
            if (int.Parse(tog.Name) == control.getNodes().NodeList[i].IdNum)
            {
                Console.WriteLine("updating!");
                //assumes the binding actually works
                control.updateNode(control.getNodes().NodeList[i]);
            }
        }
    }
}