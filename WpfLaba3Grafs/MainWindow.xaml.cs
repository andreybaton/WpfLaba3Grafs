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
        public List<(int, int, int)> graphData = new List<(int, int, int)>();
        public Dictionary<int, Node> graph = new Dictionary<int, Node>();
        private Line tempLine;
        public MainWindow()
        {
            function = new FunctionsLogic(this);
            InitializeComponent();
        }
        public void BtnClick_SelectItem()
        {

        }
        public void BtnClick_Paint(object sender, RoutedEventArgs e)
        {
            bool isChecked = Bucket.IsChecked ?? false;
            if (isChecked == true) { 
                if (sender is Line line)
                {
                    string selectedColorName = GetSelectedColor();
                    line.Stroke = ConvertStringToBrush(selectedColorName);
                }
                else if (sender is Ellipse vertex)
                {
                    string selectedColorName = GetSelectedColor();
                    vertex.Stroke = ConvertStringToBrush(selectedColorName);
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
                    function.CreateVertex(MousePos);
                    graphData.Add((node.value, -1, 0));

                    NodePicture nodePic = new NodePicture("", "Black", node);
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
                    Stroke = ConvertStringToBrush(GetSelectedColor()),
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
                        var temp = DrawingCanvas.InputHitTest(MousePos) as UIElement;
                        while (temp != null && !(temp is Grid))
                            temp = VisualTreeHelper.GetParent(temp) as UIElement;
                        if (temp is Grid grid)
                            DrawingCanvas.Children.Remove(grid);
                        for (int i = 0; i < graph.Count; i++)
                            if (graph.ElementAt(i).Value.AreNodesClose(MousePos, graph.ElementAt(i).Value.position, function.size/2 + 3))
                            {
                                for (int j = 0; j < graphData.Count; j++)
                                {
                                    if (graphData[j].Item2 == graph.ElementAt(i).Value.value)
                                        graphData[j] = (graphData[j].Item1, -1, graphData[j].Item3);
                                    if (graphData[j].Item1 == graph.ElementAt(i).Value.value)
                                    {
                                        graphData.RemoveAt(j);
                                        break;
                                    }
                                }
                                Node delNode = graph.ElementAt(i).Value;
                                graph.Remove(graph.ElementAt(i).Key);
                                for (int k = 0; k < graph.Count; k++)
                                {
                                    if (delNode.parents.Count > 0)
                                        if (graph.ElementAt(k).Value == delNode.parents.ElementAt(0).Key) //если вершина - предок
                                            graph.ElementAt(k).Value.edges.Remove(delNode.parents.ElementAt(0).Value);
                                    else
                                        for (int j = 0; j < delNode.edges.Count; j++)
                                        if (graph.ElementAt(k).Value == delNode.edges.ElementAt(j).adjacentNode) //если вершина - потомок  
                                            graph.ElementAt(k).Value.parents.Remove(graph.ElementAt(k).Value.parents.ElementAt(0).Key);
                                }
                            }
                    }
                    if (element.GetType() == typeof(Line))
                    {
                        DrawingCanvas.Children.Remove(element);
                        Line line = (Line)element;
                        Point begin = new Point(line.X1, line.Y1);
                        Point end = new Point(line.X2, line.Y2);
                        for (int i = 0; i < graph.Count; i++)
                            if (graph.ElementAt(i).Value.AreNodesClose(begin, graph.ElementAt(i).Value.position, 10) || graph.ElementAt(i).Value.AreNodesClose(end, graph.ElementAt(i).Value.position, 10))
                            {
                                for (int j = 0; j < graphData.Count; j++)
                                    if (graphData[j].Item2 == graph.ElementAt(i).Value.value)
                                    {
                                        graphData[j] = (graphData[j].Item1, -1, graphData[j].Item3);
                                        break;
                                    }

                                for (int j = 0; j < graph.Count; j++)
                                {
                                    if (graph.ElementAt(i).Value.position == end)
                                    {
                                        if (graph.ElementAt(j).Value == graph.ElementAt(i).Value.parents.ElementAt(0).Key) //для конца ребра
                                            graph.ElementAt(j).Value.parents.Remove(graph.ElementAt(i).Value);
                                    }
                                    else if (graph.ElementAt(i).Value.position == begin)
                                        for (int k = 0; k < graph[i].edges.Count; k++) //для начала ребра
                                            if (graph.ElementAt(j).Value == graph[i].edges.ElementAt(k).adjacentNode)
                                            {
                                                MessageBox.Show("remove");
                                                graph[j].edges.Remove(graph[i].edges.ElementAt(k));
                                            }
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
            ResetToggleButtons(DockPanel2, checkedButton);
            ResetToggleButtons(DockPanel3, checkedButton);
            ResetToggleButtons(DockPanel4, checkedButton);
            ResetToggleButtons(DockPanel5, checkedButton);
            if (checkedButton == Pointer)
                BtnClick_SelectItem();
            else if (checkedButton == Vertex)
                newVertex = true;
            else if (checkedButton == Edge)
                newEdge = true;
            else if (checkedButton == Crest)
                delete = true;
            else if (checkedButton == Bucket)
                BtnClick_Paint(sender, e);
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
            function.Dg_BuildArr(function.GenerateIncidenceMatrix(graph), dg_IncidenceMatrix);
        }
        public void BtnClick_GenerateAdjacencyMatrix(object sender, EventArgs e)
        {
            function.Dg_BuildArr(function.GenerateAdjacencyMatrix(graph), dg_AdjecencyMatrix);
        }
        public void BtnClick_arrGraph(object sender, EventArgs e) {
            graphData.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            dg_graph.ItemsSource = graphData.Select(t => new { from = t.Item1, to = t.Item2, weight = t.Item3 }).ToList();
        }
        public string GetSelectedColor()
        {
            if (BlackButton.IsChecked == true)
                return "Black";
            if (RedButton.IsChecked == true)
                return "Red";
            if (OrangeButton.IsChecked == true)
                return "Orange";
            if (YellowButton.IsChecked == true)
                return "Yellow";
            if (GreenButton.IsChecked == true)
                return "Green";
            if (CadetBlueButton.IsChecked == true)
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

        private void ControlToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            newEdge=false;
            newVertex=false;
        }
    }
}
