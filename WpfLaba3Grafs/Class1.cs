using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes; // для фигур

namespace WpfLaba3Grafs
{
    public class Node
    {
        public int id { get; set; }
        public Point Position { get; set; } //пара координат в двумерном пространстве
        public Node(int id, Point position)
        {
            this.id = id;
            Position = position;
        }
    }

    public class Edge
    {
        public Node from { get; set; }
        public Node to { get; set; }

        public Edge(Node vertex1, Node vertex2)
        {
            from = vertex1;
            to = vertex2;
        }
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
        public void AddNode(Node node)
        {
            if (!Nodes.Contains(node))
                Nodes.Add(node);
        }
        public void RemoveNode(Node node)
        {
            if (Nodes.Contains(node))
            {
                Nodes.Remove(node);
                Edges.RemoveAll(edge => edge.from == node || edge.to == node);
            }
        }
        public void AddEdge(Node vertex1, Node vertex2)
        {
            if (Nodes.Contains(vertex1) && Nodes.Contains(vertex2))
            {
                Edge newEdge = new Edge(vertex1, vertex2);
                if (!Edges.Contains(newEdge))
                    Edges.Add(newEdge);
            }

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