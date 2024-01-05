using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Avalonia.Controls;
using Avalonia.Threading;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using Newtonsoft.Json;

namespace avalonia_rider_test;

public delegate void IoTEvent(Node n);

public class IotControl
{
    public event IoTEvent NodeDataChanged;
    public event IoTEvent NewNodeAdded;


    private readonly UdpClient _server;
    private IPEndPoint _client;
    private Nodes? _nodeList;
    private byte[]? _buff;
    private string? _resp;
    private bool _shouldRun;

    public IotControl()
    {
        //read in saved list of nodes, or make a new one
        _nodeList = File.Exists(@"./nodes.json")
            ? JsonConvert.DeserializeObject<Nodes>(File.ReadAllText(@"./nodes.json"))
            : new Nodes();

        //clients send to server on port 8984, server sends updates to client on port 8985
        _server = new UdpClient(58984);
        _client = new IPEndPoint(IPAddress.Any, 58984);
        _shouldRun = true;
    }

    public async void start()
    {
        if (_nodeList is null)
        {
            Console.WriteLine("class not properly initialized!");
            return;
        }
        //loop to handle connections
        await Task.Run(function: () =>
        {
            Console.WriteLine("waiting for packets");
            while (_shouldRun)
            {
                //blocking call, will sit until there are packets to handle
                _buff = _server.Receive(ref _client);
                _resp = Encoding.ASCII.GetString(_buff);


                Console.WriteLine($"{_resp} from {_client.Address}");
                // XmlSerializer ser = new(typeof(Node));
                // Node foundClient = ser.Deserialize(new MemoryStream(_buff)) as Node;
                //send whole ass nodes to the esp for updating stuff
                Node foundClient = JsonConvert.DeserializeObject<Node>(_resp) ?? throw new InvalidOperationException();
                Console.WriteLine(foundClient.GetType());
                foundClient.Ip = _client.Address.ToString();

                bool found = false;
                for (int i = 0; i < _nodeList.NodeList.Count; i++)
                {
                    if (_nodeList.NodeList[i]!.IdNum == foundClient.IdNum)
                    {
                        int index = _nodeList.NodeList[i].index;
                        foundClient.index = index;
                        _nodeList.NodeList[i] = foundClient; //sync if exists
                        //raise node change event with node
                        NodeDataChanged?.Invoke(_nodeList.NodeList[i]!);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    newNode(foundClient);
                }


            }
            Console.WriteLine("no longer waiting for packets");
            return null;
        });
    }

    private void newNode(Node found)
    {
        {
            Console.WriteLine("ERROR! node does not exist!");

            //wait on dialog asynchronously to prevent locking the render thread
            Task.Run(async () =>
            {
                ButtonResult b = await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    IMsBox<ButtonResult>? dialog = MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard(
                    new MessageBoxStandardParams
                    {
                        ButtonDefinitions = ButtonEnum.YesNo,
                        ContentTitle = "New Node",
                        ContentHeader = "New Node Detected",
                        ContentMessage = "A new node has been detected, would you like to add it?",
                        Icon = Icon.Warning
                    });
                    return dialog.ShowAsync();
                }, DispatcherPriority.MaxValue);



                Console.WriteLine(b.ToString());
                if (b != ButtonResult.Yes) return;
                addNode(found);
                saveNodes();
                NewNodeAdded?.Invoke(found);
            });
        }
    }

    //for the ui sending updates to the backend, which in turn sends data to the esps
    public void updateNode(Node n)
    {
        for (int i = 0; i < _nodeList.NodeList.Count; i++)
        {
            if (n.IdNum == _nodeList.NodeList[i].IdNum)
            {
                _nodeList.NodeList[i] = n;

                if (_nodeList.NodeList[i].Ip is null){
                    Console.WriteLine("Could not contact device!");
                    return;
                }

                //send node data to client esp
                byte[] buf = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(_nodeList.NodeList[i]));
                Console.WriteLine(_nodeList.NodeList[i].Ip);
                _server.Send(buf, new IPEndPoint(IPAddress.Parse(_nodeList.NodeList[i].Ip), 58985));
                return;
            }
        }

        //TODO: implement function for updating node data, which should raise an event to send it out via udp server
    }

    public ref Nodes getNodes()
    {
        return ref _nodeList!;
    }



    public void stop()
    {
        _shouldRun = false;
    }

    private bool saveNodes()
    {
        //this hangs if the IPaddress class is used
        string output = JsonConvert.SerializeObject(_nodeList, Formatting.Indented);
        Console.WriteLine($"saving...\n{output}");
        try
        {
            File.WriteAllText(@"./nodes.json", output);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to save nodes! {e.Message}");
        }
        return true; //for returning if it ran as intended or not ig
    }

    private void addNode(Node? n)
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