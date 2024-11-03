using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;


namespace WpfLaba3Grafs
{
    
    public class Node
    {
        public int id { get; set; }
        public Point Position { get; set; } 
        public Node(int id, Point position)
        {
            this.id = id;
            Position = position;
        }
        public Node() { }
        public override string ToString() { return $"{id}, {Position.ToString()} "; }
    }

    public class Edge
    {
        public Node from { get; set; }
        public Node to { get; set; }
        public int weight { get; set; }
        public Edge(Node vertex1, Node vertex2)
        {
            from = vertex1;
            to = vertex2;
            weight = 0;
        }
        public Edge(Node vertex1, Node vertex2, int weight)
        {
            from = vertex1;
            to = vertex2;
            this.weight = weight;
        }
        public override string ToString() { return $"{from.ToString()}, {to.ToString()}"; }
    }
    public class Graph
    {
        public List<Node> Nodes { get; set; } = new List<Node>();
        public List<Edge> Edges { get; set; } = new List<Edge>();

        public bool ContainsNode(Node node)
        {
            for (int i = 0; i < Nodes.Count; i++)
                if (AreNodesClose(Nodes[i].Position, node.Position, 5))
                    return true;
            return false;
        }
        public bool ContainsEdge(Edge edge)
        {
            for (int i = 0; i < Edges.Count; i++)
                if ((Edges[i].from == edge.from && Edges[i].to == edge.to) || (Edges[i].from == edge.to && Edges[i].to == edge.from))
                    return true;
            return false;
        }
        public bool AddNode(Node node)
        {
            if (!ContainsNode(node))
            {
                Nodes.Add(node);
                return true;
            }
            return false;
        }
        public void RemoveNode(Node node)
        {
            if (Nodes.Contains(node))
            {
                Nodes.Remove(node);
                Edges.RemoveAll(edge => edge.from == node || edge.to == node);
            }
        }
        public bool AddEdge(Node vertex1, Node vertex2)
        {
            if (Nodes.Contains(vertex1) && Nodes.Contains(vertex2))
            {
                Graph graph = new Graph();
                Edge newEdge = new Edge(vertex1, vertex2);
                if (!ContainsEdge(newEdge))
                {
                    Edges.Add(newEdge);
                    return true;
                }
            }
            return false;
        }
        public void RemoveEdge(Edge edge)
        {
            if (Edges.Contains(edge))
            {
                Edges.Remove(edge);
                Nodes.RemoveAll(node => edge.from == node || edge.to == node);
            }
        }
        public bool AreNodesClose(Point point1, Point point2, double radius)
        {
            double radiusSquared = radius * radius;
            double distanceSquared = (point1.X - point2.X) * (point1.X - point2.X) +
                                     (point1.Y - point2.Y) * (point1.Y - point2.Y);
            return distanceSquared <= radiusSquared;
        }
        private const double MergeDistance = 5.0;//10.0;
        public void Merge(Graph graph)
        {
            for (int i = 0; i < graph.Nodes.Count; i++)
            {
                var node1 = graph.Nodes[i];
                for (int j = i+1;j < graph.Nodes.Count; j++)
                {
                    var node2 = graph.Nodes[j];
                    if (AreNodesClose(node1.Position, node2.Position, MergeDistance))//(GetDistance(node1.Position, node2.Position) < MergeDistance)
                    {
                        Point mergedPosition = GetMergedPosition(node1.Position, node2.Position);
                        node1.Position = mergedPosition;
                        node2.Position = mergedPosition;
                    }
                }
                foreach(var edge in graph.Edges)
                {
                    if(AreNodesClose(node1.Position, edge.from.Position, MergeDistance))
                        edge.from.Position = node1.Position;
                    if (AreNodesClose(node1.Position, edge.to.Position, MergeDistance))
                        edge.to.Position = node1.Position;
                }
            }
        }
        //private double GetDistance(Point p1, Point p2)
        //{
        //    double dx = p1.X - p2.X;
        //    double dy = p1.Y - p2.Y;
        //    return Math.Sqrt(dx * dx + dy * dy);
        //}
        private Point GetMergedPosition(Point p1, Point p2)
        {
            double centerX = (p1.X + p2.X) / 2;
            double centerY = (p1.Y + p2.Y) / 2;
            return new Point(centerX, centerY);
        }
    }

}