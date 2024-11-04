using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace WpfLaba3Grafs
{
    public class Node2
    {
        public int value;
        public Point position;
        HashSet<Edge2> edges = new HashSet<Edge2>(); //список ребер
        Dictionary<Node2, Edge2> parents = new Dictionary<Node2, Edge2>(); //список родителей 
        public Node2(int value) { this.value = value; }
        public Node2(int value, Point pos) { this.value = value; position = pos; }
        public Node2() { }
        public Node2 AddOrGetNode(Dictionary<int, Node2> graph, int value)
        {
            if (value == -1) return null;
            if (graph.ContainsKey(value))
                return graph[value];
            Node2 node = new Node2(value);
            graph.Add(value, node);
            return node;
        }
        public Dictionary<int,Node2> CreateGraph( List<(int,int,int)> graphData)
        {
            Dictionary<int,Node2> graph = new Dictionary<int,Node2>();
            foreach ((int,int,int) row in graphData)
            {
                Node2 node = AddOrGetNode(graph, row.Item1); //откуда
                Node2 adjacentNode = AddOrGetNode(graph, row.Item2); //куда
                if (adjacentNode == null)
                    continue;
                Edge2 edge = new Edge2(adjacentNode, row.Item3);
                node.edges.Add(edge);
                adjacentNode.parents.Add(node, edge);
            }
            return graph;
        }
        
    }

    public class Edge2
    {
        Node2 adjacentNode; //узел, на который ведёт ребро
        int weight;
        public Edge2(Node2 adjacentNode, int weight) {  this.adjacentNode = adjacentNode; this.weight = weight; }
        public bool AddEdge(List<(int, int, int)> graphData, Node2 node, Node2 adjacentNode, int weight)
        {
            foreach ((int, int, int) row in graphData)
                if (row.Item1 == node.value && row.Item2 == adjacentNode.value)
                    return false;
            graphData.Add((node.value, adjacentNode.value, weight));
            return true;
        }
    }
}
