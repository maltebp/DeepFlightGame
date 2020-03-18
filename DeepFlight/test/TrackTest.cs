using System;

public static class BlockMapTest {

    public static int blockCount = 0;
    
    public static void RunTests() {
        MassTest();
        //GetSetBlockTest();
        //LargeMapTest();
        //ChunkBorderTest();
        //ForEachCoordinatesTest();
        //CoordinateConversionTest();
    }

    private static void GetSetBlockTest() {
        Track blockMap = new Track();

        blockMap.SetBlock(BlockType.SPACE, 10, 10);
        blockMap.SetBlock(BlockType.SPACE, -10, -10);

        blockMap.SetBlock(BlockType.SPACE, 10 + Chunk.SIZE, -10);

        Console.WriteLine("Chunk Count = " + blockMap.GetChunkCount()) ;

        blockCount = 0;
        blockMap.ForBlocksInRange(-10, -10, 10, 10, (type, x, y) => {
            blockCount++;
        });
        Console.WriteLine(blockCount);

        blockCount = 0;
        blockMap.ForBlocksInRange(0, 0, 10, 10, (type, x, y) => {
            blockCount++;
        });
        Console.WriteLine(blockCount);
    }

    private static void LargeMapTest() {

        var RANGE = 50;

        Track blockMap = new Track();

        for (int y = -RANGE; y < RANGE; y++) {
            for (int x = -RANGE; x < RANGE; x++) {
                blockMap.SetBlock(BlockType.SPACE, x * Chunk.SIZE*Cell.SIZE + 50, y * Chunk.SIZE*Cell.SIZE + 50);
            }
        }

        Console.WriteLine(blockMap.GetChunkCount());

        blockCount = 0;
        blockMap.ForBlocksInRange(0, 0, 999, 999, (type, x, y) => {
            blockCount++;
        });

        Console.WriteLine(blockCount);
    }

    private static void ChunkBorderTest() {
        Console.WriteLine("\nChunkBorderTest()");

        Track blockMap = new Track();
        blockMap.SetBlock(BlockType.SPACE, 99, 99);
        blockMap.SetBlock(BlockType.SPACE, 98, 98);
        Console.WriteLine(blockMap.GetChunkCount());

        blockMap.SetBlock(BlockType.SPACE, 100, 99);
        Console.WriteLine(blockMap.GetChunkCount());
    }

    private static void ForEachCoordinatesTest() {

        Console.WriteLine("\nForEachCoordinatesTest()");
        Track blockMap = new Track();
        blockMap.SetBlock(BlockType.SPACE, 0, 0);
        blockMap.SetBlock(BlockType.SPACE, 10, -10);
        blockMap.SetBlock(BlockType.SPACE, 1337, 1337);
        blockMap.SetBlock(BlockType.SPACE, -1337, 300);
        blockMap.SetBlock(BlockType.SPACE, -9999, -4000);

        Console.WriteLine("Getters");
        Console.WriteLine("0, 0 = {0}", blockMap.GetBlock(0,0) );
        Console.WriteLine("10, -10 = {0}", blockMap.GetBlock(10, -10));
        Console.WriteLine("1337, 1337 = {0}", blockMap.GetBlock(1337, 1337));

        Console.WriteLine("For each");
        blockMap.ForBlocksInRange(-10000,-10000, 10000, 10000, (type,x,y) => {
            Console.WriteLine("x={0}  y={1}", x,y);
        });
        
    }


    private static void CoordinateConversionTest() {

        Console.WriteLine("\nCoordinateConversionTest()");

        TestCoordinate(0, 0);
        TestCoordinate(1000, 1000);
        TestCoordinate(-1000, -1000);
        TestCoordinate(-250, 250);
        TestCoordinate(999, 999);
    }



    private static void TestCoordinate(int x, int y) {
        Console.WriteLine("Coordinate: {0},{1}\tChunk: {2},{3}\tCell in chunk: {4},{5}\tBlock in cell: {6},{7}",
            x, y,
            Track.ToChunkCoordinate(x), Track.ToChunkCoordinate(y),
            Chunk.ToCellIndexInChunk(x), Chunk.ToCellIndexInChunk(y),
            Cell.ToBlockIndexInCell(x), Cell.ToBlockIndexInCell(y) );
    }

    private static void MassTest() {

        Console.WriteLine("\n\nMass Test");

        Track blockMap = new Track();

        for( int x=-100; x<100; x++) {
            for( int y=-100; y<100; y++) {
                Console.WriteLine("Is none:\t{0}", blockMap.GetBlock(x,y) == null);
                blockMap.SetBlock(BlockType.SPACE, x, y);
                Block block = blockMap.GetBlock(x, y);
                Console.WriteLine("Is space:\t{0}", block.type == BlockType.SPACE);
                Console.WriteLine("x correct:\t{0}", block.x == x);
                Console.WriteLine("y correct:\t{0}", block.y == y);
                blockMap.SetBlock(BlockType.NONE, x, y);
                Console.WriteLine("Is none:\t{0}", blockMap.GetBlock(x, y).type == BlockType.NONE);
            }
        }

    }

}