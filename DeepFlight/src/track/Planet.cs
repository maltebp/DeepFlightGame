using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFlight.track {
    public class Planet {

        public long ID { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }


        public override string ToString() {
            return
                "Planet( " +
                    "id=" + ID + ", " +
                    "name=" + Name + ", " +
                    "color=" + "(" + Color.R + "," + Color.G + "," + Color.B + ")" + 
                " )";
        }
    }
}
