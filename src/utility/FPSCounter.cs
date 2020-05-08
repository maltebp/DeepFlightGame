using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DeepFlight.utility {

    // Tracks the Frames Per Second, by averaging previous update
    // time differences
    public class FPSCounter {

        private const int SAMPLE_SIZE = 20;
        
        private Queue<double> samples = new Queue<double>();

        /// <summary>
        /// Signals that a new frame has been drawn
        /// </summary>
        /// <param name="gameTime"> Time since last updated frame </param>
        public void Update(GameTime gameTime) {
            samples.Enqueue(gameTime.ElapsedGameTime.TotalSeconds);
            if (samples.Count > SAMPLE_SIZE)
                samples.Dequeue();
        }

        /// <summary>
        /// Get the average FPS
        /// </summary>
        public double GetFPS() {
            double sum = 0;
            foreach (double time in samples) {
                sum += time;
            }
            return samples.Count / sum;
        }


    }
}
