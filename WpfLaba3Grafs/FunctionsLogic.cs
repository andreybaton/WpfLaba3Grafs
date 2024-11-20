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
using System.Windows.Input;


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
        public void CreateVertex(Point position, Node node) //создаем вершину
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
            vertex.MouseDown += mainWindow.PaintColor; //подписываем vertex(вершину) на событие изменения цвета
            double posX = position.X - vertex.Width / 2;
            double posY = position.Y - vertex.Height / 2;

            TextBlock label = new TextBlock //создаем текстблок - номер вершины
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

            var grid = new Grid(); //засовываем вершину и текстбокс в один объект grid
            grid.Children.Add(vertex);
            grid.Children.Add(label);

            Canvas.SetTop(grid, posY);
            Canvas.SetLeft(grid, posX);
            mainWindow.DrawingCanvas.Children.Add(grid);
        }
        public void AddEdge(Point pos1, Point pos2, Dictionary<int, Node> graph, List<(int, int, int)> graphData, int weight) //создаем ребро
        {
            Node from = new Node(); Node to = new Node();
            for (int i = 0; i < graph.Count; i++) //если ребро между вершинами, то меняем координаты начала и конца ребра на координаты вершины, чтобы было по центру
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
                    edge.MouseDown += mainWindow.PaintColor;
                    Polygon polygon = new Polygon();                   

                    newEdge = false;
                    TextBox textBox = new TextBox
                    {
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0, 0, 0, 0),
                        Width = 60,
                        Height = 18,
                        IsReadOnly = false,
                        IsHitTestVisible = true,
                    };
                    string tb = "№ " + CalculateEdges(graph).ToString();
                    if (weight != 0)
                        tb = tb + "; Вес " + weight.ToString();
                    textBox.Text = tb;
                    textBox.IsEnabled = false;

                    textBox.PreviewMouseDown += (object sender, MouseButtonEventArgs e) =>
                    {
                        MessageBox.Show("a");
                        var clickedTb = sender as TextBox;
                        if (clickedTb != null)
                            clickedTb.Focus();
                    };

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

        public List<List<int>> SearchPath(int[,] AdjacencyMatrix, int startVertex, int endVertex)//alg Deikstra
        {
            int verticesCount = AdjacencyMatrix.GetLength(1);
            int[] distances = new int[verticesCount];
            bool[] visited = new bool[verticesCount];
            List<int>[] paths = new List<int>[verticesCount];
            List<List<int>> resultPaths = new List<List<int>>();

            // Инициализация
            for (int i = 0; i < verticesCount; i++)
            {
                distances[i] = int.MaxValue;
                visited[i] = false;
                paths[i] = new List<int>();
            }

            distances[startVertex] = 0;
            paths[startVertex].Add(startVertex);

            for (int count = 0; count < verticesCount - 1; count++)
            {
                int u = MinDistance(distances, visited);
                if (u == -1) break;
                visited[u] = true;

                for (int v = 0; v < verticesCount; v++)
                    if (!visited[v] && AdjacencyMatrix[u+1, v] != 0 &&
                        distances[u] != int.MaxValue)
                    {
                        int newDist = distances[u] + AdjacencyMatrix[u+1, v];
                        if (newDist < distances[v])
                        {
                            distances[v] = newDist;
                            paths[v] = new List<int>(paths[u]) { v };
                        }
                        else if (newDist == distances[v])
                        {
                            paths[v].AddRange(paths[u]);
                            paths[v].Add(v);
                        }
                    }
            }
            foreach (var path in paths[endVertex])
                if (path != endVertex)
                {
                    List<int> altPath = new List<int>(paths[path]) { endVertex };
                    if (!resultPaths.Contains(altPath))
                        resultPaths.Add(altPath);
                }
            return resultPaths;
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
        //private List<List<int>> ConstructAllPaths(List<int>[] previousVertices, int startVertex, int endVertex)
        //{
        //    List<List<int>> paths = new List<List<int>>();
        //    FindAllPaths(previousVertices, endVertex, new List<int>(), paths);

        //    // Добавляем начальную вершину к каждому найденному пути
        //    foreach (var path in paths)
        //    {
        //        path.Add(startVertex);
        //        path.Reverse();
        //    }

        //    return paths;
        //}

        //public static List<List<int>> SearchShortestPaths(int[,] adjacencyMatrix, int startVertex, int endVertex)
        //{
        //    int n = adjacencyMatrix.GetLength(1);
        //    List<Vertex> vertices = new List<Vertex>();

        //    for (int i = 0; i < n; i++)
        //        vertices.Add( new Vertex(i, int.MaxValue, new List<int>(), new List<(int, int)>()) );

        //    vertices[startVertex].Distance = 0;
        //    vertices[startVertex].Path.Add(startVertex);
        //    HashSet<int> visited = new HashSet<int>();

        //    while (visited.Count < n)
        //    {
        //        Vertex current = vertices.Where(v => !visited.Contains(v.Index)).OrderBy(v => v.Distance).FirstOrDefault();
        //        if (current == null)
        //            break;

        //        visited.Add(current.Index);

        //        for (int i = 0; i < n; i++)
        //            if (adjacencyMatrix[current.Index+1, i] > 0 && !visited.Contains(i))
        //            {
        //                int distance = current.Distance + adjacencyMatrix[current.Index+1, i];

        //                if (distance < vertices[i].Distance)
        //                {
        //                    vertices[i].Distance = distance;
        //                    vertices[i].Path.Clear();
        //                    vertices[i].Path.AddRange(current.Path);
        //                    vertices[i].Path.Add(i);
        //                }
        //                else if (distance == vertices[i].Distance) //newpath
        //                {
        //                    List<int> newPath = new List<int>(current.Path);
        //                    newPath.Add(i);
        //                    vertices[i].Path.AddRange(newPath);
        //                }
        //            }
        //    }
        //    List<List<int>> allPath = vertices[endVertex].Path.Select(p => vertices[p].Path).ToList();
        //    return allPath;
        //}

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
        public List<List<int>> SearchMaximumFlowProblem(int[,] AdjacencyMatrix, int startVertex, int endVertex, int allVertices, Dictionary<int,Node> graph) //Ford-Falkerson alg
        {
            int maxFlowProblem = 0;
            int weight = int.MaxValue;
            List<List<int>> allPaths = new List<List<int>>();
            allPaths = SearchPath(AdjacencyMatrix, startVertex, endVertex);

            for (int num = 0; num < allPaths.Count; num++)
            {
                List<int> path = allPaths[num];
                for (int q = 0; q < allPaths[num].Count; q++)
                    MessageBox.Show(allPaths[num].ElementAt(q).ToString());
                for (int v = 0; v < path.Count - 1; v++)
                    for (int i = 0; i < graph[path[v]].edges.Count; i++)
                        if (v != path.Count - 1)
                            if (graph[path[v]].edges.ElementAt(i).adjacentNode.MyValue == path[v + 1] && weight > graph[path[v]].edges.ElementAt(i).weight)
                                weight = graph[path[v]].edges.ElementAt(i).weight;
                //MessageBox.Show(weight.ToString());
                maxFlowProblem = maxFlowProblem + weight;
            }
            //    allPaths.Add(path);
            //    path = SearchPath(AdjacencyMatrix, startVertex, endVertex, allVertices);
            //}


            MessageBox.Show("result " + maxFlowProblem.ToString());
            return allPaths;
        }
        public bool IsPointInsideEllipse(Grid grid, int x, int y)
        {
            double left = Canvas.GetLeft(grid);
            double top = Canvas.GetTop(grid);

            Ellipse ellipse = (Ellipse)grid.Children[0];
            double width = ellipse.Width;
            double height = ellipse.Height;

            double h = left + width / 2;
            double k = top + height / 2;
            double r = width / 2;

            return (Math.Pow(x - h, 2) / Math.Pow(r, 2)) + (Math.Pow(y - k, 2) / Math.Pow(r, 2)) <= 1;
        }
    }
    class Vertex
    {
        public int Index { get; set; }
        public int Distance { get; set; }
        public List<int> Path { get; set; }
        public List<(int, int)> Edges = new List<(int, int)>(); //prev, current

        public Vertex(int index, int distance, List<int> path, List<(int, int)> edges)
        {
            Index = index;
            Distance = distance;
            Path = new List<int>(path);
            Edges = edges;
        }
    }
}
