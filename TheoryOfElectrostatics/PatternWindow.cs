using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace TheoryOfElectrostatics
{
    public class PatternWindow : Window
    {
        public bool Maximaze { get; set; }
        public double LeftW { get; set; }
        public double TopW { get; set; }
        public double WidthW { get; set; }
        public double HeightW { get; set; }
        public Button MinButton { get; set; }
        public Button MaxButton { get; set; }
        public Button HelpButton { get; set; }
        public bool ButtonsVisible { get; set; }

        public PatternWindow()
        {
            Style = FindResource("PatternWindowStyleKey") as Style;

            ContentRendered += new EventHandler(BaseWindow_ContentRendered);
            ButtonsVisible = true;
            this.Closed += PatternWindow_Closed;
            this.StateChanged += PatternWindow_StateChanged;
        }

        private void PatternWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                Maximazed(!Maximaze);
                this.WindowState = WindowState.Normal;
            }
        }

        private void PatternWindow_Closed(object sender, EventArgs e)
        {
            if (Directory.Exists(Properties.Settings.Default.TempPath))
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Directory.Delete(Properties.Settings.Default.TempPath, true);
            }
        }

        void BaseWindow_ContentRendered(object sender, EventArgs e)
        {
            Grid header = Template.FindName("HeaderGrid", this) as Grid;
            header.MouseLeftButtonDown += Border_MouseLeftButtonDown;

            Button closeButton = Template.FindName("ExitButton", this) as Button;
            closeButton.Click += ExitButton_Click;

            MinButton = Template.FindName("MinButton", this) as Button;
            MaxButton = Template.FindName("MaxButton", this) as Button;
            HelpButton = Template.FindName("HelpButton", this) as Button;
            MinButton.Click += MinButton_Click;
            MaxButton.Click += MaxButton_Click;
            HelpButton.Click += HelpButton_Click;
            if (!ButtonsVisible)
            {
                HelpButton.Visibility = Visibility.Collapsed;
                MaxButton.Visibility = Visibility.Collapsed;
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Maximaze)
            {
                this.DragMove();
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaxButton_Click(object sender, RoutedEventArgs e)
        {
            Maximazed(!Maximaze);
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            new HelpWindow().Show();
        }

        private void Maximazed(bool max)
        {
            if (max)
            {
                Image image = MaxButton.Content as Image;
                image.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/CurtailWindow.png"));
                LeftW = this.Left;
                TopW = this.Top;
                WidthW = this.Width;
                HeightW = this.Height;
                this.ResizeMode = ResizeMode.NoResize;
                this.Left = 0;
                this.Top = 0;
                this.Width = SystemParameters.WorkArea.Width;
                this.Height = SystemParameters.WorkArea.Height;
                Maximaze = true;
            }
            else
            {
                this.Left = LeftW;
                this.Top = TopW;
                NormalScreen();
            }
        }

        private void NormalScreen()
        {
            Maximaze = false;
            Image image = MaxButton.Content as Image;
            image.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/Expand.png"));
            this.ResizeMode = ResizeMode.CanResize;
            this.Width = WidthW;
            this.Height = HeightW;
        }
    }
}
