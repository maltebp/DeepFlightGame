
namespace DeepFlight.network.exceptions {

    /// <summary>
    /// Signals that an authentication error occured (i.e. credentials
    /// were not correct).
    /// </summary>
    public class AuthenticationException : APIException {

        public AuthenticationException(string msg) : base(msg) { }
    }
}
