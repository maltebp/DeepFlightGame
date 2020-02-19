using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class Generator {

	private static LinkedList<Block> sector = new LinkedList<Block>();
	public static int BLOCK_SIZE = 2000;
	public static int CELL_SIZE = 10;
	private static ulong masterSeed;

	public static void setMasterSeed(ulong seed) {
		masterSeed = seed;
	}

	public static LinkedList<Block> GenerateSector(Vector2 center, int sectorRadius) {

		// TODO: This could be optimized with arrays

		int minBlockX = GetBlockX((int) center.X) - sectorRadius;
		int minBlockY = GetBlockY((int) center.Y) - sectorRadius;

		LinkedList<Block> newSector = new LinkedList<Block>();

		for( int y=0; y < sectorRadius*2; y++) {
			int adjustedY = minBlockY + y;
			for ( int x=0; x < sectorRadius*2; x++) {
				int adjustedX = minBlockX + x;

				Block block = null;
				foreach (Block existingBlock in sector ){
					if( adjustedX == existingBlock.getX() && adjustedY == existingBlock.getY()) {
						block = existingBlock;
						sector.Remove(existingBlock);
						break;
					}
				}
				if (block == null)
					block = generateBlock(adjustedX, adjustedY);

				newSector.AddLast(block);					
			}
		}

		sector = newSector;
		return sector;
	}

	public static LinkedList<Block> GetSector() {
		return sector;
	}

	public static Block generateBlock(int blockX, int blockY) {

		int minX = blockX * BLOCK_SIZE;
		int minY = blockY * BLOCK_SIZE;
		int maxX = minX + BLOCK_SIZE;
		int maxY = minY + BLOCK_SIZE;

		Block block = new Block(blockX, blockY);

		for(int x = minX; x < maxX; x += CELL_SIZE) {
			for(int y = minY; y <  maxY; y += CELL_SIZE) {

				ulong xL = (ulong) x;
				ulong yL = (ulong) y;

				//ulong seed = (yL << 32) | ((xL << 32) >> 32) ;
				ulong seed = ((yL << 40) | (xL & 0x00000FFF)) | (masterSeed & 0x000FF000);
				//ulong seed = (yL & 0x0000FFFF) | (xL & 0xFFFF0000);


				//// Not optimal to the long to int
				////uint seed = (uint)(masterSeed + y + x);
				//uint seed = (uint) ((((y << 16) | x) >> 16) | masterSeed );
				//if (seed == int.MaxValue) seed--;
				//if (seed == 0) seed++;
				//Console.WriteLine("s: " + seed);

				LehmerRng rand = new LehmerRng(seed);

				ulong r = rand.Next();

				ulong i = r % 100;
				//Console.WriteLine("{5}\t({0},{1})\ts: {2}, \tr: {3}\ti: {4}", x, y, seed, r, i, i<10 );

				if (  i < 10) {
					block.AddNode(new Node(new Vector2(x, y)));
				}

				// Generate cell from coordinates and master seed
				// Add cell to dynamic index list
			}
		}

		// Establish connections


		// Return array
		return block;
	}

	public static int GetBlockX(int xCoordinate) {
		int blockX = (xCoordinate / BLOCK_SIZE);
		if (xCoordinate < 0) blockX--;
		return blockX;
	}

	public static int GetBlockY(int yCoordinate) {
		int blockY = (yCoordinate / BLOCK_SIZE);
		if (yCoordinate < 0) blockY--;
		return blockY;
	}

}


class Node {

	LinkedList<Connection> connections = new LinkedList<Connection>();
	private Vector2 pos;

	public Node(Vector2 pos) {
		this.pos = pos;
	}

	public void AddConnection(Connection connection) {
		connections.AddLast(connection);
	}

	public bool IsConnectedTo(Node node) {
		foreach( Connection connection in connections) {
			if (connection.GetConnectedNode(this) == node)
				return true;
		}
		return false;
	}

	public Vector2 GetPos() {
		return pos;
	}
}

class Block {

	private LinkedList<Node> nodes = new LinkedList<Node>();
	private int x, y;
	public Block(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public int getX() {
		return x;
	}

	public int getY() {
		return y;
	}

	public void AddNode(Node node) {
		nodes.AddLast(node);
	}

	public LinkedList<Node> GetNodes() {
		return nodes;
	}

}

class Connection {

	private Node node1;
	private Node node2;

	Connection(Node node1, Node node2) {
		this.node1 = node1;
		this.node2 = node2;

		node1.AddConnection(this);
		node2.AddConnection(this);
	}
	
	public Node GetConnectedNode(Node node) {
		return node == node1 ? node2 : node1;
	}
}
