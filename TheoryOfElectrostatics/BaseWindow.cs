using System;
using System.Collections.Generic;
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
    public class BaseWindow : Window
    {
        public bool Maximaze { get; set; }
        public double LeftW { get; set; }
        public double TopW { get; set; }
        public double WidthW { get; set; }
        public double HeightW { get; set; }
        public BaseWindow()
        {
            Style = FindResource("BaseWindowStyleKey") as Style;

            ContentRendered += new EventHandler(BaseWindow_ContentRendered);
        }

        void BaseWindow_ContentRendered(object sender, EventArgs e)
        {
            Grid header = Template.FindName("HeaderGrid", this) as Grid;
            header.MouseLeftButtonDown += Border_MouseLeftButtonDown;
            Button closeButton = Template.FindName("ExitButton", this) as Button;
            closeButton.Click += ExitButton_Click;
            Button minButton = Template.FindName("MinButton", this) as Button;
            minButton.Click += MinButton_Click;
            Button maxButton = Template.FindName("MaxButton", this) as Button;
            maxButton.Click += MaxButton_Click;

            this.LocationChanged += BaseWindow_LocationChanged;
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

        private void BaseWindow_LocationChanged(object sender, EventArgs e)
        {
        }

        private void Maximazed(bool max)
        {
            if (max)
            {
                Image image = (Template.FindName("MaxButton", this) as Button).Content as Image;
                image.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/CurtailWindow2.png"));
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
            Image image = (Template.FindName("MaxButton", this) as Button).Content as Image;
            image.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/Expand.png"));
            this.ResizeMode = ResizeMode.CanResize;
            this.Width = WidthW;
            this.Height = HeightW;
        }
    }
}
