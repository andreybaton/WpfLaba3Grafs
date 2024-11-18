using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace WpfLaba3Grafs
{
    public class Node : INotifyPropertyChanged
    {
        private int _myValue;

        public int value
        {
            get { return _myValue; }
            set
            {
                if (_myValue != value)
                {
                    _myValue = value;
                    OnPropertyChanged(nameof(Node.value));
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public Point position;
        public HashSet<Edge> edges = new HashSet<Edge>(); //список ребер
        public Dictionary<Node, Edge> parents = new Dictionary<Node, Edge>(); //список родителей 
        public Node(int value) { this.value = value; }
        public Node(int value, Point pos) { this.value = value; position = pos; }
        public Node() { }
        //public string ToString() { return MyValue.ToString(); }
        public Node AddOrGetNode(Dictionary<int, Node> graph, int value)
        {
            if (value == -1) return null;
            if (graph.ContainsKey(value))
                return graph[value];
            Node node = new Node(value);
            graph.Add(value, node);
            return node;
        }
        public Dictionary<int, Node> CreateGraph(List<(int, int, int)> graphData)
        {
            Dictionary<int, Node> graph = new Dictionary<int, Node>();
            foreach ((int, int, int) row in graphData)
            {
                Node node = AddOrGetNode(graph, row.Item1); //откуда
                Node adjacentNode = AddOrGetNode(graph, row.Item2); //куда
                if (adjacentNode == null)
                    continue;
                Edge edge = new Edge(adjacentNode, row.Item3);
                node.edges.Add(edge);
                adjacentNode.parents.Add(node, edge);
            }
            return graph;
        }
        public bool ContainsNode(Point pos, Dictionary<int, Node> graph)
        {
            for (int i = 0; i < graph.Count; i++)
                if (AreNodesClose(graph.ElementAt(i).Value.position, pos, 10))
                    return true;
            return false;
        }
        public bool AreNodesClose(Point point1, Point point2, double radius)
        {
            double radiusSquared = radius * radius;
            double distanceSquared = (point1.X - point2.X) * (point1.X - point2.X) +
                                     (point1.Y - point2.Y) * (point1.Y - point2.Y);
            return distanceSquared <= radiusSquared;
        }
    }

    public class Edge
    {
        public Node adjacentNode; //узел, на который ведёт ребро
        public int weight;
        public Edge(Node adjacentNode, int weight) { this.adjacentNode = adjacentNode; this.weight = weight; }
        public Edge() { }
        public string ToString() { return adjacentNode.ToString(); }
        public bool AddEdge(List<(int, int, int)> graphData, Node node, Node adjacentNode, int weight, bool typeEdge)
        {
            Edge edge = new Edge(adjacentNode, weight);

            if (AddSearchElement(graphData, node, adjacentNode, weight))
            {
                adjacentNode.parents.Add(node, edge);
                node.edges.Add(edge);
            }
            return true;
        }
        public bool AddSearchElement(List<(int, int, int)> graphData, Node node, Node adjacentNode, int weight)
        {
            int count = 0; int marker = -1;
            foreach ((int, int, int) row in graphData)
            {
                count++;
                if (row.Item1 == node.value && row.Item2 == adjacentNode.value)
                {
                    MessageBox.Show("Такое ребро уже существует!");
                    return false;
                }
                else if (row.Item1 == node.value && row.Item2 == -1)
                    marker = count - 1;
            }
            if (marker != -1)
                graphData[marker] = ((node.value, adjacentNode.value, weight));
            else
                graphData.Add((node.value, adjacentNode.value, weight));
            return true;
        }
    }
    public class EdgePicture : Edge
    {
        public string tbEdge { get; set; }
        public string colorEdge { get; set; }
        public Edge edge { get; set; }

        public EdgePicture(string tb, string color, Edge edge)
        {
            tbEdge = tb;
            colorEdge = color;
            this.edge = edge;
        }
    }
    public class NodePicture : Node
    {
        public string tbNode { get; set; }
        public string colorNode { get; set; }
        public Node node { get; set; }
        public NodePicture (string tb, string color, Node node)
        {
            tbNode = tb;
            colorNode = color;
            this.node = node;
        }
    }
}
