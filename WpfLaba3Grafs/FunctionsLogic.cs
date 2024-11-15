using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes; 
using System.Drawing;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections;
using System.Data;


namespace WpfLaba3Grafs
{
    public class FunctionsLogic 
    {
        public int size = 20;
        public bool newVertex = false;
        public bool newEdge = false;
        private MainWindow mainWindow;
        public List<EdgePicture> edgePictures = new List<EdgePicture>();
        public List<NodePicture> nodePictures = new List<NodePicture>();
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

            //Canvas.SetTop(vertex, posY);
            //Canvas.SetLeft(vertex, posX);
            //mainWindow.DrawingCanvas.Children.Add(vertex);
            TextBlock label = new TextBlock
            {
                Text = mainWindow.graph.Count().ToString(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontWeight = FontWeights.Bold,
            };
            //Canvas.SetTop(label, posY);
            //Canvas.SetLeft(label, posX+5);
            var grid = new Grid();
            grid.Children.Add(vertex);
            grid.Children.Add(label);
            Canvas.SetTop(grid, posY);
            Canvas.SetLeft(grid, posX + 5);
            mainWindow.DrawingCanvas.Children.Add(grid);
            //mainWindow.DrawingCanvas.Children.Add(label);
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
                if (edge2.AddEdge(graphData, from, to, weight, mainWindow.typeEdge))
                {
                    //100=(x-from.postion.item1)^2+(y-from.position.item2)^2 100=(x-to.postion.item1)^2+(y-to.position.item2)^2
                    CreateEdge(from.position, to.position);
                    TextBox textBox = new TextBox
                    {
                        Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
                        BorderThickness = new Thickness(0, 0, 0, 0),
                        Width = 60,
                        Height = 18
                    };
                    string tb = "№ " + CalculateEdges(graphData).ToString();
                    if (weight != 0)
                        tb = tb + "; Вес " + weight.ToString();
                    textBox.Text = tb;
                    textBox.IsEnabled = false;
                    Canvas.SetLeft(textBox, (pos1.X + pos2.X) / 2);
                    Canvas.SetTop(textBox, (pos1.Y + pos2.Y) / 2);

                    mainWindow.DrawingCanvas.Children.Add(textBox);

                    Edge edge = new Edge(to, weight);
                    EdgePicture edgePic = new EdgePicture(textBox.Text, "Black", edge);
                    edgePictures.Add(edgePic);
                }
        }
        private void EdgeLine(Point ellipse1, Point ellipse2, double radius)
        {
            double distance = 0;
            double d = Math.Sqrt((ellipse2.X - ellipse1.X) * (ellipse2.X - ellipse1.X) - (ellipse2.Y - ellipse1.Y) * (ellipse2.Y - ellipse1.Y));
            if (d > radius * 2) 
                distance = d - radius * 2;
        }
        public void CreateEdge(Point pos1, Point pos2)
        {
            string selectedColorName = mainWindow.GetSelectedColor();
            Brush strokeBrush = mainWindow.ConvertStringToBrush(selectedColorName);
            Line edge = new Line()
            {
                X1 = pos1.X,
                Y1 = pos1.Y,
                X2 = pos2.X,
                Y2 = pos2.Y,
                Stroke = strokeBrush,
                StrokeThickness = 2
            };
            edge.MouseDown += mainWindow.BtnClick_Paint;
            mainWindow.DrawingCanvas.Children.Add(edge);
            if (mainWindow.typeEdge == true)
                mainWindow.DrawingCanvas.Children.Add(DrawArrow(pos1, pos2));
            newEdge = false;
        }
        public Polygon DrawArrow(Point pos1, Point pos2)
        {
            double arrowLength = 10; // Длина стрелки
            double angle = Math.Atan2(pos2.Y - pos1.Y, pos2.X - pos1.X); // Угол в радианах

            // Создаем стрелку
            Polygon arrowHead = new Polygon
            {
                Fill = Brushes.Black,
                Points = new PointCollection
                {
                    new Point(pos2.X, pos2.Y), // Конечная точка линии
                    new Point(pos2.X - arrowLength * Math.Cos(angle - Math.PI / 6),
                    pos2.Y - arrowLength * Math.Sin(angle - Math.PI / 6)), // Левый угол стрелки

                    new Point(pos2.X - arrowLength * Math.Cos(angle + Math.PI / 6),
                    pos2.Y - arrowLength * Math.Sin(angle + Math.PI / 6))  // Правый угол стрелки
                }
            };
            return arrowHead;
        }
        public void GenerateIncidenceMatrix(List<(int, int, int)> graphData, Dictionary<int, Node> graph)
        {
            graphData.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            int numEdges = CalculateEdges(graphData);
            if (numEdges <= 0)
                return;
            int[,] matrix = new int[graph.Count+1,numEdges];
            //MessageBox.Show("row " + matrix.GetLength(0));
            //MessageBox.Show("column " + matrix.GetLength(1));
            for (int i = 0; i < numEdges; i++)
            {
                matrix[0, i] = i+1;
                for (int j = 1; j < graph.Count; j++)
                    matrix[j,i] = 0;
            }


            var (start, end, weight) = graphData[0]; int retry=0, step = 0;
            for (int i = 0; i < numEdges; i++)
            {
                if (end != -1)
                {
                    if (graphData[i].Item1 == start)
                    {
                        while (graphData[retry].Item1 == graphData[i+step].Item1)
                        {
                            matrix[start + 1, i + retry] = 1;
                            
                            if (mainWindow.typeEdge == true)
                                matrix[end + 1, i + retry] = -1;
                            else
                                matrix[end + 1, i + retry] = 1;
                            retry++;
                            (start, end, weight) = graphData[retry];
                        }
                        step = step + retry; 
                    }
                    else
                    {
                        matrix[start + 1, i] = 1; 
                        matrix[end + 1, i] = -1;   
                    }
                    if (i+step < graphData.Count)
                        (start, end, weight) = graphData[i + step];
                }
                else
                {
                    while (graphData[i + step].Item2 == -1)
                    {
                        if (i + step < graphData.Count)
                            step++;
                        else
                            break;
                    }
                    (start, end, weight) = graphData[i + step];
                }
            }

            DataTable dataTable = new DataTable();
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
            mainWindow.dg_IncidenceMatrix.ItemsSource = dataTable.DefaultView;
        }
        //функцию сортировки матрицы когда меняешь индекс у ребра
        private int CalculateEdges(List<(int, int, int)> graphData)
        {
            int count = 0;
            HashSet<(int,int)> edges = new HashSet<(int,int)> ();
            foreach(var row in graphData)
            {
                if (row.Item2 != -1)
                {
                    var edge = row.Item1 < row.Item2 ? (row.Item1, row.Item2) : (row.Item2, row.Item1);
                    if (edges.Add(edge))
                        count++;
                }
            }
            return count;
        }
        public void GenerateAdjacencyMatrix(List<(int, int, int)> graphData, Dictionary<int, Node> graph)
        {
            int[,] matrix = new int[graph.Count + 1, graph.Count];
            for (int i = 0; i < graph.Count; i++)
            {
                matrix[0, i] = graph.ElementAt(i).Value.value;
                for (int j = 1; j < graph.Count; j++)
                    matrix[i, j] = 0;
            }
            foreach (var row in graphData)
                if (row.Item2 != -1)
                    matrix[row.Item1 + 1, row.Item2] = 1;
            DataTable dataTable = new DataTable();

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
            mainWindow.dg_AdjecencyMatrix.ItemsSource = dataTable.DefaultView;
        }
    }
}
