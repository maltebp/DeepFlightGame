using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.track {
    class Round {

        // TODO: Add start and end date


        public int RoundNumber { get; }
        public List<Track> Tracks { get; }


        public Round(int roundNumber, List<Track> tracks) {
            RoundNumber = roundNumber;
            Tracks = tracks;
        }


        public override string ToString() {
            return string.Format("Round( number={0}, tracks=[{1}] )", RoundNumber, string.Join(",", Tracks));
        }

    }
}
