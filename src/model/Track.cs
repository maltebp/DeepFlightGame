using DeepFlight.track;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


public class Track {

    public string     Id { get; set; }
    public string   Name { get; set; }
    public Planet Planet { get; set; }
    public List<Time> Times { get; set; }

    // The local time record on the Track
    public long OfflineTime { get; set; } = 0;

    // The Raw (unserialized) Track information
    public byte[]   BlockData { get; set; }

    // Whether or not the BlockDataDeserialized
    public bool BlockDataDeserialized { get; set; }

    // Data obtained from deserializing BlockData
    public int StartX { get; set; }
    public int StartY { get; set; }
    public double StartRotation { get; set; }
    public Checkpoint[] Checkpoints { get; set; }
    public Dictionary<int, Dictionary<int, Chunk>> ChunkMap { get; set; }


    public override string ToString() {
        return
            "Track( " +
                "name=" + Name + ", "+    
                "id=" + Id + ", "+    
                "planet=" + Planet.Name + " (id=" + Planet.Id + "), " +
                "startPos=(" + StartX + "," + StartY + "), " +
                "startRot=" + string.Format("{0:N2}", StartRotation) + ", " +
                "processed=" + BlockDataDeserialized +
            " )"
        ;
    }


    public delegate void ChunkCallback(Chunk chunk);

    /// <summary>
    /// Calls the given callback for each chunk which contains any
    /// world coordinates within the given range.
    /// </summary>
    public void ForChunksInRange(int minX, int maxX, int minY, int maxY, ChunkCallback callback) {
        int chunkMinX = Chunk.ToChunkCoordinate(minX);
        int chunkMaxX = Chunk.ToChunkCoordinate(maxX);
        int chunkMinY = Chunk.ToChunkCoordinate(minY);
        int chunkMaxY = Chunk.ToChunkCoordinate(maxY);
        for (int x = chunkMinX; x <= chunkMaxX; x++) {
            if (ChunkMap.ContainsKey(x)) {
                var row = ChunkMap[x];
                for (int y = chunkMinY; y <= chunkMaxY; y++) {
                    if (row.ContainsKey(y)) {
                        callback(row[y]);
                    }
                }
            }
        }
    }

    // Represents a User's time on some Track
    public struct Time {
        public string username;
        public int time; // Time in milliseconds
    }
}


/// <summary>
/// The chunk represents a part of the blocks within the Track
/// The blocks added to the chunk may be "built", such that the
/// blocks are saved within a drawable texture, which allows for
/// faster rendering of the chink.
/// </summary>
public class Chunk {

    // The block dimensions of the Chunk (10x10 chunk contains 100 blocks)
    public static readonly int SIZE = Settings.TRACK_CHUNK_SIZE;

    // CHUNK coordinates of the block
    public int X { get; private set; }
    public int Y { get; private set; }

    public List<CollisionBlock> CollisionBlocks = new List<CollisionBlock>();

    // List of "unbuilt" blocks (blocks which are added, but Build() has been called yet)
    private List<Block> buildingBlocks = new List<Block>();
    
    // Whether or not the blocks within the 'buildingBlocks' has been built
    public bool Ready { get; private set; }

    // The block texture, built from the 'buildingBlocks' list
    public Texture2D Texture { get; private set; }

    public Chunk(int x, int y) {
        X = x;
        Y = y;
    }


    /// <summary>
    /// Add a new block the Chunk. The block will not be drawable
    /// by the chunk until 'Build()' is called.
    /// </summary>
    public void AddBlock(int x, int y) {
        buildingBlocks.Add(new Block() { x = x, y = y });
        Ready = false; 
    }


    /// <summary>
    /// Adds a collision block to the chunk. These blocks don't need
    /// to be built.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void AddCollisionBlock(int x, int y) {
        CollisionBlocks.Add(new CollisionBlock(x, y));
    }


    /// <summary>
    /// Build's the Chunk, by creating the rendering Texture
    /// which is drawn to the screen, when drawing the Chunk.
    /// </summary>
    public void Build() {

        /*
         * Visual demonstration of how chunk coordinates are laid
         * out in one axis (it's the same on the X and Y)
         * 
         *                    Chunk -1          Chunk 0
         *                ----------------- -----------------
         *                                -   +
         *  Coordinate    9 8 7 6 5 4 3 2 1 0 1 2 3 4 5 6 7 8 
         
         *  Chunk Array:  0 1 2 3 4 5 6 7 8 0 1 2 3 4 5 6 7 8
         * 
         */
        var texture = Textures.CreateTexture(Chunk.SIZE, Chunk.SIZE);
        var data = new Color[Chunk.SIZE * Chunk.SIZE];

        for (int i = 0; i < Chunk.SIZE * Chunk.SIZE; i++)
            data[i] = Color.Transparent;

        foreach (var block in buildingBlocks) {
            int normalizedX = MathExtension.Mod(block.x, SIZE);
            int normalizedY = MathExtension.Mod(block.y, SIZE);
            data[normalizedX + normalizedY * SIZE] = Color.White;
    
        }

        texture.SetData(data);
        Texture = texture;
        Ready = true;
    }


    public static int ToChunkCoordinate(int coordinate) {
        return coordinate < 0 ? ((coordinate+1) / Chunk.SIZE) - 1 : (coordinate / Chunk.SIZE);
    }


    /// <summary>
    /// Just a struct to represent a Block when building the chunk
    /// </summary>
    private struct Block {
        public int x, y;
    }

}


/// <summary>
/// Block with a collider (so it's collidable).
/// Used to the track blocks, for wall collision detection
/// </summary>
public class CollisionBlock : Collidable {
    public CollisionBlock(int x, int y) : base(1f, 1f) {
        this.X = x;
        this.Y = y;
        AddCollider(new RectCollider(this));
    }
}
