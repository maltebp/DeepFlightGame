using DeepFlight.network;
using System.Collections.Generic;

namespace DeepFlight.track {
    
    public class Round {

        public string Id { get; set; }
        public int RoundNumber { get; set; }
        public List<Track> Tracks { get; set; }
        public long StartDate { get; set;  }
        public long EndDate { get; set;  }
        public List<UserRanking> Rankings { get; set; }

        public override string ToString() {
            return string.Format("Round( number={0}, tracks=[{1}] )", RoundNumber, string.Join(",", Tracks));
        }

        public UserRanking GetUserRanking(string username) {
            if (Rankings == null) return new UserRanking() { name = "", rating = 0, rank = 0 };
            foreach (var userRanking in Rankings) {
                if (userRanking.name == username)
                    return userRanking;
            }
            return new UserRanking() { name="", rating=0, rank=0};
        }

    }

}
