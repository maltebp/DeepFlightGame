using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.track {
    public class Planet {

        public string Id { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }

        public Planet() { }


        /* Custom JSON constructor because color
           is in the "color" : [1,2,3] format*/
        [JsonConstructor]
        public Planet(string id, string name, int[] color) {
            Id = id;
            Name = name;
            Color = new Color(color[0], color[1], color[2]);
        }

        public Planet(string id, string name, Color color) {
            Id = id;
            Name = name;
            Color = color;
        }

        public override string ToString() {
            return
                "Planet( " +
                    "id=" + Id + ", " +
                    "name=" + Name + ", " +
                    "color=" + "(" + Color.R + "," + Color.G + "," + Color.B + ")" + 
                " )";
        }
    }
}
