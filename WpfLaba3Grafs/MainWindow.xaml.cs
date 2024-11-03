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
        private FunctionsMainWindow function;
        
        private Graph graph;
        private Point MousePos;
        private bool newVertex = false;
        private bool newEdge = false;
        public MainWindow()
        {
            function = new FunctionsMainWindow(this);
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
        public void MouseLeftBtnDown_DrawingGraph(object sender, MouseButtonEventArgs e)
        {
            MousePos = e.GetPosition(DrawingCanvas);
            if (newVertex)
            {
                Node newNode = new Node(graph.Nodes.Count, MousePos);
                if (graph.AddNode(newNode))
                    function.CreateVertex(MousePos);
                newVertex = false;
            }
        }
      
        private void MouseLeftButtonUp_DrawingGraph(object sender, MouseButtonEventArgs e)
        {
             if (newEdge)
             {
                Point secondMousePos = e.GetPosition(DrawingCanvas);
                newEdge = false;

                Node from = new Node(); Node to = new Node();
                for (int i = 0; i < graph.Nodes.Count; i++) {
                    if (graph.AreNodesClose(MousePos, graph.Nodes[i].Position, 5))
                        from = graph.Nodes[i];
                    else if (graph.AreNodesClose(secondMousePos, graph.Nodes[i].Position, 5))
                        to = graph.Nodes[i];
                }
                if (graph.AddEdge(from, to))
                {
                    function.CreateEdge(MousePos, secondMousePos);
                    TextBox textBox = new TextBox
                    {
                        Width = 14,
                        Height = 18
                    };
                    Canvas.SetLeft(textBox, (MousePos.X + secondMousePos.X)/2);
                    Canvas.SetTop(textBox, (MousePos.Y + secondMousePos.Y) / 2);
                    textBox.Text = graph.Edges[graph.Edges.Count-1].weight.ToString();
                    DrawingCanvas.Children.Add(textBox);
                    if (graph.Edges.Count > 1)
                    {
                        graph.Merge(graph);
                        DrawingCanvas.Children.Clear();
                        function.ReDrawGraph(graph);
                    }
                }
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
        public Brush GetSelectedColor()
        {
            if (BlackButton.IsChecked==true)
                return Brushes.Black;
            if (RedButton.IsChecked == true)
                return Brushes.Red;
            if (OrangeButton.IsChecked == true)
                return Brushes.Orange;
            if (YellowButton.IsChecked == true)
                return Brushes.Yellow;
            if (GreenButton.IsChecked == true)
                return Brushes.Green;
            if (CadetBlueButton.IsChecked == true)
                return Brushes.CadetBlue;
            return Brushes.Black;
        }
    }
}
