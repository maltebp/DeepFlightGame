
using System.Collections.Generic;

class NodeChunk {

    public int X { get; private set;  }
    public int Y { get; private set;  }

    private LinkedList<Node> nodes = new LinkedList<Node>();

    private NodeChunk left = null;
    private NodeChunk right = null;
    private NodeChunk up = null;
    private NodeChunk down = null;

    public NodeChunk(int x, int y) {
        X = x;
        Y = y;
    }
   
    public LinkedList<Node> GetNodes() { return nodes; }

    public void AddNode(Node node) {
        nodes.AddLast(node);
    }

    public static void ConnectHorizontal(NodeChunk left, NodeChunk right) {
        left.right = right;
        right.left = left;
    }

    public static void ConnectVertical(NodeChunk up, NodeChunk down) {
        up.down = down;
        down.up = up;
    }


}