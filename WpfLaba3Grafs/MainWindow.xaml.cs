using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        public Point MousePos;

        public MainWindow()
        {
            InitializeComponent();
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
            OnMouseLeftBtnDown_DrawingGraph(sender, e);
        }
        public bool newVertex = false;
        public bool newEdge = false;

        private void CreateVertex(Point position)
        {
            Ellipse vertex = new Ellipse()
            {
                Width = 10,
                Height = 10,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            double posX = position.X - vertex.Width / 2;          
            double posY = position.Y - vertex.Height / 2;

            Canvas.SetTop(vertex, posY);
            Canvas.SetLeft(vertex, posX);
            DrawingCanvas.Children.Add(vertex);
        }

        private void CreateEdge(Point pos1, Point pos2)
        {
            if (newEdge)
            {
                Line edge = new Line()
                {
                    X1 = pos1.X,
                    Y1 = pos1.Y,
                    X2 = pos2.X,
                    Y2 = pos2.Y,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };
                DrawingCanvas.Children.Add(edge);
                newEdge = false;
            }
        }
        public void OnMouseLeftBtnDown_DrawingGraph(object sender, MouseButtonEventArgs e)
        {
            MousePos = e.GetPosition(DrawingCanvas);
            if (newVertex)
            {
                CreateVertex(MousePos);
                newVertex = false;
            }
            
        }

        private void MouseLeftButtonUp_DrawingGraph(object sender, MouseButtonEventArgs e)
        {
             if (newEdge)
             {
                //Point mousePosition = e.GetPosition(DrawingCanvas);
                Point secondMousePos = e.GetPosition(DrawingCanvas);
                CreateEdge(MousePos, secondMousePos);
                newEdge = false;
             }
        }
    }
}
