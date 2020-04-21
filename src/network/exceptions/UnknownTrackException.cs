namespace DeepFlight.network.exceptions {
    
    /// <summary>
    /// Signals that a given Track doesn't exist within the API
    /// </summary>
    class UnknownTrackException : APIException {
        public Track Track { get; }
        
        /// <param name="track"> Track which it couldn't locate within the API </param>
        public UnknownTrackException(Track track) : base(string.Format("Unknown track: {0}", track)) {
            Track = track;
        }
    }

}
