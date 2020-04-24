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


        public Task<SortedDictionary<string, int>> GetRoundRatings(Round round, int count) {
            throw new System.NotImplementedException();
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


        public Task<SortedDictionary<string, long>> GetUniversalRatings(int count) {
            throw new System.NotImplementedException();
        }


        public Task<SortedDictionary<string, long>> GetTrackTimes(Track track, int count) {
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


        Task<byte[]> IGameAPIConnector.GetTrackBlockData(Track track) {
            throw new System.NotImplementedException();
        }
    }
}
