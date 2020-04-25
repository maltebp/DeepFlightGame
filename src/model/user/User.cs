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
        private static User localUser = null;
        public static User LocalUser {
            get {
                if (localUser == null)
                    localUser = new User();
                return localUser;
            }
        }

        // Whether this user is a guest
        public bool Guest { get; set; } = true;
        public string Username { get; set; } = null;
        public int Rating { get; set;  } = 0;

        /// <summary>
        /// Clears the local user object back to null
        /// </summary>
        public static void ResetLocalUser() {
            localUser = null;
        }

    }
}
