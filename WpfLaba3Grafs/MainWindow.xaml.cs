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
        private Point MousePos;
        private bool newVertex = false;
        private bool newEdge = false;
        public MainWindow()
        {
            function = new FunctionsMainWindow(this);
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
            MousePos = e.GetPosition(DrawingCanvas);
            if (newVertex)
            {
                function.CreateVertex(MousePos);
                newVertex = false;
            }
        }
      
        private void MouseLeftButtonUp_DrawingGraph(object sender, MouseButtonEventArgs e)
        {
             if (newEdge)
             {
                Point secondMousePos = e.GetPosition(DrawingCanvas);
                function.CreateEdge(MousePos, secondMousePos);
                newEdge = false;
             }
        }
    }
}
