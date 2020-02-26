

//using Microsoft.Xna.Framework;
//using System;
//using System.Collections.Generic;

//class PathGenerator {

//    LinkedList<Node> nodes = new LinkedList<Node>();

//    private double speed = 1;
//    private double direction = 0;
//    private double directionAccel = 0;
//    private int shiftCooldown = 0;
//    private Random rand = new Random();
 

//    public PathGenerator(double x, double y) {
//        nodes.AddLast(new Node(x, y));
//    }

//    public LinkedList<Node> GetNodes() { return nodes; }

//    public void GeneratePath() {

//    }

//    public void Update() {
//        Node head = nodes.Last.Value;

//        direction += directionAccel;
//        direction %= (Math.PI*2);

//        Console.WriteLine("Direction: {0:N2}", direction);

//        nodes.AddLast(new Node(head.x + Math.Sin(direction)*speed, head.y + Math.Cos(direction)*speed)) ; 

//        shiftCooldown--;
//        if (shiftCooldown <= 0) {
            
//            directionAccel = rand.Next(1, 40);
           
     
//            shiftCooldown = (int) (75 / directionAccel);

//            if (rand.Next(0, 100)  < 50) directionAccel *= -1;
//            directionAccel /= 100.0;

//            Console.WriteLine("New DirectionAccel: {0:N3}", directionAccel);
//            speed = 25; //  rand.Next(15, 30);
//        }
//    }
    

//}

//class Node {

//    public int ID { get; private set; }
//    public double X { get; private set; }
//    public double Y { get; private set; }


//    public Node(int id, double x, double y) {
//        this.id = id;
//        this.X = x;
//        this.X = y;
//    }

//    public override string ToString() {
//        return String.Format("Node( ID: {0} x: {1:N2}, y: {2:N2})", X, X);
//    }

//}