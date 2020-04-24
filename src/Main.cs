using DeepFlight.generation;
using DeepFlight.src;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

public static class Program {

    static bool done = false;

    [STAThread]
    static void Main() {
        
        // Setup logging
        Directory.CreateDirectory("logs");
        var traceListener = new TextWriterTraceListener(Path.Combine("logs", "generic.log"));
        Trace.Listeners.Add(traceListener);
        Trace.AutoFlush = true;

        Trace.TraceInformation("Program started");

        bool error = false;

        try {
            var game = new ApplicationController();
            game.Run();
        } catch( Exception e) {
            //Console.WriteLine("Exception occure: " + e.Message);
            Trace.TraceError("\nFatal error: " + e);
            error = true;
        }

        if (error)
            Trace.TraceInformation("Program terminated with errors");
        else
            Trace.TraceInformation("Program terminated without errors");

        // Apparently process is not killed automatically
        Process.GetCurrentProcess().Kill();

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