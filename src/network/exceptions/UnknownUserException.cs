using DeepFlight.user;

namespace DeepFlight.network.exceptions {

    /// <summary>
    /// Signals that a given User doesn't exist within the API
    /// </summary>
    class UnknownUserException : APIException {
        public User User { get; }

        /// <param name="user"> User which it couldn't locate within the API </param>
        public UnknownUserException(User user) : base(string.Format("Unknown user: {0}", user)) {
            User = user;
        }
    }

}
