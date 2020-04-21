using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.src {
    class RestTest {
    
        public static void Test() {

            var client = new RestClient("http://localhost:10000/gameapi");


            var request = new RestRequest("track/1/blockdata", DataFormat.Json);

            var response = client.Get(request);

            Console.WriteLine("Status: " + response.StatusCode);
            //Console.WriteLine("Response: " + response.Content);

            byte[] data = response.RawBytes;

            Console.WriteLine("Response length: " + data.Length);

        }
    
    }
}
