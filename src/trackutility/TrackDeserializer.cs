using DeepFlight.track;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


/// <summary>
/// Static class containing helper methods to Deserialize from and Serialize to a Track from the .dftbd format.
/// </summary>
public static class TrackDeserializer {


    /// <summary>
    /// Just calls the DeserializeBlockData as a Task, allowing
    /// to deserialize in an asynchronous manner.
    /// </summary>
    public static Task DeserializeBlockDataAsync(this Track track) {
        return Task.Run(()=> {
            track.DeserializeBlockData();
        });
    } 


    /// <summary>
    /// Deserializes the loaded Block data, preparing the Track to
    /// be played / rendered.
    /// The deserialization happens a manual manner, meaning we use
    /// a stream to read each set of bytes and map them to the expected
    /// attributes of the class manually.
    /// </summary>
    public static void DeserializeBlockData(this Track track) {
        if (track.BlockData == null)
            throw new ArgumentNullException("Block data is null for Track ('{0}')", track.Name);

        if (track.BlockDataDeserialized) {
            Trace.TraceWarning($"Block data of Track(track.Name) has already been processed!");
            return;
        }

        // Take time because it's interesting to now
        var stopwatch = Stopwatch.StartNew(); 

        // Perform the deserialization
        using (MemoryStream stream = new MemoryStream(track.BlockData)) {
            using (BinaryReader reader = new BinaryReader(stream)) {

                // Deserialize starting pos and direction
                track.StartX = reader.ReadInt32();
                track.StartY = reader.ReadInt32();
                track.StartRotation = reader.ReadDouble();

                var trackBuilder = new TrackBuilder();

                // Deserialize Blocks (this is the bulk of the file)
                while (true) {
                    int blockX = reader.ReadInt32();
                    int blockY = reader.ReadInt32();
                    char blockType = reader.ReadChar();

                    if (blockX == 0 && blockY == 0 && blockType == 0)
                        break;

                    if (blockType == 2) {
                        trackBuilder.AddCollisionBlock(blockX, blockY);
                    }
                    else {
                        trackBuilder.AddBlock(blockX, blockY);
                    }
                }

                trackBuilder.Build(track);

                // Deserialize Checkpoints
                LinkedList<Checkpoint> checkpoints = new LinkedList<Checkpoint>();
                int checkPointIndex = 0;
                while (stream.Position != stream.Length) {
                    Checkpoint checkpoint = new Checkpoint(checkPointIndex++, new Color(track.Planet.Color, 0.5f), reader.ReadInt32(), reader.ReadInt32());
                    checkpoints.AddLast(checkpoint);
                }
                track.Checkpoints = checkpoints.ToArray();
            }
        }
        track.BlockDataDeserialized = true;
        stopwatch.Stop();
        Console.WriteLine("Deserialization of Block data for Track '{0}' took {1:N3} seconds", track.Name, stopwatch.Elapsed.TotalSeconds);
    }


    /// <summary>
    /// Class which provides some helper methods, in order to build the Track
    /// when we're loading it in.
    /// </summary>
    private class TrackBuilder {

        private Dictionary<int, Dictionary<int, Chunk>> chunkMap = new Dictionary<int, Dictionary<int, Chunk>>();
        private List<Chunk> chunks = new List<Chunk>();


        public void AddBlock(int x, int y) {
            GetChunk(x, y).AddBlock(x, y);
        }

        public void AddCollisionBlock(int x, int y) {
            GetChunk(x, y).AddCollisionBlock(x, y);
        }


        private Chunk GetChunk(int x, int y) {
            int chunkX = Chunk.ToChunkCoordinate(x);
            int chunkY = Chunk.ToChunkCoordinate(y);

            if( !chunkMap.ContainsKey(chunkX) ) {
                chunkMap.Add(chunkX, new Dictionary<int, Chunk>());
            }
            var row = chunkMap[chunkX];

            if( !row.ContainsKey(chunkY) ) {
                row.Add(chunkY, new Chunk(chunkX, chunkY));
            }

            return row[chunkY];
        }

        /// <summary>
        /// Builds the current chunks/blocks and add them
        /// to the given Track.
        /// </summary>
        /// <param name="track"></param>
        public void Build(Track track) {

            // Sequentialize the chunks
            foreach( var row in chunkMap.Values) {
                foreach( var chunk in row.Values) {
                    chunk.Build();
                }
            }

            track.ChunkMap = chunkMap;
        }
    }
}