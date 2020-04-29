using DeepFlight.track;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


// Serialize into and deserialize a Track object into a byte array
public static class TrackDeserializer {

    public static Task DeserializeBlockDataAsync(this Track track) {
        return Task.Run(()=> {
            track.DeserializeBlockData();
        });
    } 

    /// <summary>
    /// Deserializes the loaded Block data, preparing the Track to
    /// be played / rendered.
    /// </summary>
    public static void DeserializeBlockData(this Track track) {
        if (track.BlockData == null)
            throw new ArgumentNullException("Block data is null for Track ('{0}')", track.Name);

        if (track.BlockDataDeserialized) {
            Console.WriteLine("WARNING: Block data of Track ('{0}') has already been processed!", track.Name);
            return;
        }

        var stopwatch = Stopwatch.StartNew(); 

        using (MemoryStream stream = new MemoryStream(track.BlockData)) {
            using (BinaryReader reader = new BinaryReader(stream)) {

                // Deserialize starting pos and direction
                track.StartX = reader.ReadInt32();
                track.StartY = reader.ReadInt32();
                track.StartRotation = reader.ReadDouble();

                var trackAssembler = new TrackAssembler();

                // Deserialize Blocks
                while (true) {
                    int blockX = reader.ReadInt32();
                    int blockY = reader.ReadInt32();
                    char blockType = reader.ReadChar();

                    if (blockX == 0 && blockY == 0 && blockType == 0)
                        break;

                    trackAssembler.AddBlock(blockX, blockY, (BlockType) blockType);
                }

                trackAssembler.Assemble(track);

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


    private class TrackAssembler {


        private Dictionary<int, Dictionary<int, Chunk>> chunkMap = new Dictionary<int, Dictionary<int, Chunk>>();
        private List<Chunk> chunks = new List<Chunk>();

        public void AddBlock(int x, int y, BlockType type) {
            GetChunk(x, y).AddBlock(x, y, type);
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

        public void Assemble(Track track) {

            // Sequentialize the chunks
            foreach( var row in chunkMap.Values) {
                foreach( var chunk in row.Values) {
                    chunk.Sequentialize();
                }
            }

            track.ChunkMap = chunkMap;
        }
    }
}