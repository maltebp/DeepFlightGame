using DeepFlight.user;
using DeepFlight.track;
using System;
using System.Collections.Generic;
using DeepFlight.network.exceptions;
using System.Threading.Tasks;

namespace DeepFlight.network {
    public interface IGameAPIConnector {

        /// <summary>
        /// Get the current Round, including Track metadata, but not round ratings
        /// </summary>
        /// 
        /// <exception cref="ConnectionException"> Connector can't connect to server </exception>
        /// <exception cref="ServerException"> Some unknown error occurs on the server </exception>
        Task<Round> GetCurrentRound();


        /// <summary>
        /// Get the previous Round, including Track metadata for that round, but not 
        /// round ratings
        /// </summary>
        /// 
        /// <exception cref="ConnectionException"> Connector can't connect to server </exception>
        /// <exception cref="ServerException"> Some unknown error occurs on the server </exception>
        Task<Round> GetPreviousRound();


        /// <summary>
        /// Returns the block data of a given Track. Data will not be added automatically
        /// to the Track but returned.
        /// </summary>
        /// <param name="track">The Track to find the block data for (uses only ID)</param>
        /// <returns>The block data as a sequential array of bytes</returns>
        /// 
        /// <exception cref="UnknownTrackException"> Couldn't find track within API </exception>
        /// <exception cref="ConnectionException"> Connector can't connect to server </exception>
        /// <exception cref="ServerException"> Some unknown error occurs on the server </exception>
        Task<byte[]> GetTrackBlockData(Track track);

        /// <summary>
        /// Updates the Users track time on a given Track
        /// </summary>
        /// <param name="user"> The User to upload the time for </param>
        /// <param name="track"> The Track to upload the time for </param>
        /// <returns> True if the new Time was better than existing (and thus the time was updated) </returns>
        /// 
        /// <exception cref="UnknownUserException"> Couldn't find user within API </exception>
        /// <exception cref="UnknownTrackException"> Couldn't find track within API </exception>
        /// <exception cref="ConnectionException"> Connector can't connect to server </exception>
        /// <exception cref="ServerException"> Some unknown error occurs on the server </exception>
        Task<bool> UpdateUserTrackTime(User user, Track track, ulong newTime);


        /// <summary>
        /// Get the Universal Rating of highest rated users
        /// </summary>
        /// <param name="numberOfRatings"> Maximum number of ratings to get </param>
        /// <returns> A list of UserRating structs sorted from best to worts rating</returns>
        /// 
        /// <exception cref="ConnectionException"> Connector can't connect to server </exception>
        /// <exception cref="ServerException"> Some unknown error occurs on the server </exception>
        Task<List<UserRanking>> GetUniversalRankings(int count);
    }

    public struct UserTrackTime {
        public string name;
        public long time;
    }

    public struct UserRanking {
        public string name;
        public int rank;
        public double rating;
    }
}
