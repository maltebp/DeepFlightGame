using System;

namespace DeepFlight.network.exceptions {
    /// <summary>
    /// Generic exception describing some API exception occuring
    /// while interacting with API
    /// </summary>
    public class APIException : Exception {
        public APIException() : base() { }
        public APIException(string msg) : base(msg) { }
    }
}
