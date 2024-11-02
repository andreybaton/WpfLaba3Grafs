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

        public MainWindow()
        {
            InitializeComponent();
            
        }
        public void BtnClick_CreateVertex(object sender, RoutedEventArgs e)
        {
            function = new FunctionsMainWindow();
            function.newVertex = true;
        }
        public void BtnClick_CreateEdge(object sender, RoutedEventArgs e)
        {
            function = new FunctionsMainWindow();
            function.newEdge = true;
        }
        public void MouseLeftBtnDown_DrawingGraph(object sender, MouseButtonEventArgs e)
        {
            function = new FunctionsMainWindow();
            function.OnMouseLeftBtnDown_DrawingGraph(sender, e);
        }
    }
}
