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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TheoryOfElectrostatics.Classes;

namespace TheoryOfElectrostatics
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : PatternWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataManager.CheckTempFolder();
            DataManager.Edit = false;

            DataManager.MainFrame = MainFrame;
            DataManager.MainFrame.Navigate(new Pages.MainPage());
        }
    }
}
