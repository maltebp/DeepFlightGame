using DeepFlight.track;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.src.network {

    class GameAPIConnector {

        private static readonly string URL = "http://localhost:10000/gameapi";

        private RestClient client;

        public GameAPIConnector() {
            // It doesn't connect to the servere here - only setup 
            // base information for future requests
            client = new RestClient(URL);
        }


        // Deserialize the current round
        public Task<Round> GetCurrentRound() {      
            return Task.Run(() => {

                var request = new RestRequest("round/current", DataFormat.Json);
                var response = client.Get(request);

                if( response.StatusCode != HttpStatusCode.OK ) {
                    // TODO: Implement error handling 
                    return null;
                }

                var round = JsonConvert.DeserializeObject<Round>(response.Content);

                return round;
            });
        }


        public Task GetTrackBlockData(Track track) {
            return Task.Run(() => {
                var request = new RestRequest("track/" +track.ID + "/blockdata");
                var response = client.Get(request);

                if (response.StatusCode != HttpStatusCode.OK) {
                    // TODO: Implement error handling 
                    return;
                }

                var blockData = response.RawBytes;
                track.BlockData = blockData;
            });
        }



        //public class RoundJsonConverter : JsonConverter {

        //}


        //public class TemperatureConverter : JsonConverter<Temperature> {
        //    public override Temperature Read(
        //        ref Utf8JsonReader reader,
        //        Type typeToConvert,
        //        JsonSerializerOptions options) =>
        //            Temperature.Parse(reader.GetString());

        //    public override void Write(
        //        Utf8JsonWriter writer,
        //        Temperature temperature,
        //        JsonSerializerOptions options) =>
        //            writer.WriteStringValue(temperature.ToString());
        //}
    }
}
