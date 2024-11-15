using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfLaba3Grafs
{

    public partial class MainWindow : Window
    {
        private FunctionsLogic function;
        //private Graph graph;
        private Point MousePos;
        private bool newVertex = false;
        private bool newEdge = false;
        private bool delete = false;
        public bool typeEdge;
        public List<(int, int, int)> graphData = new List<(int, int, int)>();
        public Dictionary<int, Node> graph = new Dictionary<int, Node>();
        private Line tempLine;
        List<(int, int, int)> tuples = new List<(int, int, int)>();
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
            bool kostul = Bucket.IsChecked ?? false;
            if (kostul == true) { 
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

        public void MouseLeftBtnDown_DrawingGraph(object sender, MouseButtonEventArgs e) //for add vertex
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
                    function.nodePictures.Add(nodePic);
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
                if(DrawingCanvas != null)
                {
                    var element = DrawingCanvas.InputHitTest(MousePos) as UIElement;
                    
                        DrawingCanvas.Children.Remove(element);
                    //MessageBox.Show(element.GetType().ToString());
                    if (element.GetType() == typeof(Ellipse))
                    {
                        for (int i = 0; i < graph.Count; i++)
                            if (graph.ElementAt(i).Value.AreNodesClose(MousePos, graph.ElementAt(i).Value.position, 10))
                            {
                                for (int j = 0; j < graphData.Count; j++)
                                    if (graphData[j].Item1 == graph.ElementAt(i).Value.value)
                                    {
                                        graphData.RemoveAt(j);
                                        break;
                                    }
                                graph.Remove(graph.ElementAt(i).Key);
                            }
                    }
                    if (element.GetType() == typeof(Line))
                    {
                        Line line = (Line) element;
                        Point begin = new Point(line.X1, line.Y1);
                        Point end = new Point(line.X2, line.Y2);
                        for (int i = 0; i < graph.Count; i++)
                            if (graph.ElementAt(i).Value.AreNodesClose(begin, graph.ElementAt(i).Value.position, 10) || graph.ElementAt(i).Value.AreNodesClose(end, graph.ElementAt(i).Value.position, 10))
                            {
                                for (int j = 0; j < graphData.Count; j++)
                                    if (graphData[j].Item1 == graph.ElementAt(i).Value.value)
                                    {
                                        graphData.RemoveAt(j);
                                        break;
                                    }
                                graph.Remove(graph.ElementAt(i).Key);
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
                typeEdge = true;
            else
                typeEdge = false;
        }
        
        public void BtnClick_GenerateIncidenceMatrix(object sender, RoutedEventArgs e)
        {
            function.GenerateIncidenceMatrix(graphData, graph);
        }
        public void BtnClick_GenerateAdjacencyMatrix(object sender, EventArgs e)
        {
            function.GenerateAdjacencyMatrix(graphData, graph);
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
