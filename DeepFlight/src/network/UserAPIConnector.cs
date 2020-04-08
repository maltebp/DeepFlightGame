using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.src.network {
    class UserAPIConnector {




        public struct AuthenticationResponse {
        }

        public delegate void AuthenticationCallback(AuthenticationResponse response);
        public void AuthenticateUser(string username, string password, AuthenticationCallback callback) {

        }

    }
}
