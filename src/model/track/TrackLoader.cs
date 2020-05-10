using DeepFlight.control.offlinetracktime;
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

        private static readonly string GENERATOR_FILE_NAME  = "TrackGenerator.exe";
        private static readonly string LOCAL_TRACK_FOLDER   = "localtrack";
        private static readonly string FILE_EXTENSION       = ".dftbd";
       

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

                var trackTimeController = OfflineTrackTimeController.Instance;

                Track track;
                var tracks = new LinkedList<Track>();

                track = LoadTrackFile(offlineTracksFolder + "aerth" + FILE_EXTENSION);
                track.Id = "";
                track.Name = "AGRC-313";
                track.Planet = new Planet("1", "Aerth", new int[] { 49, 102, 44 });
                track.OfflineTime = trackTimeController.GetTrackTime(track.Name);
                tracks.AddLast(track);

                track = LoadTrackFile(offlineTracksFolder + "smar" + FILE_EXTENSION);
                track.Id = "";
                track.Name = "IAUI-636";
                track.Planet = new Planet("2", "Smar", new int[] { 150, 30, 9 });
                track.OfflineTime = trackTimeController.GetTrackTime(track.Name);
                tracks.AddLast(track);

                track = LoadTrackFile(offlineTracksFolder + "turnsa" + FILE_EXTENSION);
                track.Id = "";
                track.Name = "NSIY-432";
                track.Planet = new Planet("3", "Turnsa", new int[] { 120, 120, 90 });
                track.OfflineTime = trackTimeController.GetTrackTime(track.Name);
                tracks.AddLast(track);

                Console.WriteLine("Loaded offline tracks: ");
                foreach (var offlineTrack in tracks) {
                    Console.WriteLine("\t" + offlineTrack);
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
                    generatedTrack.Id = "";
                    generatedTrack.Name = "Unknown Cave";
                    generatedTrack.Planet = new Planet("0", "Unknown Planet", Settings.COLOR_PRIMARY * 1.25f);

                    var timeController = OfflineTrackTimeController.Instance;
                    timeController.DeleteTime(generatedTrack.Name);
                    
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
