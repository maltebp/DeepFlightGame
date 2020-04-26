
using RestSharp;
using System.Threading;
using System.Threading.Tasks;
using DeepFlight.network.exceptions;

namespace DeepFlight.network {
    public class UserAPIConnector {

        private static readonly string URL = "http://maltebp.dk:8000/userapi";

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
                // TODO: Implement correct contact to API
                Thread.Sleep(1000);
                return "thisisatoken";
            });
        }

    }
}
