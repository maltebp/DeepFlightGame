using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

class FPSCounter {
    private const int SAMPLE_SIZE = 10;

    private Queue<double> samples = new Queue<double>();
 
    public void Update(GameTime gameTime) {
        samples.Enqueue(gameTime.ElapsedGameTime.TotalSeconds);
        if (samples.Count > SAMPLE_SIZE)
            samples.Dequeue();
    }

    public double GetFPS() {
        double sum = 0;
        foreach(double time in samples){
            sum += time;
        }
        return samples.Count / sum;
    }


}
