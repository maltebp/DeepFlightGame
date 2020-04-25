using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.control.offlinetracktime {
    
    /// <summary>
    /// Object which maps track times to track names
    /// </summary>
    [Serializable]
    class OfflineTrackTimeMap {

        private List<TrackTime> trackTimes = new List<TrackTime>();

        public long GetTrackTime(string trackName) {
            foreach( var trackTime in trackTimes) {
                if (trackTime.name == trackName)
                    return trackTime.time;
            }
            return -1;
        }

        public void SetTrackTime(string trackName, long trackTime) {
            foreach(var existingTime in trackTimes) {
                if( trackName == existingTime.name) {
                    existingTime.time = trackTime;
                    return ;
                }
            }
            trackTimes.Add( new TrackTime(){ name=trackName, time=trackTime });           
        }

        public void RemoveTrackTime(string trackName) {
            TrackTime timeToRemove = null;
            foreach(var trackTime in trackTimes) {
                if (trackTime.name == trackName)
                    timeToRemove = trackTime;
            }
            trackTimes.Remove(timeToRemove);
        }

        public override string ToString() {
            var trackTimesString = "";
            foreach(var trackTime in trackTimes) {
                if (trackTimesString != "")
                    trackTimesString += ", ";
                trackTimesString += trackTime;
            }
            return string.Format("OfflineTrackTimeMap( trackTimes=({0})", trackTimesString);
        }

        [Serializable]
        private class TrackTime {
            public string name;
            public long time;

            public override string ToString() {
                return string.Format("TrackTime( name={0}, time=time{1} )", name, time);
            }
        }
    }
}
