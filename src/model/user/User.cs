using System;

namespace DeepFlight.user {

    /// <summary>
    /// Singleton representing the user which is currently playing
    /// </summary>
    public class User {

        /// <summary>
        /// The User object representing the local user
        /// playing the game
        /// </summary>
        public static User LocalUser { get; set; }

        // Whether this user is a guest
        public bool Guest { get; set; } = false;
        public string Username { get; set; } = "Unknown User";
        public double UniversalRating { get; set;  } = 0;
        public int UniversalRank { get; set; }
        public string Token { get; set; }

        /// <summary>
        /// Clears the local user object back to null
        /// </summary>
        public static void ResetLocalUser() {
            LocalUser = null;
        }

    }
}
