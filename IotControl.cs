using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace avalonia_rider_test;

public class IotControl
{
    private readonly UdpClient _server;
    private IPEndPoint _client;
    private Nodes? _nodeList;
    private byte[]? _buff;
    private string? _resp;
    private bool _shouldRun;

    public IotControl()
    {
        _nodeList = File.Exists(@"./nodes.json")
            ? JsonConvert.DeserializeObject<Nodes>(File.ReadAllText(@"./nodes.json"))
            : new Nodes();
        
        _server = new UdpClient(8984);
        _client = new IPEndPoint(IPAddress.Any, 0);
        _shouldRun = true;
    }
    
    public async void start()
    {
        //loop to handle connections
        await Task.Run(function: () =>
        {
            while (_shouldRun)
            {
                _buff = _server.Receive(ref _client);
                _resp = Encoding.ASCII.GetString(_buff);
                
                
                Console.WriteLine($"{_resp} from {_client.Address}");
                //send whole ass nodes to the esp for updating stuff
                Node temp = JsonConvert.DeserializeObject<Node>(_resp);
                
                //for loop to match to a local node, and update data locally if it exists,
                //then raise an event that data has changed so the ui can act upon it

            }
            return null;
        });
    }

    public void stop()
    {
        _shouldRun = false;
    }

    public bool saveNodes()
    {
        string output = JsonConvert.SerializeObject(_nodeList);
        Console.WriteLine(output);
        File.WriteAllText(@"./nodes.json", output);
        return true; //for returning if it ran as intended or not ig
    }

    public void addNode(Node n)
    {
        _nodeList.NodeList.Add(n);
    }

    // private async void runUdp()
    // {
    //     await Task.Run(function: () =>
    //     {
    //         Server = new UdpClient(8984);
    //
    //         Byte[] response = Encoding.ASCII.GetBytes("0,110");
    //         Byte[] response2 = Encoding.ASCII.GetBytes("hi other one");
    //         
    //         Console.WriteLine("starting listen loop");
    //         while (true)
    //         {
    //             buffer = Server.Receive(ref client);
    //             data = Encoding.ASCII.GetString(buffer);
    //
    //             Console.WriteLine($"{data} from {client.Address}");
    //
    //             if (data.Equals("a"))
    //             {
    //                 Console.WriteLine("replying to 1");
    //                 for (int i = 10; i <= 110; i+= 10)
    //                 {
    //                     Thread.Sleep(100);
    //                     Server.Send(Encoding.ASCII.GetBytes("0," + i), client.Address.ToString(), 8984);
    //                 }
    //             }
    //             if (data.Equals("b"))
    //             {
    //                 Console.WriteLine("replying to 2");
    //                 for (int i = 0; i < 10; i++)
    //                 {
    //                     Server.Send(response2, client.Address.ToString(), 8984);
    //                 }
    //             }
    //         }
    //     });
}