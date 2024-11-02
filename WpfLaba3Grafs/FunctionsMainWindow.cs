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
using System.Collections;

namespace WpfLaba3Grafs
{
    public class FunctionsMainWindow : MainWindow
    {
        //public bool newVertex = false;
        //public bool newEdge = false;
        
        //private void CreateVertex(Point position)
        //{
        //    Ellipse vertex = new Ellipse()
        //    {
        //        Width = 20,
        //        Height = 20,
        //        Stroke = Brushes.Black,
        //        StrokeThickness = 2
        //    };
        //    Canvas.SetLeft(vertex, position.X - vertex.Width / 2);
        //    Canvas.SetTop(vertex, position.Y - vertex.Height / 2);

        //    DrawingCanvas.Children.Add(vertex);
        //}
        
        //private void CreateEdge(Point pos1, Point pos2) 
        //{
        //    Line edge = new Line()
        //    {
        //        X1 = pos1.X,
        //        Y1 = pos1.Y,
        //        X2 = pos2.X,
        //        Y2 = pos2.Y,
        //        Stroke = Brushes.Black,
        //        StrokeThickness = 2
        //    };
        //    DrawingCanvas.Children.Add(edge);
        //}
        //public void OnMouseLeftBtnDown_DrawingGraph (object sender, MouseButtonEventArgs e)
        //{
        //    Point mousePosition = e.GetPosition(DrawingCanvas);
        //    MessageBox.Show(mousePosition.ToString());
        //    MessageBox.Show(newVertex.ToString());
        //    if (newVertex)
        //    {
        //        CreateVertex(mousePosition);
        //        newVertex = false;
        //    }
        //    else if (newEdge)
        //    {
        //        Point secondMousePos = e.GetPosition(DrawingCanvas);
        //        CreateEdge(mousePosition, secondMousePos);
        //        newEdge = false;
        //    }
        //}
    }
}
