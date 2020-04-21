using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.src.user {

    /// <summary>
    /// Controls the logging in of a User, and maintains information about
    /// the user you're currently logged in as.
    /// </summary>
    public class UserController {

        public static bool LoggedInAsGuest { get; private set; } = false;

        public static void LoginAsGuest() {
            LoggedInAsGuest = true;
        }

        /// <summary>
        /// Logs out the User (also if you're a guest).
        /// </summary>
        public static void Logout() {
            LoggedInAsGuest = false;
        }
    }
}
