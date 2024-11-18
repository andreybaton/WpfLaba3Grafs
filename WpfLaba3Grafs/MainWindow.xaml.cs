using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfLaba3Grafs
{

    public partial class MainWindow : Window
    {
        private FunctionsLogic function;
        private Point MousePos;
        private bool newVertex = false;
        private bool newEdge = false;
        private bool delete = false;
        public bool isOriented;
        public List<(int, int, int)> graphData = new List<(int, int, int)>(); // (int,int,int,Point)
        public Dictionary<int, Node> graph = new Dictionary<int, Node>();
        private Line tempLine;
        public MainWindow()
        {
            function = new FunctionsLogic(this);
            InitializeComponent();
        }
        public void PaintColor(object sender, RoutedEventArgs e)
        {
            bool isChecked = Bucket.IsChecked ?? false;
            if (isChecked == true) { 
                if (sender is Line line)
                {
                    string selectedColorName = function.GetSelectedColor();
                    line.Stroke = function.ConvertStringToBrush(selectedColorName);
                }
                else if (sender is Ellipse vertex)
                {
                    string selectedColorName = function.GetSelectedColor();
                    vertex.Stroke = function.ConvertStringToBrush(selectedColorName);
                }
            }
        }

        public void MouseLeftBtnDown_DrawingGraph(object sender, MouseButtonEventArgs e) 
        {
            MousePos = e.GetPosition(DrawingCanvas);
            if (newVertex)
            {
                Node node = new Node();
                if (!node.ContainsNode(MousePos, graph))
                {
                    node = node.AddOrGetNode(graph, graph.Count);
                    node.position = MousePos;
                    function.CreateVertex(MousePos, node);
                    graphData.Add((node.MyValue, -1, 0));

                    NodePicture nodePic = new NodePicture("", "Black", node);
                    if (!function.nodePictures.Keys.Contains(node))
                        function.nodePictures.Add(node, nodePic);
                }
            }
            else if (newEdge)
            {
                tempLine = new Line
                {
                    X1 = MousePos.X,
                    Y1 = MousePos.Y,
                    X2 = MousePos.X,
                    Y2 = MousePos.Y,
                    Stroke = function.ConvertStringToBrush(function.GetSelectedColor()),
                    StrokeThickness = 2,                  
                };
                DrawingCanvas.Children.Add(tempLine);
            }
            else if (delete)
            {
                if (DrawingCanvas != null && DrawingCanvas.InputHitTest(MousePos) != null)
                {
                    var element = DrawingCanvas.InputHitTest(MousePos) as UIElement;

                    if (element.GetType() == typeof(Ellipse))
                    {
                        //
                        var temp = DrawingCanvas.InputHitTest(MousePos) as UIElement;
                        while (temp != null && !(temp is Grid))
                            temp = VisualTreeHelper.GetParent(temp) as UIElement;
                        if (temp is Grid grid)
                            DrawingCanvas.Children.Remove(grid);

                        //
                        
                        for (int i = 0; i < graph.Count; i++)
                            if (graph.ElementAt(i).Value.AreNodesClose(MousePos, graph.ElementAt(i).Value.position, function.size/2 + 10))
                            {
                                for (int j = 0; j < graphData.Count; j++)
                                {
                                    if (graphData[j].Item2 == graph.ElementAt(i).Value.MyValue)
                                        graphData[j] = (graphData[j].Item1, -1, graphData[j].Item3);
                                    if (graphData[j].Item1 == graph.ElementAt(i).Value.MyValue)
                                    {
                                        graphData.RemoveAt(j);
                                        break;
                                    }
                                }
                                Node delNode = graph.ElementAt(i).Value;
                                function.nodePictures.Remove(delNode);
                                graph.Remove(graph.ElementAt(i).Key);
                                for (int k = 0; k < graph.Count; k++) 
                                {
                                        for (int l = 0; l < delNode.parents.Count; l++)
                                            if (graph.ElementAt(k).Value == delNode.parents.ElementAt(l).Key) //если вершина - предок
                                                graph.ElementAt(k).Value.edges.Remove(delNode.parents.ElementAt(l).Value);
                                            else
                                                for (int j = 0; j < delNode.edges.Count; j++)
                                                    if (graph.ElementAt(k).Value == delNode.edges.ElementAt(j).adjacentNode) //если вершина - потомок  
                                                        graph.ElementAt(k).Value.parents.Remove(graph.ElementAt(k).Value.parents.ElementAt(0).Key);
                                }

                                for (int k = delNode.MyValue; k < graph.Count; k++)
                                {
                                    int newV = graphData[k].Item1 - 1;
                                    graphData[k] = (newV, graphData[k].Item2, graphData[k].Item3);

                                    graph.ElementAt(k).Value.MyValue--;
                                    Node tempNode = graph.ElementAt(k).Value;
                                    graph.Remove(graph.ElementAt(k).Key);
                                    graph.Add(tempNode.MyValue, tempNode);
                                }

                                // + удаление картинки ребра
                                List <Line> delLines = new List <Line>();
                                if (delNode.parents.Count > 0)
                                    for (int j = 0; j < delNode.parents.Count; j++)
                                    {
                                        Line delLine = new Line
                                        {
                                            X1 = delNode.parents.ElementAt(j).Key.position.X,
                                            Y1 = delNode.parents.ElementAt(j).Key.position.Y,
                                            X2 = delNode.position.X,
                                            Y2 = delNode.position.Y,
                                        };
                                        delLines.Add(delLine);
                                    }
                                if (delNode.edges.Count > 0)
                                    for (int j = 0; j < delNode.edges.Count; j++)
                                    { 
                                        Line delLine = new Line
                                        {
                                            X1 = delNode.position.X,
                                            Y1 = delNode.position.Y,
                                            X2 = delNode.edges.ElementAt(j).adjacentNode.position.X,
                                            Y2 = delNode.edges.ElementAt(j).adjacentNode.position.Y,
                                        };
                                        delLines.Add(delLine);
                                }
                                for (int z = DrawingCanvas.Children.Count - 1; z >= 0; z--)
                                    if (DrawingCanvas.Children[z] is Line line)
                                        for(int w = delLines.Count - 1; w >= 0; w--)
                                            if(delLines[w].X1 == line.X1 && delLines[w].X2 == line.X2 && delLines[w].Y1 == line.Y1 && delLines[w].Y2 == line.Y2) {
                                                delLines.RemoveAt(w);
                                                DrawingCanvas.Children.RemoveAt(z);
                                                TextBox tbToRemove = line.Tag as TextBox;
                                                Polygon arrowToRemove = tbToRemove.Tag as Polygon;
                                                DrawingCanvas.Children.Remove(tbToRemove);
                                                try
                                                {
                                                    DrawingCanvas.Children.Remove(arrowToRemove);
                                                }
                                                catch { }
                                            }
                            }
                    }
                    if (element.GetType() == typeof(Line))
                    {
                        DrawingCanvas.Children.Remove(element);
                        Line line = (Line)element;
                        TextBox tbToRemove = line.Tag as TextBox;
                        Polygon arrowToRemove = tbToRemove.Tag as Polygon;
                        DrawingCanvas.Children.Remove(tbToRemove);
                        try
                        {
                            DrawingCanvas.Children.Remove(arrowToRemove);
                        }
                        catch { }
                        Point begin = new Point(line.X1, line.Y1);
                        Point end = new Point(line.X2, line.Y2);
                        for (int i = 0; i < graph.Count; i++)
                            if (graph.ElementAt(i).Value.AreNodesClose(begin, graph.ElementAt(i).Value.position, 10) || graph.ElementAt(i).Value.AreNodesClose(end, graph.ElementAt(i).Value.position, 10))
                            {
                                for (int j = 0; j < graphData.Count; j++)
                                    if (graphData[j].Item2 == graph.ElementAt(i).Value.MyValue)
                                    {
                                        graphData[j] = (graphData[j].Item1, -1, graphData[j].Item3);
                                        break;
                                    }


                                if (graph.ElementAt(i).Value.position == end)
                                {
                                    for (int k = 0; k < graph.Count; k++)
                                        for (int l = 0; l < graph.ElementAt(i).Value.parents.Count; l++)
                                            if (begin == graph.ElementAt(i).Value.parents.ElementAt(l).Key.position) //для конца ребра
                                                graph.ElementAt(i).Value.parents.Remove(graph.ElementAt(k).Value);
                                }
                                else if (graph.ElementAt(i).Value.position == begin)
                                    for (int j = 0; j < graph.Count; j++)
                                    {
                                        for (int k = 0; k < graph[i].edges.Count; k++) //для начала ребра
                                            if (graph.ElementAt(j).Value == graph[i].edges.ElementAt(k).adjacentNode && graph.ElementAt(j).Value.position == end)
                                                graph.ElementAt(i).Value.edges.Remove(graph[i].edges.ElementAt(k));
                                    }
                            }
                    }
                }
            }
        }
      
        private void MouseLeftButtonUp_DrawingGraph(object sender, MouseButtonEventArgs e) //for add edge
        {
             if (newEdge && tempLine!=null)
             {
                DrawingCanvas.Children.Remove(tempLine);
                Point secondMousePos = e.GetPosition(DrawingCanvas);
                tempLine.X2= secondMousePos.X;
                tempLine.Y2 = secondMousePos.Y;
                tempLine = null;
                if (string.IsNullOrEmpty(tbWeight.Text))
                    tbWeight.Text = "0";
                function.AddEdge(MousePos, secondMousePos, graph, graphData, Convert.ToInt32(tbWeight.Text));
            }
        }
        private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (tempLine != null)
            {
                Point currentPoint = e.GetPosition(DrawingCanvas);
                tempLine.X2 = currentPoint.X;
                tempLine.Y2 = currentPoint.Y;
            }
        }
        
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            var checkedButton = sender as ToggleButton;
            ResetToggleButtons(colors1, checkedButton);
            ResetToggleButtons(colors2, checkedButton);

            foreach(var child in (checkedButton.Parent as Panel).Children)
                if (child is ToggleButton button && button != checkedButton)
                    button.IsChecked = false;
        }

        private void ControlToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            var checkedButton = sender as ToggleButton;

            if (checkedButton == null)
                return;
            ResetToggleButtons(DockPanel1,checkedButton);
            //ResetToggleButtons(DockPanel2, checkedButton);
            ResetToggleButtons(DockPanel3, checkedButton);
            //ResetToggleButtons(DockPanel4, checkedButton);
            ResetToggleButtons(DockPanel5, checkedButton);
            if (checkedButton == Pointer)
            {
                //BtnClick_SelectItem();
            }

            else if (checkedButton == Vertex)
                newVertex = true;
            else if (checkedButton == Edge)
                newEdge = true;
            else if (checkedButton == Crest)
                delete = true;
            else if (checkedButton == Bucket)
                PaintColor(sender, e);
        }

        public void ResetToggleButtons(Panel panel, ToggleButton checkedButton)
        {
            foreach (var child in panel.Children)
                if (child is ToggleButton button && button != checkedButton)
                    button.IsChecked = false; 
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton pressed = (RadioButton)sender;

            if (pressed.Content.ToString() == "Ориентированный")
                isOriented = true;
            else
                isOriented = false;
        }
        
        public void BtnClick_GenerateIncidenceMatrix(object sender, RoutedEventArgs e)
        {
            function.Dg_BuildArr(function.GenerateIncidenceMatrix(graph), dg_graph);
        }
        public void BtnClick_GenerateAdjacencyMatrix(object sender, EventArgs e)
        {
            function.Dg_BuildArr(function.GenerateAdjacencyMatrix(graph), dg_graph);
        }
        public void BtnClick_arrGraph(object sender, EventArgs e)
        {
            graphData.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            dg_graph.ItemsSource = graphData.Select(t => new { from = t.Item1, to = t.Item2, weight = t.Item3 }).ToList();
        }
        public void BtnClick_SearchShortestPath(object sender, RoutedEventArgs e)
        {
            List<List<int>> allPaths = function.SearchShortestPaths(function.GenerateAdjacencyMatrix(graph), 0, 4, graph.Count);
            for (int count = 0; count < allPaths.Count; count++)
            {
                List<int> path = allPaths[count];
                
                if (path != null)
                    for (int i = 0; i < DrawingCanvas.Children.Count; i++)
                        if (DrawingCanvas.Children[i] is Grid grid)
                        {
                            Ellipse ellipse = (Ellipse)grid.Children[0];
                            for (int j = 0; j < path.Count; j++)
                                if (IsPointInsideEllipse(grid, Convert.ToInt32(graph.ElementAt(path[j]).Value.position.X), Convert.ToInt32(graph.ElementAt(path[j]).Value.position.Y)))
                                    ellipse.Fill = Brushes.Blue;
                        }
            }
        }
        private bool IsPointInsideEllipse(Grid grid, int x, int y)
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
        private void ControlToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            newEdge = false;
            newVertex = false;
            delete = false;
        }
    }
}
