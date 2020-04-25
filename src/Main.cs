
using DeepFlight.control.offlinetracktime;
using System;
using System.Diagnostics;
using System.IO;

public static class Program {


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
            // Runs the Game application
            var game = new Application();
            game.Run();
        }
        catch (Exception e) {
            //Console.WriteLine("Exception occure: " + e.Message);
            Trace.TraceError("\nFatal error: " + e);
            error = true;
        }

        if (error)
            Trace.TraceInformation("Program terminated with errors");
        else
            Trace.TraceInformation("Program terminated without errors");

        // Apparently process is not killed automatically
        // due to MonoGame
        Process.GetCurrentProcess().Kill();
    }
}