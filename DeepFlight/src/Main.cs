using DeepFlight.generation;
using DeepFlight.src;
using System;
using System.Threading;

public static class Program {

    static bool done = false;

    [STAThread]
    static void Main() {
        //BlockMapTest.RunTests();

        var game = new ApplicationController();
        game.Run();

        //TestTrackLoading();

        //while (!done) {
        //    Thread.Sleep(100);
        //}

        //Console.WriteLine("Finished program!");

        //RestTest.Test();

        //System.Threading.Thread.Sleep(3000);


    }


    static async void TestTrackLoading() {

        var genTask = TrackLoader.GenerateLocalTrack();
        Track track = await genTask;
        Console.WriteLine("Finished generating track");

        Console.WriteLine(track);
        
        done = true;
    }
}