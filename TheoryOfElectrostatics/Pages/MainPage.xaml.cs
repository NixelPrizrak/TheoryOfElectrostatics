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

namespace TheoryOfElectrostatics.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            DataManager.DataFrame = DataFrame;
            DataManager.DataFrame.Navigate(new Pages.ListPage(0));
        }

        private void LectionsButton_Click(object sender, RoutedEventArgs e)
        {
            DataManager.DataFrame.Navigate(new Pages.ListPage(0));
        }

        private void PracticesButton_Click(object sender, RoutedEventArgs e)
        {
            DataManager.DataFrame.Navigate(new Pages.ListPage(1));
        }

        private void TestsButton_Click(object sender, RoutedEventArgs e)
        {
            DataManager.DataFrame.Navigate(new Pages.ListPage(2));
        }

        private void CollectSchemeButton_Click(object sender, RoutedEventArgs e)
        {
            DataManager.DataFrame.Navigate(new Pages.CollectSchemePage());
        }
    }
}
