using System;
using System.Diagnostics;
using System.IO;
using Avalonia;
using NetCoreAudio;
using Newtonsoft.Json;

namespace avalonia_rider_test
{
    internal static class Program
    {
        //TODO: add some error checking on build.sh so it stops if anything fails
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            //creating stuff
            Nodes n = new()
            {
                nodeList =
                {
                    new LightNode(123, "somelight"),
                    new RgbNode(456, "balls")
                }
            };
            n.nodeList[0].status = NodeStatus.Ok;
            
            //serialize class, will have to empty out stuff that shouldn't be saved or
            //remember to change it next run, or make more complex node props that splits this up more
            string output = JsonConvert.SerializeObject(n);
            Console.WriteLine(output);
            
            //replaces all text with new serialized data
            File.WriteAllText(@"./nodes.json", output);


#if !DEBUG
            //this is because the rpi apparently only inits audio streams fully when something actually tries to play, so by
            //forcing audio at launch we can get audio ready so first sound effect isn't missed

            Player fxWav = new Player();
            fxWav.Play("./output.wav");
            
            Process p = new();
            p.StartInfo.FileName = "/home/auto/sleep.sh";
            p.StartInfo.CreateNoWindow = true;
            p.Start();
#endif
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        
        // Avalonia configuration, don't remove; also used by visual designer.
        private static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>().UsePlatformDetect().LogToTrace();
    }
}