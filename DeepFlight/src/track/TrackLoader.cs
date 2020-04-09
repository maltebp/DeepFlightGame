using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DeepFlight.generation {
    public static class TrackLoader {

        // Whether or not to use saved track files or generate new ones
        private static readonly bool LOAD_TRACKS = true;

        // Save track as a file after generating it
        private static readonly bool SAVE_TRACK = true;

        private static readonly string FILE_EXTENSION = ".dft";

        private static readonly bool RANDOM_GENERATION = true;
        private static readonly int GENERATION_SEED = 1234;


        public static Task<Track[]> LoadTracks() { 
            // Running a seperate task, and returning Task, we make the
            // method 'awaitable'.
            return Task.Run(() => {
                    Console.WriteLine("Loading tracks...");

                    // Fetching the permanent assembly execution path (for this assembly that is)
                    string executionPath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                    Console.WriteLine("Execution path: '{0}'", executionPath);

                    // Adjust to directory, not executable file
                    Uri baseAddress = new Uri(executionPath);
                    //Uri directory = new Uri(baseAddress, ".").LocalPath;

                    string executionDirectory = new Uri(new Uri(executionPath), ".").LocalPath;// new Uri(executionPath).LocalPath;//directory.OriginalString.Remove(0, 8); // Remove the 'file:///' from the string
                    Console.WriteLine("Execution directory: " + executionDirectory);

                    string[] trackFiles = Directory.GetFiles(executionDirectory, "*"+FILE_EXTENSION).Select(Path.GetFileName).ToArray();
                    Console.WriteLine("Track files:  " );
                    foreach (var file in trackFiles) Console.Write("'{0}'  ", file);
                    Console.WriteLine("\n");

                    LinkedList<Track> tracks = new LinkedList<Track>();

                    // Load saved tracks
                    if(LOAD_TRACKS && trackFiles.Length > 0) {
                            Console.WriteLine("Loading saved tracks");
                            foreach(var file in trackFiles) {
                                var trackData = File.ReadAllBytes(file);
                                var track = TrackSerializer.Deserialize(trackData);
                                track.Name = file.Remove(file.Length-FILE_EXTENSION.Length);
                                Console.WriteLine("Loaded track: " + track.Name);
                                tracks.AddLast(track);
                            }
                        }

                    // Generate new track
                    else {
                        Console.WriteLine("Generating new track");
                        // Generate track (textures must be loaded before generating)
                        var seed = GENERATION_SEED;
                        if (RANDOM_GENERATION) {
                            Console.WriteLine("Using random seed");
                            Random rand = new Random();
                        seed = rand.Next(0, 999999);
                        }
                        var track = Generator.GenerateTrack(seed);
                        Console.WriteLine("Generated track: " + track);
                        if (SAVE_TRACK) {
                            Console.WriteLine("Saving track");
                            File.WriteAllBytes(seed + ".dft", TrackSerializer.Serialize(track));
                        }
                        track.Name = seed.ToString();
                        tracks.AddLast(track);
                    }

                    return tracks.ToArray();
            });
        }
    
    }
}
