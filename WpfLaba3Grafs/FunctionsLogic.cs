using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Shapes; 
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Data;
using System.Reflection.Emit;


namespace WpfLaba3Grafs
{
    public class FunctionsLogic 
    {
        public int size = 20;
        public bool newVertex = false;
        public bool newEdge = false;
        private MainWindow mainWindow;
        public Dictionary<Edge, EdgePicture> edgePictures = new Dictionary<Edge, EdgePicture>();
        public Dictionary<Node, NodePicture> nodePictures = new Dictionary<Node, NodePicture>();
        public FunctionsLogic(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }
        public void CreateVertex(Point position)
        {
            string selectedColorName = mainWindow.GetSelectedColor();
            Brush strokeBrush = mainWindow.ConvertStringToBrush(selectedColorName);
            Ellipse vertex = new Ellipse()
            {
                Width = size,
                Height = size,
                Stroke = strokeBrush,
                StrokeThickness = 2,
                Fill = Brushes.White,
            };
            vertex.MouseDown += mainWindow.BtnClick_Paint;
            double posX = position.X - vertex.Width / 2;
            double posY = position.Y - vertex.Height / 2;

            TextBlock label = new TextBlock
            {
                Text = mainWindow.graph.Count().ToString(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontWeight = FontWeights.Bold,
            };
            var grid = new Grid();
            grid.Children.Add(vertex);
            grid.Children.Add(label);
            Canvas.SetTop(grid, posY);
            Canvas.SetLeft(grid, posX);
            mainWindow.DrawingCanvas.Children.Add(grid);
        }
        public void AddEdge(Point pos1, Point pos2, Dictionary<int, Node> graph, List<(int, int, int)> graphData, int weight)
        {
            Node from = new Node(); Node to = new Node();
            for (int i = 0; i < graph.Count; i++)
            {
                if (from.AreNodesClose(pos1, graph.ElementAt(i).Value.position, 10))
                    from = graph.ElementAt(i).Value;
                else if (to.AreNodesClose(pos2, graph.ElementAt(i).Value.position, 10))
                    to = graph.ElementAt(i).Value;
            }
            Edge edge2 = new Edge();
            if (from.ContainsNode(from.position, graph) && to.ContainsNode(to.position, graph))
                if (edge2.AddEdge(graphData, from, to, weight, mainWindow.isOriented))
                {
                    string selectedColorName = mainWindow.GetSelectedColor();
                    Brush strokeBrush = mainWindow.ConvertStringToBrush(selectedColorName);
                    Line edge = new Line()
                    {
                        X1 = from.position.X,
                        Y1 = from.position.Y,
                        X2 = to.position.X,
                        Y2 = to.position.Y,
                        Stroke = strokeBrush,
                        StrokeThickness = 2
                    };
                    edge.MouseDown += mainWindow.BtnClick_Paint;
                    Polygon polygon = new Polygon();
                    

                    newEdge = false;
                    TextBox textBox = new TextBox
                    {
                        Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
                        BorderThickness = new Thickness(0, 0, 0, 0),
                        Width = 60,
                        Height = 18,
                    };
                    string tb = "№ " + CalculateEdges(graph).ToString();
                    if (weight != 0)
                        tb = tb + "; Вес " + weight.ToString();
                    textBox.Text = tb;
                    textBox.IsEnabled = false;
                    

                    Edge edgesuk = new Edge(to, weight);
                    EdgePicture edgePic = new EdgePicture(textBox.Text, "Black", edgesuk);
                    edgePictures.Add(edgesuk, edgePic);

                    double centerX = (from.position.X + to.position.X) /2;
                    double centerY = (from.position.Y + to.position.Y) / 2;
                    
                    Canvas.SetLeft(textBox, centerX - textBox.ActualWidth / 2);
                    Canvas.SetTop(textBox, centerY - textBox.ActualHeight / 2);
                    edge.Tag=textBox;
                    //edge.Tag = DrawArrow(pos1, pos2);
             
                    mainWindow.DrawingCanvas.Children.Add(edge);
                    mainWindow.DrawingCanvas.Children.Add(textBox);
                    if (mainWindow.isOriented == true)
                    {
                        polygon = DrawArrow(pos1, pos2);
                        textBox.Tag = polygon;
                        mainWindow.DrawingCanvas.Children.Add(polygon);
                    }
                }
        }
        private void EdgeLine(Point ellipse1, Point ellipse2, double radius)
        {
            double distance = 0;
            double d = Math.Sqrt((ellipse2.X - ellipse1.X) * (ellipse2.X - ellipse1.X) - (ellipse2.Y - ellipse1.Y) * (ellipse2.Y - ellipse1.Y));
            if (d > radius * 2) 
                distance = d - radius * 2;
        }
        
        public Polygon DrawArrow(Point pos1, Point pos2)
        {
            double arrowLength = 10; // Длина стрелки
            double angle = Math.Atan2(pos2.Y - pos1.Y, pos2.X - pos1.X); // Угол в радианах

            Polygon arrowHead = new Polygon
            {
                Fill = Brushes.Black,
                Points = new PointCollection
                {
                    new Point(pos2.X, pos2.Y), 
                    new Point(pos2.X - arrowLength * Math.Cos(angle - Math.PI / 6),
                    pos2.Y - arrowLength * Math.Sin(angle - Math.PI / 6)), // Левый угол стрелки

                    new Point(pos2.X - arrowLength * Math.Cos(angle + Math.PI / 6),
                    pos2.Y - arrowLength * Math.Sin(angle + Math.PI / 6))  // Правый угол стрелки
                }
            };
            return arrowHead;
        }
        public int[,] GenerateIncidenceMatrix(Dictionary<int, Node> graph)
        {
            int numEdges = CalculateEdges(graph);
            
            if (numEdges < 1)
                return null;
            int[,] matrix = new int[graph.Count+1, numEdges];
            for (int i = 0; i < numEdges; i++)
            {
                matrix[0, i] = i+1;
                for (int j = 1; j < graph.Count; j++)
                    matrix[j,i] = 0;
            }

            int edgeIndex = 0;
            foreach (var node in graph.Values)
            {
                MessageBox.Show("from " + node.ToString());
                foreach (var edge in node.edges)
                {
                    MessageBox.Show("to " + edge.ToString());
                    int rowIndex = node.value;
                    int colIndex = edgeIndex++;

                    matrix[rowIndex+1, colIndex] = edge.weight > 0 ? edge.weight : 1;
                    if (mainWindow.isOriented == true)
                        matrix[edge.adjacentNode.value+1, colIndex] = -(edge.weight > 0 ? edge.weight : 1);
                    else
                        matrix[edge.adjacentNode.value + 1, colIndex] = edge.weight > 0 ? edge.weight : 1;
                }
            }
            return matrix;
        }
        private int CalculateEdges(Dictionary<int, Node> graph)
        {
            List<Edge> allEdges = new List<Edge>();
            foreach (var node in graph.Values)
                allEdges.AddRange(node.edges);
            return allEdges.Count;
        }
        public int[,] GenerateAdjacencyMatrix(Dictionary<int, Node> graph)
        {
            int[,] matrix = new int[graph.Count + 1, graph.Count];
            for (int i = 0; i < graph.Count; i++)
            {
                matrix[0, i] = graph.ElementAt(i).Value.value;
                for (int j = 1; j < graph.Count; j++)
                    matrix[i, j] = 0;
            }

            foreach (var nodePair in graph)
            {
                int rowIndex = nodePair.Key;
                Node node = nodePair.Value;

                foreach (Edge edge in node.edges)
                {
                    int colIndex = edge.adjacentNode.value;
                    matrix[rowIndex+1, colIndex] = edge.weight > 0 ? edge.weight : 1;
                    if (mainWindow.isOriented == false)
                        matrix[colIndex + 1, rowIndex] = edge.weight > 0 ? edge.weight : 1;
                }
            }
            return matrix;
        }
        public void Dg_BuildArr(int[,] matrix, DataGrid dgName)
        {
            
            DataTable dataTable = new DataTable();
            if (matrix != null)
            {
                for (int column = 0; column < matrix.GetLength(1); column++)
                {
                    string columnName = matrix[0, column].ToString();
                    dataTable.Columns.Add(columnName);
                }
                for (int row = 1; row < matrix.GetLength(0); row++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    for (int column = 0; column < matrix.GetLength(1); column++)
                        dataRow[column] = matrix[row, column];
                    dataTable.Rows.Add(dataRow);
                }
            }
            dgName.ItemsSource = dataTable.DefaultView;
        }
    }
}
