using System;
using System.Diagnostics;
using System.Net;

namespace avalonia_rider_test;

//class to hold data from json and runtime related info to communicate with ESPs
public class Node
{
    public int idNum;
    public string devName;
    public IPAddress ip;
    public NodeStatus status;
    
    public Node(int idNum, string devName)
    {
        this.idNum = idNum;
        this.devName = devName;
    }
}

public class LightNode : Node
{
    public double brightness;

    public LightNode(int idNum, string devName) : base(idNum, devName)
    {
        
    }
}

public class RgbNode : LightNode
{
    public int r, g, b;

    public RgbNode(int idNum, string devName) : base(idNum, devName)
    {
        
    }
}

public class SensorNode : Node
{
    public string[] valNames;
    public string[] valUnits;
    public double[] vals;

    public SensorNode(int idNum, string devName) : base(idNum, devName)
    {
        
    }
    
    public SensorNode(int idNum, string devName, string[] valNames, string[] valUnits) : base(idNum, devName)
    {
        this.valNames = valNames;
        this.valUnits = valUnits;
    }
}

//for json list of nodes
[Serializable]
public class Nodes
{
    public Node[] nodeList;

    public Nodes(int count)
    {
        nodeList = new Node[count];
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