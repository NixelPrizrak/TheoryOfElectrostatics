using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TheoryOfElectrostatics
{
    public class BaseWindow : Window
    {
        public BaseWindow()
        {
            Style = FindResource("BaseWindowStyleKey") as Style;


            ContentRendered += new EventHandler(BaseWindow_ContentRendered);
        }



        void BaseWindow_ContentRendered(object sender, EventArgs e)
        {
            ContentPresenter contentPresenter = Template.FindName("MyContentPresenter", this) as ContentPresenter;



            MessageBox.Show(String.Format("The dimensions for the content presenter are {0} by {1}",
                contentPresenter.ActualWidth,
                contentPresenter.ActualHeight));
        }
    }
}
