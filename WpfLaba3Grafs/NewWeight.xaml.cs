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
using static WpfLaba3Grafs.NewWeight;

namespace WpfLaba3Grafs
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class NewWeight : Window
    {

        public int newWeight = 0;
        public NewWeight()
        {
            InitializeComponent();
        }
        private void newButtonTb_Click(object sender, RoutedEventArgs e)
        {
            newWeight = Convert.ToInt32(newWeightTb.Text);
            Close();
        }
    }
}

