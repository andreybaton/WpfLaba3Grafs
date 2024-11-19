﻿using System;
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
    /// Логика взаимодействия для InputWindow.xaml
    /// </summary>
    public partial class InputWindow : Window
    {
        public bool isOpen = true;
        public InputWindow()
        {
            InitializeComponent();
        }
        public int GetStartV()
        {
            return Convert.ToInt32(tb_start.Text);
        }
        public int GetEndV()
        {
            return Convert.ToInt32(tb_end.Text);
        }
        public void BtnClick_Close(object sender, RoutedEventArgs e)
        {
            isOpen = false;
            Close();
        }
    }
}
