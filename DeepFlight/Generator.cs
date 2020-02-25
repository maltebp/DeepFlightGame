

using System;
using System.Collections.Generic;

class Generator {

    private const int SIZE = 100;
    private const int MARGIN = 15;

    private int offsetX;
    private int offsetY;

    private int indexOffsetX = 0;
    private int indexOffsetY = 0;

    private BlockType[,] blocks = new BlockType[SIZE, SIZE];

    private Random rand = new Random();

    //private NodeChunk centerChunk = null;
    private LinkedList<TrackNode> nodes = new LinkedList<TrackNode>();

    public delegate void BlockCallback(BlockType type, int x, int y);
    

    public void UpdateOffset(int newX, int newY) {

        int offsetDiffX = newX - offsetX;
        int offsetDiffY = newY - offsetY;

        if (offsetDiffX > SIZE || offsetDiffY > SIZE || offsetDiffX < -SIZE || offsetDiffY < -SIZE) {
            indexOffsetX = 0;
            indexOffsetY = 0;
            offsetDiffX = SIZE;
            offsetDiffY = 0;
        } else {
            indexOffsetX += offsetDiffX;
            indexOffsetX = MathExtension.Mod(indexOffsetX, SIZE);
            indexOffsetY += offsetDiffY;
            indexOffsetY = MathExtension.Mod(indexOffsetY, SIZE);
        }

        if( offsetDiffX > 0)
            for (int y = 0; y < blocks.GetLength(0); y++)
                for (int i = 1; i <= offsetDiffX; i++)
                    blocks[y, MathExtension.Mod((indexOffsetX - i),SIZE)] = BlockType.NONE;
        
        if( offsetDiffX < 0)
            for (int y = 0; y < blocks.GetLength(0); y++)
                for (int i = 0; i < offsetDiffX*-1; i++)
                    blocks[y, MathExtension.Mod((indexOffsetX+i), SIZE) ] = BlockType.NONE;

        if (offsetDiffY > 0)
            for (int i = 1; i <= offsetDiffY; i++)
                for (int x = 0; x < blocks.GetLength(1); x++)
                    blocks[MathExtension.Mod((indexOffsetY-i), SIZE), x] = BlockType.NONE;

        if (offsetDiffY < 0)
            for (int i = 0; i < offsetDiffY*-1; i++)
                for (int x = 0; x < blocks.GetLength(1); x++)
                    blocks[MathExtension.Mod((indexOffsetY+i), SIZE), x] = BlockType.NONE;



        offsetX = newX;
        offsetY = newY;


        int innerMinX = offsetX - SIZE / 2;
        int innerMaxX = offsetX + SIZE / 2;
        int innerMinY = offsetY - SIZE / 2;
        int innerMaxY = offsetY + SIZE / 2;

        int outerMinX = innerMinX - MARGIN;
        int outerMaxX = innerMaxX + MARGIN;
        int outerMinY = innerMinY - MARGIN;
        int outerMaxY = innerMaxY + MARGIN;

        if (offsetDiffX != 0 || offsetDiffY != 0) {

            TrackNode node = nodes.First.Value;
            do {
                // Check if node is relevant for bounds
                if (node.X >= outerMinX && node.X < outerMaxX && node.Y >= outerMinY && node.Y < outerMaxY) {

                    int centerIndexX = (int)(node.X - offsetX + SIZE / 2);
                    int centerIndexY = (int)(node.Y - offsetY + SIZE / 2);

                    double targetX = node.Next.X;
                    double targetY = node.Next.Y;

                    double currentX = centerIndexX;
                    double currentY = centerIndexY;

                    double xDistance = targetX - node.X;
                    double yDistance = targetY - node.Y;

                    double xStepSize = xDistance / 3.0;
                    double yStepSize = yDistance / 3.0;

                    for(int i=0; i<3; i++) {
                        currentX += xStepSize;
                        currentY += yStepSize;

                        int size = 8;

                        for (int y = -size; y <= size; y++) {
                            for (int x = -size; x <= size; x++) {
                                int distance = Math.Abs(x) + Math.Abs(y);
                                if (distance <= size) {

                                    int indexX = (int) currentX + x;
                                    int indexY = (int) currentY + y;

                                    if (indexX >= 0 && indexX < SIZE && indexY >= 0 && indexY < SIZE) {

                                        indexX += indexOffsetX;
                                        indexY += indexOffsetY;

                                        indexX = MathExtension.Mod(indexX, SIZE);
                                        indexY = MathExtension.Mod(indexY, SIZE);

                                        if (x == 0 && y == 0)
                                            blocks[indexY, indexX] = BlockType.CENTER;
                                        else
                                            blocks[indexY, indexX] = BlockType.WALL;
                                    }
                                }
                            }
                        }

                    }
                    
                    //centerIndexX += indexOffsetX;
                    //centerIndexY += indexOffsetY;

                    //centerIndexX = MathExtension.Mod(centerIndexX, SIZE);
                    //centerIndexY = MathExtension.Mod(centerIndexY, SIZE
                }
            } while ((node = node.Next) != null && node.Next != null);
        }
    }

    public void ForEachBlock( BlockCallback callback ) {
        int y=indexOffsetY, x;
        for (int yCount = 0; yCount < blocks.GetLength(0); yCount++) {
            y = (++y) % SIZE;
            x = indexOffsetX;
            for (int xCount = 0; xCount < blocks.GetLength(1); xCount++) {
                x = (++x) % SIZE;
                callback(blocks[y,x], offsetX - SIZE/2 + xCount, offsetY - SIZE/2 + yCount);
            }
        }
    }

    //public BlockType[,] GetChunk() {
    //    return null;
    //}

    //public BlockType[,] GetBlockChunk(int x, int y) {
    //    // Return generated block from nodes 
    //    return null;
    //}


    public void GenerateTrack() {

        nodes.AddLast(new TrackNode(0, 0, 0) );

        TrackNode previous, next;
        double direction = 0;
        double shiftCooldown = 0;
        double directionAccel = 0;
        double speed = 15;
        Random rand = new Random();

        for( int i=1; i < 500; i++) {

            previous = nodes.Last.Value;



            direction += directionAccel;
            direction %= (Math.PI * 2);

            next = new TrackNode(i, previous.X + Math.Sin(direction) * speed, previous.Y + Math.Cos(direction) * speed);
            previous.Next = next;

            nodes.AddLast(next);

            shiftCooldown--;
            if (shiftCooldown <= 0) {

                directionAccel = rand.Next(1, 40);

                shiftCooldown = (int)(75 / directionAccel);

                if (rand.Next(0, 100) < 50) directionAccel *= -1;
                directionAccel /= 100.0;
                
            }

        }

    }


}


public enum BlockType {
    NONE,
    WALL,
    CENTER
}



class TrackNode {
    public int ID { get; private set; }
    public double X { get; private set; }
    public double Y { get; private set; }
    public TrackNode Next { get; set; } = null;

    public TrackNode(int id, double x, double y) {
        ID = id;
        X = x;
        Y = y;
    }

    public override string ToString() {
        return String.Format("Node( ID: {0} X: {1:N2}, Y: {2:N2})", ID, X, Y);
    }
}



