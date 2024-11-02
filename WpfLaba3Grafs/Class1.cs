using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


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
                if (Nodes[i].id == node.id)
                    return false;
            return true;
        }
        public bool ContainsEdge(Edge edge)
        {
            for (int i = 0; i < Edges.Count; i++)
                if ((Edges[i].from == edge.from && Edges[i].to == edge.to) || (Edges[i].from == edge.to && Edges[i].to == edge.from))
                    return false;
            return true;
        }
        public bool AddNode(Node node)
        {
            if (ContainsNode(node))
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
                Edge newEdge = new Edge(vertex1, vertex2);
                if (ContainsEdge(newEdge))
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
    }

}