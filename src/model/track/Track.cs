using DeepFlight.track;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public delegate void BlockCallback(Block type, int x, int y);


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
    public void ForChunkInRange(int minX, int maxX, int minY, int maxY, ChunkCallback callback) {
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

    public struct Time {
        public string username;
        public int time; // Time in milliseconds
    }
}


public class Chunk {
    public static readonly int SIZE = Settings.TRACK_CHUNK_SIZE;
    public int X { get; private set; }
    public int Y { get; private set; }

    public List<CollisionBlock> CollisionBlocks = new List<CollisionBlock>();

    public List<Block> bufferedBlocks = new List<Block>();

    public bool Ready { get; private set; }

    public Texture2D Texture { get; private set; }

    public Chunk(int x, int y) {
        X = x;
        Y = y;
    }


    public void AddBlock(int x, int y, BlockType type) {
        bufferedBlocks.Add(new Block(x,y,type));
        Ready = false; 
    }

    public void Sequentialize() {


        /*
         * *                Chunk -1          Chunk 0
         *                ----------------- -----------------
         *                                -   +
         *  Coordinate    9 8 7 6 5 4 3 2 1 0 1 2 3 4 5 6 7 8 
         
         *  Chunk Array:  0 1 2 3 4 5 6 7 8 0 1 2 3 4 5 6 7 8
         * 
         */

        // SQUARE (custom texture)
        var texture = Textures.CreateTexture(Chunk.SIZE, Chunk.SIZE);
        var data = new Color[Chunk.SIZE * Chunk.SIZE];

        for (int i = 0; i < Chunk.SIZE * Chunk.SIZE; i++)
            data[i] = Color.Transparent;

        foreach (var block in bufferedBlocks) {
            if( block.type == BlockType.BORDER) {
                CollisionBlocks.Add(new CollisionBlock(block.x, block.y));    
            }
            else {
                int normalizedX = MathExtension.Mod(block.x, SIZE);
                int normalizedY = MathExtension.Mod(block.y, SIZE);
                data[normalizedX + normalizedY * SIZE] = Color.White;
            }
        }

        texture.SetData(data);
        Texture = texture;
        Ready = true;
    }


    public static int ToChunkCoordinate(int coordinate) {
        return coordinate < 0 ? ((coordinate+1) / Chunk.SIZE) - 1 : (coordinate / Chunk.SIZE);
    }

}


public struct Block {
    public readonly int x;
    public readonly int y;
    public readonly BlockType type;

    public Block(int x, int y, BlockType type) {
        this.x = x;
        this.y = y;
        this.type = type;
    }

    public override string ToString() {
        return String.Format("Block( x: {0}, y: {1}, type: {2})", x, y, type);
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

public enum BlockType : byte {
    NONE,
    SPACE,
    BORDER
}