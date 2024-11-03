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
using System.Windows.Shapes;

namespace WpfLaba3Grafs
{
    /// <summary>
    /// Логика взаимодействия для WindowAddEdge.xaml
    /// </summary>
    public partial class WindowAddEdge : Window
    {
        MainWindow mw;
        public bool typeEdge;
        public bool weightExist;
        public WindowAddEdge(MainWindow mw)
        {
           
            InitializeComponent();
            this.mw = mw;
            typeEdge = false;
            weightExist = false;
        }
        private void BtnClick_AddOrientedEdge(object sender, RoutedEventArgs e)
        {
            typeEdge = true;
            this.DialogResult = true;
            this.Close();
        }
        private void BtnClick_AddUndirectedEdge(object sender, RoutedEventArgs e)
        {
            if (weightEdge.Text == "Ненагруженный")
            { }
            else {
                weightExist = true;
            }
            this.DialogResult = true;
            this.Close();
        }

    }
}
