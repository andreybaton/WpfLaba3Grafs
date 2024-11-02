using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes; 
using System.Drawing;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections;

namespace WpfLaba3Grafs
{
    public class FunctionsMainWindow 
    {
        public bool newVertex = false;
        public bool newEdge = false;
        private MainWindow mainWindow;
        public FunctionsMainWindow(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }
        public void CreateVertex(Point position)
        {
            Ellipse vertex = new Ellipse()
            {
                Width = 10,
                Height = 10,
                Stroke = mainWindow.GetSelectedColor(),
                StrokeThickness = 2
            };
            double posX = position.X - vertex.Width / 2;
            double posY = position.Y - vertex.Height / 2;

            Canvas.SetTop(vertex, posY);
            Canvas.SetLeft(vertex, posX);
            mainWindow.DrawingCanvas.Children.Add(vertex);
        }

        public void CreateEdge(Point pos1, Point pos2)
        {
            Line edge = new Line()
            {
                X1 = pos1.X,
                Y1 = pos1.Y,
                X2 = pos2.X,
                Y2 = pos2.Y,
                Stroke = mainWindow.GetSelectedColor(),
                StrokeThickness = 2
            };
            mainWindow.DrawingCanvas.Children.Add(edge);
            newEdge = false;
        }

        public bool ArePointsClose(Point point1, Point point2, double radius)
        {
            double radiusSquared = radius * radius;
            double distanceSquared = (point1.X - point2.X) * (point1.X - point2.X) +
                                     (point1.Y - point2.Y) * (point1.Y - point2.Y);
            return distanceSquared <= radiusSquared;
        }
    }
}
