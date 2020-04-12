using DeepFlight.track;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;


public delegate void BlockCallback(Block type, int x, int y);


public class Track {

    public long     ID { get; set; }
    public string   Name { get; set; }
    public long     Seed { get; set;  }
    public int      Length { get; set; }
    public uint      BlockDataSize { get; set; }
    public byte[]   BlockData { get; set; }

    public int      StartX { get; set; }
    public int      StartY { get; set; }
    public double StartRotation { get; set; }

    public bool BlockDataProcessed { get; set; }

    public Planet Planet { get; set; }

    public Checkpoint[] Checkpoints { get; set; } = new Checkpoint[0];

    private LinkedList<Chunk> chunks = new LinkedList<Chunk>();

    public int BlockCount { get; private set; }

    // Min and max coordinates registered for a block
    public int MinX { get; private set; }
    public int MinY { get; private set; }
    public int MaxX { get; private set; }
    public int MaxY { get; private set; }

    public void SetBlock(BlockType block, int x, int y) {
        int chunkX = ToChunkCoordinate(x);
        int chunkY = ToChunkCoordinate(y);
        Chunk chunk = GetChunk(chunkX, chunkY);
        if (chunk == null) {
            chunk = new Chunk(chunkX, chunkY);
            chunks.AddLast(chunk);
        }

        bool newBlock = chunk.SetBlock(block, x, y);
        if (newBlock)
            BlockCount++;

        // Update min-max coordinates
        if (x > MaxX) MaxX = x;
        if (x < MinX) MinX = x;
        if (y > MaxY) MaxY = y;
        if (y < MinY) MinY = y;
    }

    public Block GetBlock(int x, int y) {
        int chunkX = ToChunkCoordinate(x);
        int chunkY = ToChunkCoordinate(y);
        Chunk chunk = GetChunk(chunkX, chunkY);
        if (chunk == null) return null;
        return chunk.GetBlock(x, y);
    }

    public void ForBlocksInRange(int minX, int minY, int maxX, int maxY, BlockCallback callback) {
        int maxChunkX = ToChunkCoordinate(maxX);
        int maxChunkY = ToChunkCoordinate(maxY);
        int minChunkX = ToChunkCoordinate(minX);
        int minChunkY = ToChunkCoordinate(minY);

        foreach (Chunk chunk in chunks) {
            if (chunk.X >= minChunkX && chunk.Y >= minChunkY && chunk.Y <= maxChunkY && chunk.X <= maxChunkX) {
                chunk.ForEachBlock(callback);
            }
        }

    }

    public void ForAllBlocks(BlockCallback callback) {
        ForBlocksInRange(MinX, MinY, MaxX, MaxY, callback);
    }

    public int GetChunkCount() {
        return chunks.Count;
    }

    /** Get chunk which contains Block with coordinates */
    public Chunk GetChunk(int x, int y) {
        foreach (Chunk chunk in chunks)
            if (chunk.X == x && chunk.Y == y)
                return chunk;
        return null;
    }

    public static int ToChunkCoordinate(int coordinate) {
        return coordinate < 0 ? (coordinate / (Chunk.SIZE * Cell.SIZE)) - 1 : (coordinate / (Chunk.SIZE * Cell.SIZE));
    }


    public override string ToString() {
        return
            "Track( " +
                "name=" + Name + ", "+    
                "id=" + ID + ", "+    
                "seed=" + Seed +  ", " +
                "planet=" + Planet.Name + " (id=" + Planet.ID + "), " +
                "length=" + Length + ", " +
                "startPos=(" + StartX + "," + StartY + "), " +
                "startRot=" + string.Format("{0:N2}", StartRotation) + ", " +
                "size=" + string.Format("{0:N3}",BlockDataSize/1000000.0) + "mb, " +
                "processed=" + BlockDataProcessed +
            " )"
        ;
    }

    

}


public class Chunk {
    public static readonly int SIZE = 10;
    public int X { get; private set; }
    public int Y { get; private set; }

    public Chunk(int x, int y) {
        X = x;
        Y = y;
    }


    private Cell[,] cells = new Cell[SIZE, SIZE];

    // Returns true if a new block was created (was null before) or false if a block was overwritten
    public bool SetBlock(BlockType block, int x, int y) {
        int cellX = ToCellIndexInChunk(x);
        int cellY = ToCellIndexInChunk(y);
        if (cells[cellY, cellX] == null)
            cells[cellY, cellX] = new Cell(Cell.ToCellIndex(x), Cell.ToCellIndex(y));
        return cells[cellY, cellX].SetBlock(block, x, y);
    }

    public Block GetBlock(int x, int y) {
        int cellX = ToCellIndexInChunk(x);
        int cellY = ToCellIndexInChunk(y);
        if (cells[cellY, cellX] == null) return null;
        return cells[cellY, cellX].GetBlock(x, y);
    }

    public void ForEachBlock(BlockCallback callback) {
        foreach (Cell cell in cells) {
            if (cell != null)
                cell.ForEachBlock(callback);
        }
    }



    public int ToCellCoordinate(int coordinate) {
        return (coordinate / Cell.SIZE);
    }

    public static int ToCellIndexInChunk(int coordinate) {
        return (Math.Abs(coordinate) % (SIZE * Cell.SIZE)) / Cell.SIZE;
    }


}


public class Cell {
    public static readonly int SIZE = 10;

    // The cells index in the total map (not relative to its parent chunk).
    private int indexX;
    private int indexY;

    public Cell(int x, int y) {
        indexX = x;
        indexY = y;
    }

    private Block[,] blocks = new Block[SIZE, SIZE];


    // Returns true if a new block was created (the block was null before), or false it a block was overwritten
    public bool SetBlock(BlockType block, int x, int y) {
        if (blocks[ToBlockIndexInCell(y), ToBlockIndexInCell(x)] == null) {
            blocks[ToBlockIndexInCell(y), ToBlockIndexInCell(x)] = new Block(x, y, block);
            return true;
        }
        blocks[ToBlockIndexInCell(y), ToBlockIndexInCell(x)].Type = block;
        return false;
    }

    public Block GetBlock(int x, int y) {
        return blocks[ToBlockIndexInCell(y), ToBlockIndexInCell(x)];
    }

    public void ForEachBlock(BlockCallback callback) {

        int cellOffsetX = indexX < 0 ? (indexX + 1) * SIZE : indexX * SIZE;
        int cellOffsetY = indexY < 0 ? (indexY + 1) * SIZE : indexY * SIZE;


        /* Block indexes are away from 0. If the cell's range is
         * [-10, -19] then block index 0 is -10 and index 10 is -19*/
        int directionX = indexX < 0 ? -1 : 1;
        int directionY = indexY < 0 ? -1 : 1;

        for (int y = 0; y < blocks.GetLength(0); y++) {
            for (int x = 0; x < blocks.GetLength(1); x++) {
                if (blocks[y, x] != null) {
                    callback(blocks[y, x], cellOffsetX + x * directionX, cellOffsetY + y * directionY);
                }
            }
        }
    }

    public static int ToBlockIndexInCell(int coordinate) {
        return Math.Abs(coordinate) % SIZE;
    }

    public static int ToCellIndex(int coordinate) {
        return coordinate < 0 ? coordinate / SIZE - 1 : coordinate / SIZE;
    }

}


public class Block : TextureView {
    public BlockType Type { get; set; }

    public Block(int x, int y, BlockType type) : base(null, Textures.SQUARE, Color.White, x, y, 1, 1) {
        AddCollider(new RectCollider(this));
        this.Type = type;
    }

    public override string ToString() {
        return String.Format("Block( x: {0}, y: {1}, type: {2})", X, Y, Type);
    }
}

public enum BlockType : byte {
    NONE,
    SPACE,
    BORDER
}