using DeepFlight.network.exceptions;
using DeepFlight.user;
using DeepFlight.track;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace DeepFlight.network {

    public class GameAPIConnector : IGameAPIConnector {

        private static readonly string URL = "http://maltebp.dk:10000/gameapi";

        private RestClient client;

        public GameAPIConnector() {
            // It doesn't connect to the servere here - only setup 
            // base information for future requests
            client = new RestClient(URL);
        }



        public Task<Round> GetCurrentRound() {
            return Task.Run(() => {

                var request = new RestRequest("round/current", DataFormat.Json);
                var response = client.Get(request);

                // Check connection error
                if (response.ErrorException != null)
                    throw new ConnectionException(client.BaseUrl.ToString());

                // Check for other unhandled status codes
                if (response.StatusCode != HttpStatusCode.OK) {
                    Console.WriteLine("Unhandled status code when fetching current round: " + response.StatusCode);
                    throw new ServerException("Unhandled HTTP status code: " + response.StatusCode);
                }

                var round = JsonConvert.DeserializeObject<Round>(response.Content);

                return round;
            });
        }



        public Task<Round> GetPreviousRound() {
            return Task.Run(() => {
                return (Round) null;
            });
        }


        public Task<List<UserRanking>> GetRoundRatings(Round round, int count) {
            return Task.Run(() => {
                // TODO: Implement correct api!!

                List<UserRanking> ratings = new List<UserRanking>();
                ratings.Add(new UserRanking() { name = "s123456", rating = 3.30 });
                ratings.Add(new UserRanking() { name = "s185139", rating = 3.12 });
                ratings.Add(new UserRanking() { name = "s234902", rating = 2.90 });
                ratings.Add(new UserRanking() { name = "s249358", rating = 2.54 });
                ratings.Add(new UserRanking() { name = "s234009", rating = 2.40 });
                ratings.Add(new UserRanking() { name = "s124639", rating = 2.35 });
                ratings.Add(new UserRanking() { name = "s143964", rating = 1.99 });

                Thread.Sleep(2000);
                return ratings;
            });
        }


        public Task<byte[]> GetTrackBlockData(Track track) {
            return Task.Run(() => {
                var request = new RestRequest("track/" +track.ID + "/blockdata");

                Console.WriteLine("\nClient timeout: " + client.Timeout);
                Console.WriteLine("Request timeout: " + request.Timeout);

                var response = client.Get(request);

                // Check connection error
                if (response.ErrorException != null)
                    throw new ConnectionException(client.BaseUrl.ToString());

                // Check for other unhandled status codes
                if (response.StatusCode != HttpStatusCode.OK) {
                    var e = new ServerException("Unhandled HTTP status code when fetching track data: " + response.StatusCode);
                    Trace.TraceError("\nServer side error during login: " + e);
                    throw e;
                }

                return response.RawBytes;
            });
        }


        public Task<List<UserRanking>> GetUniversalRatings(int count) {
            return Task.Run(() => {
                // TODO: Implement correct api!!

                List<UserRanking> ratings = new List<UserRanking>();
                ratings.Add(new UserRanking() { name = "s185139", rating = 3.59 });
                ratings.Add(new UserRanking() { name = "s123456", rating = 3.12 });
                ratings.Add(new UserRanking() { name = "s249358", rating = 2.80 });
                ratings.Add(new UserRanking() { name = "s234902", rating = 2.23 });
                ratings.Add(new UserRanking() { name = "s124639", rating = 1.50 });
                ratings.Add(new UserRanking() { name = "s143964", rating = 1.26 });
                ratings.Add(new UserRanking() { name = "s234009", rating = 1.24 });

                Thread.Sleep(2000);
                return ratings;
            });
        }




        public Task<List<UserTrackTime>> GetTrackTimes(Track track, int count) {
            throw new System.NotImplementedException();
        }
        

        public Task<ulong> GetUserTrackTime(User user, Track track) {
            throw new System.NotImplementedException();
        }


        public Task<bool> UpdateUserTrackTime(User user, Track track, ulong newTime) {
            return Task.Run(() => {
                // TODO: Implement track time updating here!
                Thread.Sleep(2000);
                return true;
            });
        }

        public Task<UserRanking> GetUserUniversalRanking(User user) {
            return Task.Run(() => {
                return new UserRanking() { name = user.Username, rank = 101, rating = 1.99};
            });
        }

        public Task<UserRanking> GetUserRoundRanking(User user, Round round) {
            return Task.Run(() => {
                return new UserRanking() { name = user.Username, rank = 120, rating = 1.23 };
            });
        }



        /// <summary>
        /// Authentiates the user at the GameAPI, using a token generated by the 
        /// UserAPI.
        /// </summary>
        /// <param name="token"> Token recieved from UserAPI </param>
        /// <returns> User object containing information, including private information </returns>
        /// 
        /// <exception cref="AuthenticationException">Credentials were not correct</exception>
        /// <exception cref="ConnectionException"> Connector can't connect to server </exception>
        /// <exception cref="ServerException"> Some unknown error occurs on the server </exception>
        public Task<User> AuthenticateUser(string token) {
            return Task.Run( () => {

                Thread.Sleep(1000);

                var testUser = new User();
                testUser.Username = "Test User";
                testUser.UniversalRank = 11;
                testUser.UniversalRating = 1.37;

                return testUser;
            });
        }
    }
}
