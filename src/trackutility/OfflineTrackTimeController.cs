using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.control.offlinetracktime {


    /// <summary>
    /// Handles the storing and loading of Track times for offline
    /// tracks
    /// </summary>
    class OfflineTrackTimeController {

        // Singleton logic -----------------------------------------
        private static OfflineTrackTimeController instance = null;
        public static OfflineTrackTimeController Instance {
            get{
                if (instance == null)
                    instance = new OfflineTrackTimeController();
                return instance;
            }
        }

        private OfflineTrackTimeController() {
            LoadTrackTimes();
        }
        // ----------------------------------------------------------


        public OfflineTrackTimeMap trackTimes = null;

        
        /// <summary>
        /// Attempts to update the best time for the track with the 
        /// given name. Time is automatically saved to file.
        /// </summary>
        /// <param name="trackName">Name of track (storing id)</param>
        /// <param name="time">Time to store in milliseconds</param>
        /// <returns>True if the new time was better than existing (also true if no time existed)</returns>
        public bool UpdateTrackTime(string trackName, long time) {
            var currentTime = trackTimes.GetTrackTime(trackName);

            if ( currentTime == -1 || time < currentTime ) {
                trackTimes.SetTrackTime(trackName, time);
                SaveTrackTimes();
                return true;
            }

            return false;
        }


        /// <summary>
        /// Returns the current best time for the given track in milliseconds.
        /// </summary>
        /// <param name="trackName">Name of track (storage id)</param>
        /// <returns>Best time in milliseconds, or -1 if no time is stored</returns>
        public long GetTrackTime(string trackName) {
            return trackTimes.GetTrackTime(trackName);
        }

        public void DeleteTime(string trackName) {
            trackTimes.RemoveTrackTime(trackName);
            SaveTrackTimes();
        }


        /// <summary>
        /// Loads track times from file, or create new track times object
        /// if no file exists
        /// </summary>
        private void LoadTrackTimes() {
            var fileExists = File.Exists(GetExecutionDirectory() + Settings.OFFLINE_TRACK_TIMES_FILENAME);
            if( fileExists) {
                var formatter = new BinaryFormatter();
                var stream = new FileStream(Settings.OFFLINE_TRACK_TIMES_FILENAME, FileMode.Open, FileAccess.Read, FileShare.Read);
                trackTimes = (OfflineTrackTimeMap) formatter.Deserialize(stream);
                stream.Close();
            }
            else {
                trackTimes = new OfflineTrackTimeMap();
            }
        }


        /// <summary>
        /// Stores the current trackTimes object to file
        /// </summary>
        private void SaveTrackTimes() {
            if( trackTimes != null) {
                var formatter = new BinaryFormatter();
                var stream = new FileStream(Settings.OFFLINE_TRACK_TIMES_FILENAME, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, trackTimes);
                stream.Close();
            }
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
