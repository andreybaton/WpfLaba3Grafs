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
    public class FunctionsLogic 
    {
        public bool newVertex = false;
        public bool newEdge = false;
        private MainWindow mainWindow;
        public FunctionsLogic(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            
        }
        public void CreateVertex(Point position)
        {
            string selectedColorName = mainWindow.GetSelectedColor();
            Brush strokeBrush = mainWindow.ConvertStringToBrush(selectedColorName);
            Ellipse vertex = new Ellipse()
            {
                Width = 10,
                Height = 10,
                Stroke = strokeBrush,
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
            string selectedColorName = mainWindow.GetSelectedColor();
            Brush strokeBrush = mainWindow.ConvertStringToBrush(selectedColorName);
            Line edge = new Line()
            {
                X1 = pos1.X,
                Y1 = pos1.Y,
                X2 = pos2.X,
                Y2 = pos2.Y,
                Stroke = strokeBrush,
                StrokeThickness = 2
            };
            mainWindow.DrawingCanvas.Children.Add(edge);
            if (mainWindow.typeEdge == true)
                DrawArrow(pos1, pos2);
            newEdge = false;
        }
        public void ReDrawGraph(Graph graph)
        {
            try
            {
                //MnWw.DrawingCanvas.Children.Clear();
                foreach (var edge in graph.Edges)
                {
                    Point p1 = edge.from.Position;
                    Point p2 = edge.to.Position;
                    CreateEdge(p1, p2);
                }
                foreach (var node in graph.Nodes)
                {
                    Point p = node.Position;
                    CreateVertex(p);
                }
            }
            catch { MessageBox.Show("error redraw graph"); }
        }
        public Polygon DrawArrow(Point pos1, Point pos2)
        {
            double arrowLength = 10; // Длина стрелки
            double angle = Math.Atan2(pos2.Y - pos1.Y, pos2.X - pos1.X); // Угол в радианах

            // Создаем стрелку
            Polygon arrowHead = new Polygon
            {
                Fill = Brushes.Black,
                Points = new PointCollection
                {
                    new Point(pos2.X, pos2.Y), // Конечная точка линии
                    new Point(pos2.X - arrowLength * Math.Cos(angle - Math.PI / 6),
                    pos2.Y - arrowLength * Math.Sin(angle - Math.PI / 6)), // Левый угол стрелки

                    new Point(pos2.X - arrowLength * Math.Cos(angle + Math.PI / 6),
                    pos2.Y - arrowLength * Math.Sin(angle + Math.PI / 6))  // Правый угол стрелки
                }
            };
            return arrowHead;
        }
    }
}
