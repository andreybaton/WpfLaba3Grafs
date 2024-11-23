﻿using System;
using System.Collections;
using System.Collections.Generic;
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
        private bool pointer = false;
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
        //public object nadomne(object sender, MouseButtonEventArgs e)
        //{
        //    var send = object;
        //    return send;
        //}
        //public MouseButtonEventArgs nado2(MouseButtonEventArgs e)
        //{
        //    var but = e as MouseButtonEventArgs;
        //    return but;
        //}
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

                    NodePicture nodePic = new NodePicture("", "Black");
                    node.nodePic = nodePic;
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
                                //function.nodePictures.Remove(delNode);
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
            else if (pointer)
            {
                var el = DrawingCanvas.InputHitTest(MousePos) as UIElement;
                //if (e != null)
                //{
                //    MessageBox.Show(el.GetType().ToString());
                //}
                if (el.GetType() == typeof(Line))
                {
                    Line line = (Line)el;
                    TextBox tb = line.Tag as TextBox;
                    //MessageBox.Show("a");
                    //TextBox tb = (TextBox)el;
                    tb.IsEnabled = true;
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
                pointer = true;
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
            int[,] arr = function.GenerateIncidenceMatrix(graph);
            if (arr == null) { tb_graph.Text = "Граф не имеет ребёр!"; return; }
            tb_graph.Text = "р/в".PadRight(arr.GetLength(1));
            for (int row = 0; row < arr.GetLength(0); row++)
            {
                if (row != 0)
                    tb_graph.Text += (row - 1).ToString().PadRight(arr.GetLength(0));
                for (int col = 0; col < arr.GetLength(1); col++)
                    tb_graph.Text += arr[row, col].ToString().PadRight(arr.GetLength(0));
                tb_graph.Text += '\n';
            } 
        }
        public void BtnClick_GenerateAdjacencyMatrix(object sender, EventArgs e)
        {
            int[,] arr = function.GenerateAdjacencyMatrix(graph);
            if (arr == null) { tb_graph.Text = "Граф не имеет вершин!"; return; }
            tb_graph.Text = "в/в".PadRight(arr.GetLength(1));
            for (int row = 0; row < arr.GetLength(0); row++)
            {
                for (int col = 0; col < arr.GetLength(1); col++)
                    tb_graph.Text += arr[row, col].ToString().PadRight(arr.GetLength(0));
                tb_graph.Text += '\n';
                if (row != arr.GetLength(0)-1)
                    tb_graph.Text += arr[0, row].ToString().PadRight(arr.GetLength(0));
            }
        }
        public void BtnClick_SearchShortestPath(object sender, RoutedEventArgs e)
        {
            ResetColour(graph);
            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
                if (DrawingCanvas.Children[i] is Grid grid)
                {
                    Ellipse ellipse = (Ellipse)grid.Children[0];
                    if (ellipse.Fill == Brushes.Blue)
                        ellipse.Fill = Brushes.White;
                }

            InputWindow iw = new InputWindow();
            int start = 0; int end = 0;
            iw.ShowDialog();
            if (iw.isOpen == false)
            {
                start = iw.GetStartV();
                end = iw.GetEndV();
                if (end == start)
                    return;
            }
            
            List<List<int>> allPaths = function.SearchPath(function.GenerateAdjacencyMatrix(graph), start, end); 
            if (allPaths == null || allPaths.Count == 0)
            {
                tb_graph.Text = "Граф не имеет путей из вершины " + start.ToString() + " в " + end.ToString();
                return;
            }

            for (int k = 0; k < allPaths.Count; k++)
            {
                List<int> path = allPaths[k];
                if (path != null && path.Count > 0)
                    for (int i = 0; i < DrawingCanvas.Children.Count; i++)
                        if (DrawingCanvas.Children[i] is Grid grid)
                        {
                            Ellipse ellipse = (Ellipse)grid.Children[0];
                            for (int j = 0; j < path.Count; j++)
                                if (function.IsPointInsideEllipse(grid, Convert.ToInt32(graph.ElementAt(path[j]).Value.position.X), Convert.ToInt32(graph.ElementAt(path[j]).Value.position.Y)))
                                {
                                    ellipse.Fill = Brushes.Blue;
                                    for (int v = 0; v < graph.Count; v++)
                                        if (graph.ElementAt(v).Value.MyValue == path[j])
                                            graph.ElementAt(v).Value.nodePic = new NodePicture(graph.ElementAt(v).Value.nodePic.tbNode, "Blue");
                                }
                        }
            }
        }
        
        public void BtnClick_SearchMaximumFlowProblem(object sender, RoutedEventArgs e)
        {
            ResetColour(graph);
            tb_graph.Clear();
            InputWindow iw = new InputWindow();
            int start = 0; int end = 0;
            iw.ShowDialog();
            if (iw.isOpen == false)
            {
                start = iw.GetStartV();
                end = iw.GetEndV();
                if (end == start)
                    return;
            }

            bool[] visited = new bool[graph.Count];
            function.FindRoutes(function.GenerateAdjacencyMatrix(graph), start, end, visited, "", tb_graph);
            List<List<int>> allPaths = function.ExtractListsFromTextBox(tb_graph.Text);
            if (allPaths == null) { MessageBox.Show("Нет доступных путей из вершины А в B"); return; }

            function.SearchMaximumFlowProblem(allPaths, graph, start);
            for (int k = 0; k < allPaths.Count; k++)
            {
                List<int> curPath = allPaths[k];
                curPath.Insert(0, start);
                if (curPath != null && curPath.Count > 0)
                    for (int i = 0; i < DrawingCanvas.Children.Count; i++)
                        if (DrawingCanvas.Children[i] is Grid grid)
                        {
                            Ellipse ellipse = (Ellipse)grid.Children[0];
                            for (int j = 0; j < curPath.Count; j++)
                                if (function.IsPointInsideEllipse(grid, Convert.ToInt32(graph.ElementAt(curPath[j]).Value.position.X), Convert.ToInt32(graph.ElementAt(curPath[j]).Value.position.Y)))
                                {
                                    ellipse.Fill = Brushes.Blue;
                                    for (int v = 0; v < graph.Count; v++)
                                        if (graph.ElementAt(v).Value.MyValue == curPath[j])
                                            graph.ElementAt(v).Value.nodePic = new NodePicture(graph.ElementAt(v).Value.nodePic.tbNode, "Blue");
                                }
                        }
            }
        }
        public void BtnClick_SearchMBST(object sender, EventArgs e)
        {
            ResetColour(graph);
            tb_graph.Clear();
            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
                if (DrawingCanvas.Children[i] is Grid grid)
                {
                    Ellipse ellipse = (Ellipse)grid.Children[0];
                    if (ellipse.Fill == Brushes.Blue)
                        ellipse.Fill = Brushes.White;
                }

            List<Edge> mbstEdges = function.SearchMBST(graph);
            if (mbstEdges.Count == 0 || mbstEdges == null)
            {
                tb_graph.Text = "Минимальное покрывающее дерево не найдено. Скорее всего, ваш граф не связный.";
                return;
            }
            else { tb_graph.Text = "Минимальное покрывающее дерево найдено."; }

            for (int obj = 0; obj < DrawingCanvas.Children.Count; obj++)
                if (DrawingCanvas.Children[obj] is Line line)
                    for (int edg = 0; edg < mbstEdges.Count; edg++)
                        if (function.IsPointOnLine(line, mbstEdges[edg].adjacentNode.position, 5))
                        {
                            line.Fill = Brushes.Blue;
                            for (int v = 0; v < graph.Count; v++)
                                if (graph.ElementAt(v).Value.edges.Contains(mbstEdges[edg]))
                                {
                                    graph.ElementAt(v).Value.edges.Remove(mbstEdges[edg]);
                                    mbstEdges[edg].edgePic = new EdgePicture(mbstEdges[edg].edgePic.tbEdge, "Blue");
                                    graph.ElementAt(v).Value.edges.Add(mbstEdges[edg]);
                                }
                                else //if (graph.ElementAt(v).Value.parents.Values.Contains(mbstEdges[edg])) {
                                    for (int i =0; i < graph.ElementAt(v).Value.parents.Count; i++)
                                        if (graph.ElementAt(v).Value.parents.ElementAt(i).Value == mbstEdges[edg])
                                            graph.ElementAt(v).Value.parents.ElementAt(i).Value.edgePic = new EdgePicture(mbstEdges[edg].edgePic.tbEdge, "Blue");
                                
                            //function.edgePictures[mbstEdges[edg]].colorEdge = "Blue";
                        }
        }
        public void ResetColour(Dictionary<int, Node> graph)
        {
            for (int V = 0; V < graph.Count; V++)
            {
                if (graph.ElementAt(V).Value.nodePic.colorNode == "Blue")
                    graph.ElementAt(V).Value.nodePic.colorNode = "White";
                for (int i = 0; i < graph.ElementAt(V).Value.edges.Count; i++)
                    if (graph.ElementAt(V).Value.edges.ElementAt(i).edgePic.colorEdge == "Blue")
                        graph.ElementAt(V).Value.edges.ElementAt(i).edgePic.colorEdge = "Black";
                for (int j = 0; j < graph.ElementAt(V).Value.parents.Count; j++)
                    if (graph.ElementAt(V).Value.parents.ElementAt(j).Value.edgePic.colorEdge == "Blue")
                        graph.ElementAt(V).Value.parents.ElementAt(j).Value.edgePic.colorEdge = "Black";
            }
        }
        private void ControlToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            pointer = false;
            newEdge = false;
            newVertex = false;
            delete = false;
        }

        //private void edgeSelector_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    int newWeight = 0;
        //    int index = Convert.ToInt32(EdgeSelector.Text);
        //    try { 
        //        newWeight = Convert.ToInt32(WeightChange.Text);
        //    }
        //    catch { }
        //    if (newWeight > 0) {
        //        function.edgePictures.ElementAt(index).Key.weight = Convert.ToInt32(WeightChange);
        //    }
        //}
    }
}
