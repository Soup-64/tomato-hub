using System;
using System.Diagnostics;
using System.Net;

namespace avalonia_rider_test;

//class to hold NodeData from json and such to communicate with esps
public class Node
{
    public Node(NodeID id, NodeType nodeType)
    {
        this.id = id;
        type = nodeType;
        
        switch(this.id.type)
        {
            case NodeType.Light:
            case NodeType.RgbLight:
                NodeData = new LightNodeData();
                (NodeData as LightNodeData).rgb = (id.type==NodeType.RgbLight);
                break;
        }
    }

    NodeID id;
    public NodeType type;
    public NodeData NodeData;
}

//generic NodeData class
//used at runtime for storing active information that will not be saved
public class NodeData
{
    public int status;
    public IPAddress ip;
}

//NodeData for lights
public class LightNodeData : NodeData
{
    public double brightness;
    public bool rgb;
    public int r, g, b;
}

public class SensorNodeData : NodeData
{
    public string[] valNames;
    public string[] valUnits;
    public double[] vals;
}

//for json list of nodes
[Serializable]
public class Nodes
{
    public Nodes(int count)
    {
        ids = new NodeID[count];
    }
    public NodeID[] ids;
}

//save node id and name for creating the ui element while waiting to connect
[Serializable]
public struct NodeID
{
    public int id;
    public string name;
    public NodeType type;
}

//enum for types of nodes, some will init data as a different type, others change flags
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
    Ok          =   0,  //no issues
    IoErr       =   1,  //device connected, NodeData corrupted/unreadable
    CommErr     =   2,  //device paired, no communication back
    SensorErr   =   3,  //device connected, could not return sensor NodeData
    UnknownErr  =   4,  //who knows, surely not me
    NotReady    =   5,  //init still in progress
}