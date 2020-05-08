using DeepFlight.network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.track {
    public class Round {

        // TODO: Add start and end date

        public string Id { get; set; }
        public int RoundNumber { get; set; }
        public List<Track> Tracks { get; set; }
        public long StartDate { get; set;  }
        public long EndDate { get; set;  }
        public List<UserRanking> Rankings { get; set; }

        public override string ToString() {
            return string.Format("Round( number={0}, tracks=[{1}] )", RoundNumber, string.Join(",", Tracks));
        }

    }
}
