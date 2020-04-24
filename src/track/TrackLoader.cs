using DeepFlight.track;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private static readonly string FILE_EXTENSION = ".dftbd";

        private static readonly bool RANDOM_GENERATION = true;
        private static readonly int GENERATION_SEED = 1234;

        private static readonly string LOCAL_TRACK_FOLDER = "localtrack";
        private static readonly string GENERATOR_FILE_NAME = "TrackGenerator.exe";



        public static Task<Track[]> LoadOnlineTracks() { 
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
                                var track = TrackDeserializer.DeserializeMetaData(trackData);
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
                            //File.WriteAllBytes(seed + ".dft", TrackSerializer.Serialize(track));
                        }
                        track.Name = seed.ToString();
                        tracks.AddLast(track);
                    }

                    return tracks.ToArray();
            });
        }


        public static Task<Track> GenerateLocalTrack() {
            // The async process waiting is inspired from:
            // https://stackoverflow.com/questions/470256/process-waitforexit-asynchronously

            // Setup track folder
            PrepareLocalTrackFolder();

            // Setup needed paths
            var executionDir = GetExecutionDirectory();
            var trackFolderPath = GetExecutionDirectory() + LOCAL_TRACK_FOLDER + "/";
            var generatorPath = GetExecutionDirectory() + GENERATOR_FILE_NAME;
                
            var tcs = new TaskCompletionSource<Track>();

            // Setup the generation Process
            var process = new Process();
            process.StartInfo.FileName = generatorPath;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.Arguments = string.Format("{0} {1} {2} {3} {4} {5} {6} {7}",
                trackFolderPath, "randomtrack",  DateTime.Now.Millisecond,
                10, 10, 10, 10, 10
            );
            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) => {
                if( process.ExitCode != 0) {
                    tcs.TrySetResult(null);
                }
                else {
                    var generatedTrack = LoadGeneratedTrack(trackFolderPath);
                    generatedTrack.ID = 0;
                    generatedTrack.Name = "Unknown Cave";
                    generatedTrack.Seed = DateTime.Now.Millisecond;
                    generatedTrack.Planet = new Planet(1, "Unknown Planet", new int[] { 40, 40, 80 });
                    tcs.TrySetResult(generatedTrack);
                }
            };

            // Redirect the TrackGenerator's output
            process.StartInfo.RedirectStandardOutput = true;
            process.OutputDataReceived += (s, e) => Console.WriteLine("TrackGenerator:\t" + e.Data);
            
            process.Start();
            process.PriorityBoostEnabled = true;
            process.PriorityClass = ProcessPriorityClass.High;
            process.BeginOutputReadLine();

            return tcs.Task;
        }



        /// <summary>
        /// Loads 1 Track from the LOCAL_TRACK_FOLDER, which should exist after generating
        /// a Track.
        /// </summary>
        private static Track LoadGeneratedTrack(string folderPath) {
            string[] trackFiles = Directory.GetFiles(folderPath, "*" + FILE_EXTENSION).Select(Path.GetFileName).ToArray();

            if (trackFiles.Length == 0) {
                Console.WriteLine("Error: No track files found in local track folder after generating new track");
                return null;
            }

            if (trackFiles.Length > 1)
                Console.WriteLine("WARNING: '{0}' track files in local track folder after generating new track!", trackFiles.Length);

            return LoadTrackFile(folderPath + trackFiles[0]);            
        }


        /// <summary>
        /// Load all the pregenerated Tracks which are shipped with the game,
        /// and can be played offline.
        /// </summary>
        /// <returns></returns>
        public static Task<Track[]> LoadOfflineTracks() {
            return Task.Run(() => {
                Console.WriteLine("Loading offline tracks...");
                var offlineTracksFolder = GetExecutionDirectory() + Settings.OFFLINE_TRACKS_FOLDER + "/";
                string[] trackFiles = Directory.GetFiles(offlineTracksFolder, "*" + FILE_EXTENSION).Select(Path.GetFileName).ToArray();

                Track track;
                var tracks = new LinkedList<Track>();

                track = LoadTrackFile(offlineTracksFolder + "aerth" + FILE_EXTENSION);
                track.ID = 0;
                track.Name = "AGRC-313";
                track.Seed = 0;
                track.Planet = new Planet(1, "Aerth", new int[] { 49, 102, 44 });
                tracks.AddLast(track);

                track = LoadTrackFile(offlineTracksFolder + "smar" + FILE_EXTENSION);
                track.ID = 0;
                track.Name = "IAUI-636";
                track.Seed = 0;
                track.Planet = new Planet(1, "Smar", new int[] { 150, 30, 9 });
                tracks.AddLast(track);

                track = LoadTrackFile(offlineTracksFolder + "turnsa" + FILE_EXTENSION);
                track.ID = 0;
                track.Name = "NSIY-432";
                track.Seed = 0;
                track.Planet = new Planet(1, "Turnsa", new int[] { 120, 120, 90 });
                tracks.AddLast(track);
                
                Console.WriteLine("Loaded offline tracks: ");
                foreach(var offlineTrack in tracks) {
                    Console.WriteLine("\t" + offlineTrack);
                }

                return tracks.ToArray();
            });
        }



        /// <summary>
        /// Runs the 'LoadTrackFile(..)' method as an asynchronous task
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Task<Track> LoadTrackFileAsync(string filePath) {
            return Task.Run(() => {
                return LoadTrackFile(filePath);
            });
        }


        /// <summary>
        /// Loads a Track from a file and deserialize the bytes into a Track
        /// object. It does NOT deserialize the Block data.
        /// </summary>
        /// <param name="filePath"> The full path to the Track file</param>
        public static Track LoadTrackFile(string filePath) {
            byte[] trackData;
            try {
                trackData = File.ReadAllBytes(filePath);
            }
            catch (Exception e) {
                Console.WriteLine("ERROR: Got exception during LoadTrackFile using path {0}", filePath);
                Console.WriteLine(e);
                return null;
            }

            var track = new Track();
            track.BlockData = trackData;

            return track;
        }


        /// <summary>
        /// Deletes the local track folder and its content, if it exists,
        /// and recreate the folder.
        /// 
        /// The local track folder is used to hold the local track generated
        /// by the TrackGenerator.exe
        /// </summary>
        /// <returns></returns>
        public static void PrepareLocalTrackFolder() {
            var directoryInfo = new DirectoryInfo(GetExecutionDirectory()+LOCAL_TRACK_FOLDER);

            if( directoryInfo.Exists) {
                // Deletes files
                foreach (FileInfo file in directoryInfo.GetFiles()) {
                    file.Delete();
                }

                // Delete subfolders (there shouldn't be any)
                foreach (DirectoryInfo dir in directoryInfo.GetDirectories()) {
                    dir.Delete(true);
                }
            }

            // Recreate the folder
            directoryInfo.Create();
        }


        /// <summary>
        /// Retrieves the Directory of the current executing assembly
        /// (hopefully the game program).
        /// </summary>
        private static string GetExecutionDirectory() {
            // Fetching the permanent assembly execution path (for this assembly that is)
            string executionPath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            return new Uri(new Uri(executionPath), ".").LocalPath;
        }


       
    } 
}
