using System;
using System.Collections.Generic;


public delegate void BlockCallback(BlockType type, int x, int y);


public class Track {

    private LinkedList<Chunk> chunks = new LinkedList<Chunk>();

    public void SetBlock(BlockType block, int x, int y) {
        int chunkX = ToChunkCoordinate(x);
        int chunkY = ToChunkCoordinate(y);
        Chunk chunk = GetChunk(chunkX, chunkY);
        if( chunk == null) {
            chunk = new Chunk(chunkX, chunkY);
            chunks.AddLast(chunk);
        }
        chunk.SetBlock(block, x, y );
    }

    public void ForBlocksInRange(int minX, int minY, int maxX, int maxY, BlockCallback callback) {
        int maxChunkX = ToChunkCoordinate(maxX);
        int maxChunkY = ToChunkCoordinate(maxY);
        int minChunkX = ToChunkCoordinate(minX);
        int minChunkY = ToChunkCoordinate(minY);

        foreach( Chunk chunk in chunks) {
            if( chunk.X >= minChunkX && chunk.Y >= minChunkY && chunk.Y <= maxChunkY && chunk.X <= maxChunkX) {
                chunk.ForEachBlock(callback);
            }
        }

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
        return coordinate < 0 ? (coordinate / (Chunk.SIZE*Cell.SIZE)) - 1 : (coordinate / (Chunk.SIZE*Cell.SIZE));
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

    public void SetBlock(BlockType block, int x, int y) {
        int cellX = ToCellIndexInChunk(x);
        int cellY = ToCellIndexInChunk(y);
        if (cells[cellY, cellX] == null)
            cells[cellY, cellX] = new Cell(Cell.ToCellIndex(x), Cell.ToCellIndex(y) );
        cells[cellY,cellX].SetBlock(block, x, y);
    }

    public void ForEachBlock(BlockCallback callback) {
        foreach( Cell cell in cells) {
            if (cell != null)
                cell.ForEachBlock(callback);
        }
    }

    public int ToCellCoordinate(int coordinate) {
        return (coordinate / Cell.SIZE);
    }

    public static int ToCellIndexInChunk(int coordinate) {
        return (Math.Abs(coordinate)%(SIZE*Cell.SIZE)) / Cell.SIZE;
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

    private BlockType[,] blocks = new BlockType[SIZE, SIZE];

    public void SetBlock(BlockType block, int x, int y) {
        blocks[ToBlockIndexInCell(y), ToBlockIndexInCell(x)] = block;
    }

    public void ForEachBlock(BlockCallback callback) {

        int cellOffsetX = indexX < 0 ? (indexX + 1) * SIZE : indexX * SIZE;
        int cellOffsetY = indexY < 0 ? (indexY + 1) * SIZE : indexY * SIZE;


        /* Block indexes are away from 0. If the cell's range is
         * [-10, -19] then block index 0 is -10 and index 10 is -19*/
        int directionX = indexX < 0 ? -1 : 1;
        int directionY = indexY < 0 ? -1 : 1;

        for (int y=0; y < blocks.GetLength(0); y++) {
            for( int x=0; x < blocks.GetLength(1); x++) {
                if( blocks[y,x] != BlockType.NONE ) {
                    callback(blocks[y,x], cellOffsetX + x*directionX, cellOffsetY + y*directionY);
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


public enum BlockType {
    NONE,
    SPACE,
    BORDER
}