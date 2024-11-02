using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes; // для фигур
using System.Drawing;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace WpfLaba3Grafs
{
    public class FunctionsMainWindow : MainWindow
    {
        private bool newVertex = false;
        private bool newEdge = false;
        public FunctionsMainWindow()
        {
            //MainWindow mainWindow = MainWindow;
        }
        public void BtnClick_CreateVertex(object sender, RoutedEventArgs e)
        {
             newVertex = true;
        }
        private void CreateVertex(Point position)
        {
            Ellipse node = new Ellipse()
            {
                Width = 20,
                Height = 20,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            Canvas.SetLeft(node, position.X - node.Width / 2);
            Canvas.SetTop(node, position.Y - node.Height / 2);

            // Добавляем круг на Canvas
            DrawingCanvas.Children.Add(node);
        }
        public void BtnClick_CreateEdge(object sender, RoutedEventArgs e)
        {
            newEdge = true;
        }
        private void CreateEdge(Point position) 
        {
            
        }
        public void MouseLeftBtnDown_DrawingGraph (object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(DrawingCanvas);
            if (newVertex)
            {
                CreateVertex(mousePosition);
                newVertex = false;
            }
            else if (newEdge)
            {
                CreateEdge(mousePosition);
                newEdge = false;
            }
        }
    }
}
