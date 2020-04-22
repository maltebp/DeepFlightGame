

namespace DeepFlight.network.exceptions {

    /// <summary>
    /// Generic exception to throw if some unknown exception
    /// occured on the server side
    /// </summary>
    public class ServerException : APIException {

        public string Response { get; }

        public ServerException() : base("An unknown exception occured on the server") { }

        /// <param name="responseMsg">Message obtained from the server</param>
        public ServerException(string response) : this() {
            Response = response;
        }
    }
}
