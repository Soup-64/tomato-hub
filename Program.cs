﻿using System;
using Avalonia;
using NetCoreAudio;

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
#if !DEBUG
            //this is because the rpi apparently only inits audio streams fully when something actually tries to play, so by
            //forcing audio at launch we can get audio ready so first sound effect isn't missed
            Player fxWav = new Player();
            fxWav.Play("./output.wav");
            Console.WriteLine("test");
#endif
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        //some boiler plate example shit for playing audio async
        // private static async void Plays()
        // {
        //     await Task.Run( () =>
        //     {
        //         var fxWav = new Player();
        //
        //         fxWav.Play("./output.wav");
        //     });
        // }

        // Avalonia configuration, don't remove; also used by visual designer.
        private static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>().UsePlatformDetect().LogToTrace();
    }
}