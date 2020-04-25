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
            throw new System.NotImplementedException();
        }


        public Task<List<UserRating>> GetRoundRatings(Round round, int count) {
            return Task.Run(() => {
                // TODO: Implement correct api!!

                List<UserRating> ratings = new List<UserRating>();
                ratings.Add(new UserRating() { name = "s123456", rating = 3.30 });
                ratings.Add(new UserRating() { name = "s185139", rating = 3.12 });
                ratings.Add(new UserRating() { name = "s234902", rating = 2.90 });
                ratings.Add(new UserRating() { name = "s249358", rating = 2.54 });
                ratings.Add(new UserRating() { name = "s234009", rating = 2.40 });
                ratings.Add(new UserRating() { name = "s124639", rating = 2.35 });
                ratings.Add(new UserRating() { name = "s143964", rating = 1.99 });

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
                    Console.WriteLine("Unhandled status code when fetching Track data: " + response.StatusCode);
                    throw new ServerException("Unhandled HTTP status code: " + response.StatusCode);
                }

                return response.RawBytes;
            });
        }


        public Task<List<UserRating>> GetUniversalRatings(int count) {
            return Task.Run(() => {
                // TODO: Implement correct api!!

                List<UserRating> ratings = new List<UserRating>();
                ratings.Add(new UserRating() { name = "s185139", rating = 3.59 });
                ratings.Add(new UserRating() { name = "s123456", rating = 3.12 });
                ratings.Add(new UserRating() { name = "s249358", rating = 2.80 });
                ratings.Add(new UserRating() { name = "s234902", rating = 2.23 });
                ratings.Add(new UserRating() { name = "s124639", rating = 1.50 });
                ratings.Add(new UserRating() { name = "s143964", rating = 1.26 });
                ratings.Add(new UserRating() { name = "s234009", rating = 1.24 });

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

        public Task<double> GetUserUniversalRating(User user) {
            throw new NotImplementedException();
        }

        public Task<double> GetUserRoundRating(User user, Round round) {
            throw new NotImplementedException();
        }
    }
}
