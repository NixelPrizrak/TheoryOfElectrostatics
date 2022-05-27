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

namespace TheoryOfElectrostatics
{
    /// <summary>
    /// Логика взаимодействия для TestWindow.xaml
    /// </summary>
    public partial class TestWindow : PatternWindow
    {
        public UnionLine CurrentLine { get; set; }
        public Ellipse CurrentEllipse { get; set; }
        public TestWindow()
        {
            InitializeComponent();
            foreach (var resistor in SchemeGrid.Children.OfType<Resistor>())
            {
                resistor.LeftEnd.PreviewMouseLeftButtonUp += End_MouseLeftButtonUp;
                resistor.RightEnd.PreviewMouseLeftButtonUp += End_MouseLeftButtonUp;
            }
            foreach (var node in SchemeGrid.Children.OfType<Node>())
            {
                node.NodeEllipse.PreviewMouseLeftButtonUp += End_MouseLeftButtonUp;
            }
        }

        private void End_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Ellipse ellipse = sender as Ellipse;

            if (CurrentLine == null)
            {
                if (!(ellipse.Parent is Node))
                {
                    UnionLine line = new UnionLine();

                    Point position = ellipse.TranslatePoint(new Point(0, 0), SchemeGrid);
                    line.X1 = line.X2 = position.X + ellipse.ActualWidth / 2;
                    line.Y1 = line.Y2 = position.Y + ellipse.ActualHeight / 2;
                    CurrentEllipse = ellipse;
                    CurrentLine = SchemeGrid.Children[SchemeGrid.Children.Add(line)] as UnionLine;
                    CurrentLine.IsHitTestVisible = false;
                }
            }
            else
            {
                Resistor resistor = (CurrentEllipse.Parent as Grid).Parent as Resistor;

                if (ellipse.Parent is Node node)
                {
                    if (CurrentEllipse.Name == "LeftEnd")
                    {
                        resistor.LeftNode = node;
                    }
                    else
                    {
                        resistor.RightNode = node;
                    }
                    Union(ellipse, CurrentLine);
                    (CurrentEllipse.Parent as Grid).Children.Remove(CurrentEllipse);
                    CurrentEllipse = null;
                }
                else
                {
                    Resistor endResistor = (ellipse.Parent as Grid).Parent as Resistor;
                    if (resistor == endResistor)
                    {
                        return;
                    }

                    if (CurrentEllipse.Name == "LeftEnd")
                    {
                        resistor.LeftResistor = endResistor;
                    }
                    else
                    {
                        resistor.RightResistor = endResistor;
                    }

                    if (CurrentEllipse.Name != ellipse.Name)
                    {
                        Union(ellipse, CurrentLine);
                        (CurrentEllipse.Parent as Grid).Children.Remove(CurrentEllipse);
                        (ellipse.Parent as Grid).Children.Remove(ellipse);
                        CurrentEllipse = null;
                    }
                }
            }
        }

        private void Union(Ellipse ellipse, UnionLine line)
        {
            Point position = ellipse.TranslatePoint(new Point(0, 0), SchemeGrid);
            line.X2 = position.X + ellipse.ActualWidth / 2;
            line.Y2 = position.Y + ellipse.ActualHeight / 2;
            CurrentLine = null;
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //if (CurrentLine == null)
            //{
            //    Grid grid = sender as Grid;
            //    UnionLine line = new UnionLine();
            //    line.X1 = e.GetPosition(grid).X;
            //    line.Y1 = e.GetPosition(grid).Y;
            //    line.X2 = e.GetPosition(grid).X;
            //    line.Y2 = e.GetPosition(grid).Y;
            //    CurrentLine = grid.Children[grid.Children.Add(line)] as UnionLine;
            //}
            //else
            //{
            //    CurrentLine = null;
            //}
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (CurrentLine != null)
            {
                Grid grid = sender as Grid;
                CurrentLine.X2 = e.GetPosition(grid).X;
                CurrentLine.Y2 = e.GetPosition(grid).Y;
            }
        }
    }
}
