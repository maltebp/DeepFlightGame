using DeepFlight.network.exceptions;
using DeepFlight.user;
using DeepFlight.track;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using Newtonsoft.Json;
using Microsoft.CSharp.RuntimeBinder;


namespace DeepFlight.network {

    public class GameAPIConnector {

        private static readonly string TIME_UPDATE_KEY = "verysecurekey1234";
        private static readonly string URL = Settings.GAME_API_URL; 
        private RestClient client;


        public GameAPIConnector() {
            // It doesn't connect to the servere here - only setup 
            // base information for future requests
            client = new RestClient(URL);
        }



        /// <summary>
        /// Get the current Round, including Track metadata, but not round ratings
        /// </summary>
        /// 
        /// <exception cref="ConnectionException"> Connector can't connect to server </exception>
        /// <exception cref="ServerException"> Some unknown error occurs on the server </exception>
        public Task<Round> GetCurrentRound() {
            return Task.Run(() => {

                var request = new RestRequest("round/current", DataFormat.Json);
                var response = client.Get(request);

                // Check connection error
                if (response.ErrorException != null)
                    throw new ConnectionException(client.BaseUrl.ToString());

                // Check if round exists
                if (response.StatusCode == HttpStatusCode.NotFound) {
                    return null;
                }

                // Check for other unhandled status codes
                if (response.StatusCode != HttpStatusCode.OK) {
                    Console.WriteLine("Unhandled status code when fetching current round: " + response.StatusCode);
                    throw new ServerException("Unhandled HTTP status code: " + response.StatusCode);
                }

                return RoundFromJSON(JsonConvert.DeserializeObject(response.Content));
            });
        }




        /// <summary>
        /// Get the previous Round, including Track metadata for that round, but not 
        /// round ratings
        /// </summary>
        /// 
        /// <exception cref="ConnectionException"> Connector can't connect to server </exception>
        /// <exception cref="ServerException"> Some unknown error occurs on the server </exception>
        public Task<Round> GetPreviousRound() {
            return Task.Run(() => {

                var request = new RestRequest("round/previous", DataFormat.Json);
                var response = client.Get(request);

                // Check connection error
                if (response.ErrorException != null)
                    throw new ConnectionException(client.BaseUrl.ToString());

                // Check if round exists
                if( response.StatusCode != HttpStatusCode.NotFound) {
                    return null;
                }

                // Check for other unhandled status codes
                if (response.StatusCode != HttpStatusCode.OK) {
                    Console.WriteLine("Unhandled status code when fetching current round: " + response.StatusCode);
                    throw new ServerException("Unhandled HTTP status code: " + response.StatusCode);
                }

                return RoundFromJSON(JsonConvert.DeserializeObject(response.Content));
            });
        }


        // Builds a Round object from a JSON, by getting the tracks from the API
        private Round RoundFromJSON(dynamic json) {
            Round round = new Round();

            round.Id = json.id;
            round.RoundNumber = json.roundNumber;
            round.StartDate = json.startDate;
            round.EndDate = json.endDate;

            // Create ranking
            try {
                // The rankings json property may not exist, so we surround it
                // with a try catch statement
                if( json.rankings != null) {
                    List<UserRanking> rankings = new List<UserRanking>();
                    int rank = 1;
                    foreach (dynamic rankingJson in json.rankings) {
                        Console.WriteLine(rankingJson);
                        rankings.Add(new UserRanking() { name = rankingJson.username, rank = rank, rating = rankingJson.rating });
                        rank++;
                    }
                }
                
            }catch (RuntimeBinderException e) { }
            
            // Get the tracks for the given round
            List<Track> tracks = new List<Track>();
            foreach (string trackId in json.trackIds) {
                try {
                    tracks.Add(GetTrackSynchronous(trackId));
                } catch (UnknownTrackException e) {
                    // Since the track should exist, we convert it to a ServerException
                    throw new ServerException($"Couldn't find the Round's Track with id '{trackId}' which should exist."); 
                }
            }

            round.Tracks = tracks;
            return round;
        }






        /// <summary>
        /// Get the Universal Rating of highest rated users
        /// </summary>
        /// <param name="numberOfRatings"> Maximum number of ratings to get </param>
        /// <returns> A list of UserRating structs sorted from best to worts rating</returns>
        /// 
        /// <exception cref="ConnectionException"> Connector can't connect to server </exception>
        /// <exception cref="ServerException"> Some unknown error occurs on the server </exception>
        public Task<List<UserRanking>> GetUniversalRankings(int count) {
            return Task.Run(() => {
                var request = new RestRequest($"rankings/universal");
                var response = client.Get(request);

                // Check connection error
                if (response.ErrorException != null)
                    throw new ConnectionException(client.BaseUrl.ToString());

                // Check for other unhandled status codes
                if (response.StatusCode != HttpStatusCode.OK) {
                    Console.WriteLine("Unhandled status code when fetching current round: " + response.StatusCode);
                    throw new ServerException("Unhandled HTTP status code: " + response.StatusCode);
                }

                // Deserialize Rankings
                dynamic rankingsJson = JsonConvert.DeserializeObject(response.Content);
                List<UserRanking> rankings = new List<UserRanking>();
                int rank = 1;
                foreach (dynamic ranking in rankingsJson) {
                    if (rank > count) break;
                    rankings.Add(new UserRanking() { name = ranking.username, rank = rank, rating = ranking.rating });
                    rank++;
                }

                return rankings;
            });
        }



        public Track GetTrackSynchronous(string trackId) {

            var request = new RestRequest($"track/{trackId}");
            var response = client.Get(request);

            // Check connection error
            if (response.ErrorException != null)
                throw new ConnectionException(client.BaseUrl.ToString());

            // Check if resource was found
            if( response.StatusCode == HttpStatusCode.NotFound) {
                throw new UnknownTrackException($"Couldn't find Track with id '{trackId}'");
            }

            // Check for other unhandled status codes
            if (response.StatusCode != HttpStatusCode.OK) {
                Console.WriteLine("Unhandled status code when fetching current round: " + response.StatusCode);
                throw new ServerException("Unhandled HTTP status code: " + response.StatusCode);
            }

            dynamic trackJson = JsonConvert.DeserializeObject(response.Content);

            var track = new Track();
            track.Id = trackJson.id;
            track.Name = trackJson.name;
            
            // Build Times object
            //Dictionary<string, int> timesMap = trackJson.times;
            List<Track.Time> timesList = new List<Track.Time>();
            foreach( var userTime in trackJson.times) {
                timesList.Add(new Track.Time() { username = userTime.username, time = userTime.time });
            }
            track.Times = timesList;

            // Get Track Planet
            string planetId = trackJson.planetId;
            track.Planet = GetPlanetSynchronous(planetId);

            return track;
        }



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
        public Task<byte[]> GetTrackBlockData(Track track) {
            return Task.Run(() => {
                var request = new RestRequest("track/" + track.Id + "/trackdata");

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


        public Task<Planet> GetPlanet(string planetId) {
            return Task.Run(() => { return GetPlanetSynchronous(planetId);  });
        }

        public Planet GetPlanetSynchronous(string planetId) {
            var request = new RestRequest($"planet/{planetId}");
            var response = client.Get(request);

            // Check connection error
            if (response.ErrorException != null)
                throw new ConnectionException(client.BaseUrl.ToString());

            // Check if resource was found
            if (response.StatusCode == HttpStatusCode.NotFound) {
                throw new UnknownTrackException($"Couldn't find Planet with id '{planetId}'");
            }

            // Check for other unhandled status codes
            if (response.StatusCode != HttpStatusCode.OK) {
                Console.WriteLine("Unhandled status code when fetching current round: " + response.StatusCode);
                throw new ServerException("Unhandled HTTP status code: " + response.StatusCode);
            }

            return JsonConvert.DeserializeObject<Planet>(response.Content);
        }



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
        public Task<bool> UpdateUserTrackTime(User user, Track track, ulong newTime) {
            return Task.Run(() => {

                var request = new RestRequest($"track/{track.Id}/times/{user.Username}", DataFormat.Json);
                request.AddHeader("Authorization", $"Bearer {user.Token}");
                request.AddJsonBody(new { updateKey = TIME_UPDATE_KEY, time = newTime });
                var response = client.Post(request);

                // Check connection error
                if (response.ErrorException != null)
                    throw new ConnectionException(client.BaseUrl.ToString());

                // Check token is correct
                if (response.StatusCode == HttpStatusCode.Unauthorized) {
                    throw new AuthenticationException("Update request was unauthorized: " + response.Content);
                }

                // Check for other unhandled status codes
                if (response.StatusCode != HttpStatusCode.OK) {
                    Console.WriteLine("Unhandled status code when fetching current round: " + response.StatusCode);
                    throw new ServerException("Unhandled HTTP status code: " + response.StatusCode);
                }

                dynamic responseJson = JsonConvert.DeserializeObject(response.Content);
                bool newRecord = responseJson.newRecord;
                return newRecord;
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
        public Task<User> GetUserPrivate(String username, string token) {
            return Task.Run( () => {

                var request = new RestRequest($"user/{username}/private", DataFormat.Json);
                request.AddHeader("Authorization", $"Bearer {token}");
                var response = client.Get(request);

                // Check connection error
                if (response.ErrorException != null)
                    throw new ConnectionException(client.BaseUrl.ToString());

                // Check token is correct
                if( response.StatusCode == HttpStatusCode.Unauthorized ) {
                    throw new AuthenticationException("Token is unauthorized");
                }

                // Check for other unhandled status codes
                if (response.StatusCode != HttpStatusCode.OK) {
                    Console.WriteLine("Unhandled status code when fetching current round: " + response.StatusCode);
                    throw new ServerException("Unhandled HTTP status code: " + response.StatusCode);
                }

                Console.WriteLine("Got user: " + response.Content);
                dynamic userData = JsonConvert.DeserializeObject(response.Content);

                var user = new User();
                user.Username = userData.username;
                user.UniversalRank = userData.rank;
                user.UniversalRating = userData.rating;
                user.Token = token;

                return user;
            });
        }
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
