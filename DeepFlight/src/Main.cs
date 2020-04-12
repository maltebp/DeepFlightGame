using DeepFlight.generation;
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



    }


    static async void TestTrackLoading() {

        var genTask = TrackLoader.GenerateLocalTrack();
        Track track = await genTask;
        Console.WriteLine("Finished generating track");

        Console.WriteLine(track);
        
        done = true;
    }
}