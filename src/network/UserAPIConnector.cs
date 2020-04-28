
using RestSharp;
using System.Threading;
using System.Threading.Tasks;
using DeepFlight.network.exceptions;
using System.Net;
using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace DeepFlight.network {
    public class UserAPIConnector {

        private static readonly string URL = "http://maltebp.dk:7000/";

        private RestClient client;

        public UserAPIConnector() {
            // It doesn't connect to the servere here - only setup 
            // base information for future requests
            client = new RestClient(URL);
        }


        /// <summary>
        /// Authenticates the login credentials at the UserAPI, and respond
        /// with an authentication token
        /// </summary>
        /// 
        /// <exception cref="AuthenticationException">Credentials were not correct</exception>
        /// <exception cref="ConnectionException"> Connector can't connect to server </exception>
        /// <exception cref="ServerException"> Some unknown error occurs on the server </exception>
        public Task<string> AuthenticateUser(string username, string password) {
            return Task.Run(() => {

                var request = new RestRequest("login", DataFormat.Json);
                request.AddParameter("application/x-www-form-urlencoded", $"name={username}&password={password}", ParameterType.RequestBody);
                var response = client.Post(request);
                
                // Check connection error
                if (response.ErrorException != null) {
                    var e = new ConnectionException(client.BaseUrl.ToString());
                    Trace.TraceError("Connection error occured during AuthenticateUser: " + e);
                    throw e;
                }

                if( response.StatusCode == HttpStatusCode.Unauthorized) {
                    throw new AuthenticationException("Incorrect username or password");
                }

                // Check for other unhandled status codes
                if (response.StatusCode != HttpStatusCode.OK) {
                    var e = new ServerException("Unhandled HTTP status code: " + response.StatusCode);
                    Trace.TraceError("Server exception when calling AuthenticateUser: " );
                    throw e;
                }

                // Parse JSON response
                string token = "";
                try {
                    dynamic jsonResponse = JsonConvert.DeserializeObject(response.Content);
                    token = jsonResponse.jwt; // This matches the object name in the json response
                }catch(JsonReaderException e) {
                    Trace.TraceError("Couldn't parse json: " + e);
                    throw new ServerException("Couldn't parse JSON response from server");
                }

                return token;
            });
        }

    }
}
