using DeepFlight.network.exceptions;
using DeepFlight.src.user;
using DeepFlight.track;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DeepFlight.network {

    public class GameAPIConnector : IGameAPIConnector {

        private static readonly string URL = "http://localhost:10000/gameapi";

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
                    Console.WriteLine("Unhandled status code when loading current round: " + response.StatusCode);
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
                var response = client.Get(request);

                // Check connection error
                if (response.ErrorException != null)
                    throw new ConnectionException(client.BaseUrl.ToString());

                // Check for other unhandled status codes
                if (response.StatusCode != HttpStatusCode.OK) {
                    Console.WriteLine("Unhandled status code when loading current round: " + response.StatusCode);
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
            throw new System.NotImplementedException();
        }


        Task<byte[]> IGameAPIConnector.GetTrackBlockData(Track track) {
            throw new System.NotImplementedException();
        }
    }
}
