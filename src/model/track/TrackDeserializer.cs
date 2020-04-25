



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

    /// <summary>
    /// Deserialize the Track meta data (name, id, length etc.) into a new Track
    /// object. It loads the Block (position and types of blocks), into a byte array
    /// which is referenced by the Track object, but it does NOT deserialize the Block
    /// data.
    /// </summary>
    public static Track DeserializeMetaData(byte[] trackData) {
        Track track = new Track();

        using (MemoryStream stream = new MemoryStream(trackData)) {
            using (BinaryReader reader = new BinaryReader(stream)) {

                // ID
                track.ID = reader.ReadInt64();

                // Seed
                track.Seed = reader.ReadInt64();

                // Name
                string name = "";
                char c;
                while ((c = reader.ReadChar()) != 0) {
                    name += c;
                }
                track.Name = name;

                // Planet
                Planet planet = new Planet();
                track.Planet = planet;

                // Planet ID
                planet.ID = reader.ReadInt64();

                // Planet Name
                planet.Name = "";
                while ((c = reader.ReadChar()) != 0) {
                    planet.Name += c;
                }

                // Planet Color
                byte red = reader.ReadByte();
                byte green = reader.ReadByte();
                byte blue = reader.ReadByte();
                planet.Color = new Color(red, green, blue);

                // Length
                track.Length = reader.ReadInt32();

                // Starting Position
                track.StartX = reader.ReadInt32();
                track.StartY = reader.ReadInt32();

                // starting rotation
                track.StartRotation = reader.ReadDouble();

                // Read Block Data size
                track.BlockDataSize = reader.ReadUInt32();

                // Read remaining data (blocks and checkpoints)
                byte[] blockData = new byte[track.BlockDataSize];
                uint byteCount = 0;
                while(stream.Position != stream.Length) {
                    blockData[byteCount] = reader.ReadByte();
                    byteCount++;
                }

                if( byteCount != track.BlockDataSize) {
                    Console.WriteLine("WARNING: Number of bytes read ({0}) does not match expected ({1})", byteCount);
                }

                track.BlockData = blockData;
                track.BlockDataDeserialized = false;
            }
        }

        return track;
    }


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

                var brigtenColor = Settings.TRACK_COLOR_ADJUST_TRACK;
                var planetColor = track.Planet.Color;
                var trackColor = new Color(planetColor.R+brigtenColor, planetColor.G+brigtenColor, planetColor.B+brigtenColor);

                // Deserialize Blocks
                while (true) {
                    int blockX = reader.ReadInt32();
                    int blockY = reader.ReadInt32();
                    char blockType = reader.ReadChar();

                    if (blockX == 0 && blockY == 0 && blockType == 0)
                        break;

                    // Console.WriteLine("Read block: x={0}, y={1}, type={2}", blockX, blockY, blockType);

                    track.SetBlock((BlockType)blockType, blockX, blockY);
                    track.GetBlock(blockX, blockY).Color = trackColor;
                }
                 
                // Deserialize Checkpoints
                LinkedList<Checkpoint> checkpoints = new LinkedList<Checkpoint>();
                int checkPointIndex = 0;
                while (stream.Position != stream.Length) {
                    Checkpoint checkpoint = new Checkpoint(checkPointIndex++, new Color(Color.White, 0.5f), reader.ReadInt32(), reader.ReadInt32());
                    //Console.WriteLine("Read checkpoint: x={0}, y={1} ", checkpoint.X, checkpoint.Y);
                    checkpoints.AddLast(checkpoint);
                }

                track.Checkpoints = checkpoints.ToArray();
            }
        }
        track.BlockDataDeserialized = true;
        stopwatch.Stop();
        Console.WriteLine("Deserialization of Block data for Track '{0}' took {1:N3} seconds", track.Name, stopwatch.Elapsed.TotalSeconds);
    }

}