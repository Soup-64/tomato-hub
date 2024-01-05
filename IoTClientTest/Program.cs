
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using System.Xml.Serialization;

public enum NodeStatus
{
    NoInit = 0,  //default
    IoErr = 1,  //device connected, NodeData corrupted/unreadable
    CommErr = 2,  //device paired, no communication back
    SensorErr = 3,  //device connected, could not return sensor NodeData
    UnknownErr = 4,  //who knows, surely not me
    NotReady = 5,  //init still in progress
    Ok = 6   //no issues
}

public class IoTClientTest
{

    public static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        var client = new UdpClient(8985);

        int id = 999;

        Init init = new(){
            IdNum = id,
            Type = typeof(LightNode)
        };

        // string data = JsonConvert.SerializeObject(info);
        // byte[] bytes = Encoding.ASCII.GetBytes(data);
        // Console.WriteLine(data);

        // client.Send(bytes, bytes.Length, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 88984));

        LightNode info = new()
        {
            IdNum = id,
            DevName = "anotherone",
            Ip = "127.0.0.1",
            Status = NodeStatus.NotReady,
            activated = true,
            Brightness = 26
        };

        
        // XmlSerializer ser = new(typeof(Node));
        // StringWriter writer = new();
        // ser.Serialize(writer, info);
        // string data = writer.ToString();

        //prep and send init state
        string data = JsonConvert.SerializeObject(info);
        byte[] bytes = Encoding.ASCII.GetBytes(data);
        Console.WriteLine(data);

        client.Send(bytes, bytes.Length, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 58984));

        while (true)
        {
            //loop wait for changes
            IPEndPoint end = new IPEndPoint(IPAddress.Any, 0);
            byte[] inBuf = new byte[255];
            inBuf = client.Receive(ref end);
            string back = Encoding.ASCII.GetString(inBuf);
            Console.WriteLine(back);
            info = JsonConvert.DeserializeObject<LightNode>(back);
            Console.WriteLine(info.activated);

        }
    }
}

public class Init
{
    public int IdNum { set; get; }
    public Type Type { set; get; }
}

public class Node
{
    public int IdNum { set; get; }
    public string DevName { set; get; }
    public string? Ip { set; get; }
    public NodeStatus Status { set; get; }
    public bool activated { set; get; }

    // public Node(int idNum, string devName)
    // {
    //     this.IdNum = idNum;
    //     this.DevName = devName;
    // }
}

public class LightNode : Node
{
    public double Brightness;

    // public LightNode(int idNum, string devName) : base(idNum, devName)
    // {

    // }
}

public class RgbNode : LightNode
{
    public int R, G, B;

    // public RgbNode(int idNum, string devName) : base(idNum, devName)
    // {

    // }
}