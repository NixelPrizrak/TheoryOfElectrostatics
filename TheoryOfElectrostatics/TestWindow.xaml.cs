using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TheoryOfElectrostatics
{
    /// <summary>
    /// Логика взаимодействия для TestWindow.xaml
    /// </summary>
    public partial class TestWindow : PatternWindow
    {
        private double startPosition = 0;
        private double startOffset = 0;

        public TestWindow()
        {
            InitializeComponent();
        }

        private void PatternWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //for (int i = 0; i < 40; i++)
            //{
            //    QuestionsListView.Items.Add(i);
            //}
            TestScheme.AddElement(4, 2);
        }

        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            //Dictionary<string, Resistor> resistors = TestScheme.SchemeGrid.Children.OfType<Resistor>().ToDictionary(resistor => resistor.Title);
            //if (Scheme.CheckScheme(resistors, 0))
            //{
            //    MessageBox.Show("Правильно");
            //    return;
            //}

            //MessageBox.Show("Неправильно");
        }

        private void SelectionQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("");
        }

        private void QuestionsListView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListView lv = sender as ListView;
            startPosition = e.GetPosition(this).X;
            ScrollViewer sv = FindVisualChild<ScrollViewer>(lv);
            startOffset = sv.HorizontalOffset;
            (sender as ListView).CaptureMouse();
        }

        private void QuestionsListView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (sender as ListView).ReleaseMouseCapture();
        }

        private void QuestionsListView_MouseMove(object sender, MouseEventArgs e)
        {
            ListView lv = sender as ListView;
            if (!lv.IsMouseCaptured)
            {
                return;
            }
            ScrollViewer sv = FindVisualChild<ScrollViewer>(lv);
            double mousePosition = e.GetPosition(this).X;
            sv.ScrollToHorizontalOffset(startOffset - mousePosition + startPosition);
        }

        public static childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            // Search immediate children first (breadth-first)
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is childItem)
                    return (childItem)child;

                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);

                    if (childOfChild != null)
                        return childOfChild;
                }
            }

            return null;
        }

        private void QuestionsListView_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ListView lv = sender as ListView;
            ScrollViewer sv = FindVisualChild<ScrollViewer>(lv);
            sv.ScrollToHorizontalOffset(sv.HorizontalOffset + e.Delta);
        }


        //public object CheckParallel(Dictionary<string, Resistor> resistors, string ifUnion)
        //{
        //    Regex regex = new Regex(@"\(.+\)");
        //    var matches = regex.Matches(ifUnion);
        //    foreach (Match match in matches)
        //    {
        //        Dictionary
        //    }

        //    return new Resistor();
        //}
    }
}
