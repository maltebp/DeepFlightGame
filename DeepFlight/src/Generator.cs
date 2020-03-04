﻿

using System;
using System.Collections.Generic;

static class Generator {
    
    public static Track GenerateTrack() {

        Track track = new Track();
        LinkedList<TrackNode> nodes = GenerateNodes();

        TrackNode node = nodes.First.Value;
        do {
            double centerIndexX = node.X;
            double centerIndexY = node.Y;

            double targetX = node.Next.X;
            double targetY = node.Next.Y;

            double currentX = centerIndexX;
            double currentY = centerIndexY;

            double xDistance = targetX - node.X;
            double yDistance = targetY - node.Y;

            double xStepSize = xDistance / 10.0;
            double yStepSize = yDistance / 10.0;

            for (int i = 0; i < 10; i++) {
                currentX += xStepSize;
                currentY += yStepSize;

                int size = 16;

                for (int y = -size; y <= size; y++) {
                    for (int x = -size; x <= size; x++) {
                        int distance = Math.Abs(x) + Math.Abs(y);
                        if (distance <= size) {
                            track.SetBlock( BlockType.SPACE, (int) (currentX+x), (int) (currentY+y) );


                            //int indexX = (int)currentX + x;
                            //int indexY = (int)currentY + y;

                            //if (indexX >= 0 && indexX < SIZE && indexY >= 0 && indexY < SIZE) {

                            //    indexX += indexOffsetX;
                            //    indexY += indexOffsetY;

                            //    indexX = MathExtension.Mod(indexX, SIZE);
                            //    indexY = MathExtension.Mod(indexY, SIZE);

                            //    if (x == 0 && y == 0)
                            //        blocks[indexY, indexX] = BlockType.CENTER;
                            //    else
                            //        blocks[indexY, indexX] = BlockType.WALL;
                            //}
                        }
                    }
                }

            }
        } while ((node = node.Next) != null && node.Next != null);



        return track;
    }




    public static LinkedList<TrackNode> GenerateNodes() {

        LinkedList<TrackNode> nodes = new LinkedList<TrackNode>();

        nodes.AddLast(new TrackNode(0, 0, 0));

        TrackNode previous, next;
        double direction = 0;
        double shiftCooldown = 0;
        double directionAccel = 0;
        double speed = 15;
        Random rand = new Random();

        for (int i = 1; i < 500; i++) {
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

        return nodes;
    }


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


