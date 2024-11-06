using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfLaba3Grafs
{

    public partial class MainWindow : Window
    {
        private FunctionsLogic function;
        private Graph graph;
        private Point MousePos;
        private bool newVertex = false;
        private bool newEdge = false;
        public bool typeEdge;
        public List<(int, int, int)> graphData = new List<(int, int, int)>();
        public Dictionary<int, Node2> graph2 = new Dictionary<int, Node2>();
        private Line tempLine;
        public MainWindow()
        {
            function = new FunctionsLogic(this);
            graph = new Graph();
            InitializeComponent();
        }
        public void BtnClick_DeleteElement(object sender, RoutedEventArgs e)
        {

        }
        public void BtnClick_CreateVertex(object sender, RoutedEventArgs e)
        {
            newVertex = true;
        }
        public void BtnClick_CreateEdge(object sender, RoutedEventArgs e)
        {
            newEdge = true;
        }
        public void MouseLeftBtnDown_DrawingGraph(object sender, MouseButtonEventArgs e) //for add vertex
        {
            MousePos = e.GetPosition(DrawingCanvas);
            if (newVertex)
            {
                newVertex = false;
                Node2 node2 = new Node2();
                if (!node2.ContainsNode(MousePos, graph2))
                {
                    node2 = node2.AddOrGetNode(graph2, graph2.Count);
                    node2.position = MousePos;
                    function.CreateVertex(MousePos);
                    graphData.Add((node2.value, -1, graph2.Count));
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
                function.AddEdge(MousePos, secondMousePos, graph2, graphData, Convert.ToInt32(tbWeight.Text));
                
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
            {
                if (child is ToggleButton button && button != checkedButton)
                {
                    button.IsChecked = false;
                }
            }
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {

        }
        
        
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton pressed = (RadioButton)sender;

            if (pressed.Content.ToString() == "Ориентированный")
            {
                typeEdge = true;
            }
            else
            {
                typeEdge = false;
            }
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
            {
                return (Brush)property.GetValue(null);
            }
            else
            {
                return Brushes.Black;
            }
        }

        
        //public Brush GetSelectedColor()
        //{
        //    if (BlackButton.IsChecked==true)
        //        return Brushes.Black;
        //    if (RedButton.IsChecked == true)
        //        return Brushes.Red;
        //    if (OrangeButton.IsChecked == true)
        //        return Brushes.Orange;
        //    if (YellowButton.IsChecked == true)
        //        return Brushes.Yellow;
        //    if (GreenButton.IsChecked == true)
        //        return Brushes.Green;
        //    if (CadetBlueButton.IsChecked == true)
        //        return Brushes.CadetBlue;
        //    return Brushes.Black;
        //}
    }
}
