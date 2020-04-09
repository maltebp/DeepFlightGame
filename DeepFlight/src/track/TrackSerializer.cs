



using System;
using System.Diagnostics;
using System.IO;


// Serialize into and deserialize a Track object into a byte array
class TrackSerializer {

    private const string FILE_TYPE = ".dft";

    public static byte[] Serialize(Track track) {

        Console.WriteLine("\nStarting Track serializing...");
        Stopwatch stopwatch = Stopwatch.StartNew();
        int blockCount = 0;

        using (MemoryStream stream = new MemoryStream()) {
            using (BinaryWriter writer = new BinaryWriter(stream)) {
                track.ForAllBlocks((block, x, y) => {
                    blockCount++;
                    writer.Write((byte)block.Type);
                    writer.Write(x);
                    writer.Write(y);
                    // note sure why this cast is necessary
                });
            }

            byte[] data = stream.ToArray();
            stopwatch.Stop();

            Console.WriteLine("Serialized track");
            Console.WriteLine("Time: {0} ms", stopwatch.ElapsedMilliseconds);
            Console.WriteLine("Blocks: {0}", blockCount);
            Console.WriteLine("Bytes: {0}", data.Length);

            return data;
        }
    }


    public static Track Deserialize(byte[] trackData) {

        Console.WriteLine("\nStarting Track deserialization...");
        Stopwatch stopwatch = Stopwatch.StartNew();

        Track track = new Track();
        using (MemoryStream stream = new MemoryStream(trackData)) {
            using (BinaryReader reader = new BinaryReader(stream)) {
                while (stream.Position != stream.Length) {
                    track.SetBlock((BlockType)reader.ReadByte(), reader.ReadInt32(), reader.ReadInt32());
                }
            }
        }

        stopwatch.Stop(); 

        Console.WriteLine("Deserialized track:");
        Console.WriteLine("Time: {0} ms", stopwatch.ElapsedMilliseconds);
        Console.WriteLine("Blocks: {0}", track.BlockCount);

        return track;
    }


    //    int blockCount = 0;

    //    using (MemoryStream stream = new MemoryStream()) {
    //        using (BinaryWriter writer = new BinaryWriter(stream)) {
    //            track.ForAllBlocks((block, x, y) => {
    //                blockCount++;
    //                writer.Write(x);
    //                writer.Write(y);
    //                // note sure why this cast is necessary
    //                writer.Write((byte)block.Type);
    //            });
    //        }

    //        byte[] data = stream.ToArray();
    //        stopwatch.Stop();

    //        Console.WriteLine("Serialized track");
    //        Console.WriteLine("Time: {0} ms", stopwatch.ElapsedMilliseconds);
    //        Console.WriteLine("Blocks: {0}", blockCount);
    //        Console.WriteLine("Bytes: {0}", data.Length);

    //        return data;
    //    }
    //}




    //public static void SaveTrack(Track track, string name) {
    //    System.Console.WriteLine("Saving track {0}", name);
    //    Stopwatch stopwatch = Stopwatch.StartNew();

    //    using (MemoryStream m = new MemoryStream()) {
    //        using (BinaryWriter writer = new BinaryWriter(m)) {
    //            writer.Write(Id);
    //            writer.Write(Name);
    //        }
    //        return m.ToArray();
    //    }


    //    stopwatch.Stop();
    //    System.Console.WriteLine("Saved track {0}", name);
    //    System.Console.WriteLine("Time: {0}", stopwatch);
    //}

}