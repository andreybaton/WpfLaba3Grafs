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
        public bool typeEdge;
        public List<(int, int, int)> graphData = new List<(int, int, int)>();
        public Dictionary<int, Node> graph = new Dictionary<int, Node>();
        private Line tempLine;
        List<(int, int, int)> tuples = new List<(int, int, int)>();
        public MainWindow()
        {
            function = new FunctionsLogic(this);
            //graph = new Graph();
            InitializeComponent();
        }
        public void BtnClick_SelectItem()
        {

        }
        public void BtnClick_CreateVertex()
        {   
            newEdge=false;
            newVertex = true;
        }
        public void BtnClick_CreateEdge()
        {
            newVertex= false;
            newEdge = true;
        }
        public void BtnClick_DeleteElement()
        {

        }
        public void MouseLeftBtnDown_DrawingGraph(object sender, MouseButtonEventArgs e) //for add vertex
        {
            MousePos = e.GetPosition(DrawingCanvas);
            if (newVertex)
            {
                //newVertex = false;
                Node node = new Node();
                if (!node.ContainsNode(MousePos, graph))
                {
                    node = node.AddOrGetNode(graph, graph.Count);
                    node.position = MousePos;
                    function.CreateVertex(MousePos);
                    graphData.Add((node.value, -1, graph.Count)); 

                }
            }
            if (newEdge)
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
                //DrawingCanvas.MouseMove += DrawingCanvas_MouseMove;
                //DrawingCanvas.MouseUp += MouseLeftButtonUp_DrawingGraph;
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
                newEdge = false;
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

        public void Paint_MouseDown(object sender, RoutedEventArgs e)
        {
            if(sender is Line line)
            {
                string selectedColorName = GetSelectedColor();
                line.Stroke = ConvertStringToBrush(selectedColorName);
            }
            else if(sender is Ellipse vertex)
            {
                string selectedColorName = GetSelectedColor();
                vertex.Stroke = ConvertStringToBrush(selectedColorName);
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
            {
                newEdge = false;
                newVertex = false;
                return;
            }
            ResetToggleButtons(DockPanel1,checkedButton);
            ResetToggleButtons(DockPanel2, checkedButton);
            ResetToggleButtons(DockPanel3, checkedButton);
            ResetToggleButtons(DockPanel4, checkedButton);
            ResetToggleButtons(DockPanel5, checkedButton);
            if (checkedButton == Pointer)
            {
                BtnClick_SelectItem();
                
            }
            else if (checkedButton == Vertex)
            {

                BtnClick_CreateVertex();

            }
            else if (checkedButton == Edge)
            {
                BtnClick_CreateEdge();
            }
            else if (checkedButton == Crest)
            {
                BtnClick_DeleteElement();
            }
            else if (checkedButton == Bucket)
            {
                Paint_MouseDown(sender, e);
            }
        }

        public void ResetToggleButtons(Panel panel, ToggleButton checkedButton)
        {
            foreach (var child in panel.Children)
                if (child is ToggleButton button && button != checkedButton)
                {
                    button.IsChecked = false;
                    newEdge=false;
                    newVertex=false;
                }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton pressed = (RadioButton)sender;

            if (pressed.Content.ToString() == "Ориентированный")
                typeEdge = true;
            else
                typeEdge = false;
        }
        
        public void GenerateAdjacencyMatrix(List<(int,int,int)> graphData, Dictionary<int, Node> graph)
        {
            int[,] matrix = new int[graph.Count+1, graph.Count];
            for (int i = 0; i < graph.Count; i++)
            {
                matrix[0, i] = graph.ElementAt(i).Value.value;
                for (int j = 1; j < graph.Count; j++)
                    matrix[i, j] = 0;
            }
            foreach (var row in graphData)
                if (row.Item2 != -1)
                    matrix[row.Item1+1, row.Item2] = 1;
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
            dg_AdjecencyMatrix.ItemsSource = dataTable.DefaultView;
        }
        public void BtnClick_GenerateAdjacencyMatrix(object sender, EventArgs e)
        {
            GenerateAdjacencyMatrix(graphData, graph);
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
