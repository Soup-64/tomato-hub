using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using MessageBox.Avalonia.BaseWindows.Base;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
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

        _server = new UdpClient(8984);
        _client = new IPEndPoint(IPAddress.Any, 0);
        _shouldRun = true;
    }

    public async void start()
    {
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
                //send whole ass nodes to the esp for updating stuff
                Node foundClient = JsonConvert.DeserializeObject<Node>(_resp);
                foundClient.Ip = _client.Address;

                for (int i = 0; i < _nodeList.NodeList.Count; i++)
                {
                    if (_nodeList.NodeList[i].IdNum.Equals(foundClient.IdNum))
                    {
                        _nodeList.NodeList[i] = foundClient; //sync if exists
                        //raise node change event with node
                        NodeDataChanged?.Invoke(_nodeList.NodeList[i]);
                    }
                    else
                    {
                        newNode(foundClient);
                        break;
                    }
                }


            }
            Console.WriteLine("no longer waiting for packets");
            return null;
        });
    }

    public void newNode(Node found)
    {
        {
            Console.WriteLine("ERROR! node does not exist!");

            //wait on dialog asynchronously to prevent locking the render thread
            Task.Run(async () =>
            {

                ButtonResult b = await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    IMsBoxWindow<ButtonResult>? dialog = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow(
                    new MessageBoxStandardParams
                    {
                        ButtonDefinitions = ButtonEnum.YesNo,
                        ContentTitle = "New Node",
                        ContentHeader = "New Node Detected",
                        ContentMessage = "A new node has been detected, would you like to add it?",
                        Icon = Icon.Warning
                    });
                    return dialog.Show();
                }, DispatcherPriority.MaxValue);


                Console.WriteLine(b.ToString());
                if (b != ButtonResult.Yes) return;
                addNode(found);
                NewNodeAdded?.Invoke(found);
                saveNodes();
            });
        }
    }

    //for the ui sending updates to the backend, which in turn sends data to the esps
    public void updateNode(Node n)
    {
        //some code for updating data on the backend
        //...
        //send node data to client esp
        byte[] buf = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(n));
        _server.Send(buf, new IPEndPoint(n.Ip, 8984));

        //TODO: implement function for updating node data, which should raise an event to send it out via udp server
    }

    public ref Nodes getNodes()
    {
        return ref _nodeList;
    }



    public void stop()
    {
        _shouldRun = false;
    }

    private bool saveNodes()
    {
        string output = JsonConvert.SerializeObject(_nodeList);
        Console.WriteLine(output);
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