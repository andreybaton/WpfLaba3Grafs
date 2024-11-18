using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Shapes; 
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Data;
using System.Reflection.Emit;
using System.Windows.Data;


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
        public void CreateVertex(Point position, Node node)
        {
            string selectedColorName = GetSelectedColor();
            Brush strokeBrush = ConvertStringToBrush(selectedColorName);
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

            // Установка привязки
            Binding binding = new Binding("MyValue")
            {
                Source = node,
                StringFormat = node.MyValue.ToString(), // Значение вершины в canvas
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            label.SetBinding(TextBlock.TextProperty, binding);

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
                    string selectedColorName = GetSelectedColor();
                    Brush strokeBrush = ConvertStringToBrush(selectedColorName);
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

                    double centerX = (from.position.X + to.position.X) / 2;
                    double centerY = (from.position.Y + to.position.Y) / 2;

                    Canvas.SetLeft(textBox, centerX - textBox.ActualWidth / 2);
                    Canvas.SetTop(textBox, centerY - textBox.ActualHeight / 2);
                    edge.Tag = textBox;
                    //edge.Tag = DrawArrow(pos1, pos2);

                    mainWindow.DrawingCanvas.Children.Add(edge);
                    mainWindow.DrawingCanvas.Children.Add(textBox);
                    if (mainWindow.isOriented == true)
                    {
                        polygon = DrawArrow(from.position, to.position);
                        textBox.Tag = polygon;
                        mainWindow.DrawingCanvas.Children.Add(polygon);
                    }
                }
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
            int[,] matrix = new int[graph.Count + 1, numEdges];
            for (int i = 0; i < numEdges; i++)
            {
                matrix[0, i] = i + 1;
                for (int j = 1; j < graph.Count; j++)
                    matrix[j, i] = 0;
            }

            int edgeIndex = 0;
            foreach (var node in graph.Values)
            {
                foreach (var edge in node.edges)
                {
                    int rowIndex = node.MyValue;
                    int colIndex = edgeIndex++;

                    matrix[rowIndex + 1, colIndex] = edge.weight > 0 ? edge.weight : 1;
                    if (mainWindow.isOriented == true)
                        matrix[edge.adjacentNode.MyValue + 1, colIndex] = -(edge.weight > 0 ? edge.weight : 1);
                    else
                        matrix[edge.adjacentNode.MyValue + 1, colIndex] = edge.weight > 0 ? edge.weight : 1;
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
                matrix[0, i] = graph.ElementAt(i).Value.MyValue;
                for (int j = 1; j < graph.Count; j++)
                    matrix[i, j] = 0;
            }

            foreach (var nodePair in graph)
            {
                int rowIndex = nodePair.Key;
                Node node = nodePair.Value;

                foreach (Edge edge in node.edges)
                {
                    int colIndex = edge.adjacentNode.MyValue;
                    matrix[rowIndex + 1, colIndex] = edge.weight > 0 ? edge.weight : 1;
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

        //public List<int> SearchShortestPath(int[,] AdjacencyMatrix, int startVertex, int endVertex, int allVertices) //alg Deikstra
        public List<List<int>> SearchShortestPaths(int[,] AdjacencyMatrix, int startVertex, int endVertex, int allVertices)
        {
            int verticesCount = allVertices;
            int[] distances = new int[verticesCount];
            bool[] shortestPathSet = new bool[verticesCount];

            // Список списков для хранения всех предыдущих вершин
            List<int>[] previousVertices = new List<int>[verticesCount];
            for (int i = 0; i < verticesCount; i++)
            {
                previousVertices[i] = new List<int>();
            }

            // Инициализация
            for (int i = 0; i < verticesCount; i++)
            {
                distances[i] = int.MaxValue;
                shortestPathSet[i] = false;
            }

            distances[startVertex] = 0;

            for (int count = 0; count < verticesCount - 1; count++)
            {
                int u = MinDistance(distances, shortestPathSet);
                shortestPathSet[u] = true;

                for (int v = 0; v < verticesCount; v++)
                {
                    //MessageBox.Show("step " + u.ToString() + " " + v.ToString());
                    if (!shortestPathSet[v] && AdjacencyMatrix[u + 1, v] != 0 &&
                        distances[u] != int.MaxValue)
                    {
                        int newDist = distances[u] + AdjacencyMatrix[u + 1, v];
                        if (newDist < distances[v])
                        {
                            distances[v] = newDist;
                            previousVertices[v].Clear(); // Очистить предыдущие пути
                            previousVertices[v].Add(u); // Добавить текущую вершину
                            //MessageBox.Show("new" + "num" + v.ToString() + " " + u.ToString());
                        }
                        else if (newDist == distances[v])
                        {
                            previousVertices[v].Add(u); // Добавить текущую вершину как альтернативный путь
                            MessageBox.Show("another" + u.ToString());
                        }
                    }
                }
            }
            //for (int i = 0; i < previousVertices.Count(); i++)
            //{
            //    if (previousVertices[i].Count == 0)
            //        previousVertices[i] = null;
            //}
            
            return ConstructPaths(previousVertices, startVertex, endVertex);

        }
        private int MinDistance(int[] distances, bool[] shortestPathSet)
        {
            int min = int.MaxValue, minIndex = -1;
            for (int v = 0; v < distances.Length; v++)
                if (!shortestPathSet[v] && distances[v] <= min)
                {
                    min = distances[v];
                    minIndex = v;
                }
            return minIndex;
        }

        //private List<int> ConstructPath(int[] previousVertices, int startVertex, int endVertex)
        //{
        //    List<int> path = new List<int>();
        //    for (int at = endVertex; at != -1; at = previousVertices[at])
        //        path.Add(at);
        //    path.Reverse();

        //    return path.Count > 1 && path[0] == startVertex ? path : new List<int>(); 
        //}
        private List<List<int>> ConstructPaths(List<int>[] previousVertices, int startVertex, int endVertex)
        {
            List<List<int>> allPaths = new List<List<int>>();
            FindAllPaths(previousVertices, startVertex, endVertex, new List<int>(), allPaths);
            return allPaths;
        }

        private void FindAllPaths(List<int>[] previousVertices, int currentVertex, int startVertex, List<int> path, List<List<int>> allPaths)
        {
            //MessageBox.Show("count " + previousVertices.Count().ToString());
            //for (int i = 0; i < previousVertices.Count(); i++)
            //        MessageBox.Show("v" + previousVertices[i].Count.ToString());



            //MessageBox.Show(startVertex.ToString());
            //MessageBox.Show("curV " + currentVertex.ToString());
            if (currentVertex == startVertex + 1)
            {
                path.Add(currentVertex - 1);
                //MessageBox.Show(path.Count().ToString());
                path.Reverse();
                allPaths.Add(new List<int>(path));
                path.Clear(); // Вернуть порядок обратно
                return;
            }
            else
            {
                if (previousVertices[currentVertex].Count > 0)
                {
                    //MessageBox.Show("flag");
                    //MessageBox.Show("add" + currentVertex.ToString());
                    //path.Add(currentVertex);
                    //previousVertices[currentVertex].Remove(currentVertex);
                    //foreach (int prev in previousVertices[currentVertex])
                    for (int i = 0; i < previousVertices[currentVertex].Count; i++)
                    {
                        var prev = previousVertices[currentVertex].ElementAt(i);
                        path.Add(prev);
                        previousVertices[currentVertex].Remove(prev);
                        //MessageBox.Show("prev " + prev.ToString());
                        FindAllPaths(previousVertices, prev, startVertex, path, allPaths);
                    }
                }
                else
                {
                    int count = 0;
                    for (int i = 0; i < previousVertices.Count() + 1; i++)
                        if (i == previousVertices.Count())
                        {
                            currentVertex = i;
                            count++;
                        }
                        else if (previousVertices[i].Count > 0)
                        {
                            currentVertex = i;
                            count++;
                            break;
                        }
                    //MessageBox.Show("count " + count.ToString());
                    if (count != 0)
                        FindAllPaths(previousVertices, currentVertex, startVertex, path, allPaths);
                }
            }
            //MessageBox.Show("count " + path.Count.ToString());
            //if (path.Count > 0)
            //    path.RemoveAt(path.Count - 1); // Удалить текущую вершину перед возвратом
        }
        public string GetSelectedColor()
        {
            if (mainWindow.BlackButton.IsChecked == true)
                return "Black";
            if (mainWindow.RedButton.IsChecked == true)
                return "Red";
            if (mainWindow.OrangeButton.IsChecked == true)
                return "Orange";
            if (mainWindow.YellowButton.IsChecked == true)
                return "Yellow";
            if (mainWindow.GreenButton.IsChecked == true)
                return "Green";
            if (mainWindow.CadetBlueButton.IsChecked == true)
                return "CadetBlue";
            return "Black";
        }
        public Brush ConvertStringToBrush(string colorName)
        {
            var property = typeof(Brushes).GetProperty(colorName);
            if (property != null)
                return (Brush)property.GetValue(null);
            else
                return Brushes.Black;
        }
    }
}
