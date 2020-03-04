using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeepFlightUnitTests {

    [TestClass]
    public class BlockMapTests {

        [TestMethod]
        public void OverallTest() {
            var RADIUS = 50;
            BlockMap blockMap = new BlockMap();

            for(int y=-RADIUS; y < RADIUS; y++) {
                for( int x= -RADIUS; x < RADIUS; x++) {
                    blockMap.SetBlock(BlockType.SPACE, x*Chunk.SIZE + Chunk.SIZE/2, y * Chunk.SIZE + Chunk.SIZE / 2); 
                }
            }

            Assert.AreEqual(50*50, blockMap.GetChunkCount());


        }
    }
}
