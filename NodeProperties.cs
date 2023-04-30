using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace avalonia_rider_test;

//class to hold data from json and runtime related info to communicate with ESPs
public class Node
{
    public int IdNum { set; get; }
    public string DevName { set; get; }
    public IPAddress Ip { set; get; }
    public NodeStatus Status { set; get; }
    public bool activated { set; get; }
    
    public Node(int idNum, string devName)
    {
        this.IdNum = idNum;
        this.DevName = devName;
    }
}

public class LightNode : Node
{
    public double Brightness;

    public LightNode(int idNum, string devName) : base(idNum, devName)
    {
        
    }
}

public class RgbNode : LightNode
{
    public int R, G, B;

    public RgbNode(int idNum, string devName) : base(idNum, devName)
    {
        
    }
}

public class SensorNode : Node
{
    public string[] ValNames;
    public string[] ValUnits;
    public double[] Vals;

    public SensorNode(int idNum, string devName) : base(idNum, devName)
    {
        
    }
    
    public SensorNode(int idNum, string devName, string[] valNames, string[] valUnits) : base(idNum, devName)
    {
        this.ValNames = valNames;
        this.ValUnits = valUnits;
    }
}

//for json list of nodes
[Serializable]
public class Nodes
{
    public List<Node?> NodeList;

    public Nodes()
    {
        NodeList = new List<Node?>();
    }
}

//probably can remove, since actual types of boxed classes are exposed
public enum NodeType
{
    Light       =   0,
    RgbLight    =   1,
    Sensor      =   2,
    Remote      =   3,
}

//enum for status return codes
public enum NodeStatus
{
    NoInit      =   0,  //default
    IoErr       =   1,  //device connected, NodeData corrupted/unreadable
    CommErr     =   2,  //device paired, no communication back
    SensorErr   =   3,  //device connected, could not return sensor NodeData
    UnknownErr  =   4,  //who knows, surely not me
    NotReady    =   5,  //init still in progress
    Ok          =   6   //no issues
}