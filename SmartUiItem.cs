using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace avalonia_rider_test;

public class SmartUiItem
{

    public SmartUiItem()
    {
        toggle = false;
        name = "Unknown";
        status = "Unknown";
    }
    public SmartUiItem(bool toggle, string name, string status)
    {
        this.toggle = toggle;
        this.name = name;
        this.status = status;
    }

    public string name { set; get; }
    public string status { set; get; }
    public bool toggle { set; get; }
}