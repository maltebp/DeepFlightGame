using System;

namespace DeepFlight.network.exceptions {
    public class ConnectionException : APIException {

        public string URL { get; }

        /// <summary>
        /// Creates a new ConnectionException, which should be thrown if connection cannot
        /// be obtained to the server (i.e. timeout)
        /// </summary>
        /// <param name="url">The URL which caused the exception</param>
        public ConnectionException(string url) : base("Cannot get access to server on '"+url+"'") {
            URL = url;
        }
    }
}
