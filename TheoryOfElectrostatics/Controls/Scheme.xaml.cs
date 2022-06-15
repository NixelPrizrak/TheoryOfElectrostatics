using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace TheoryOfElectrostatics.Controls
{
    /// <summary>
    /// Логика взаимодействия для Scheme.xaml
    /// </summary>
    public partial class Scheme : UserControl
    {
        private UnionLine CurrentLine;
        private Ellipse CurrentEllipse;
        private double DeltaX;
        private double DeltaY;
        private bool ClearLine;
        private int countResistor = 0;
        private int countCondensator = 0;

        public Scheme()
        {
            InitializeComponent();
        }

        public void AddElement(int countResistor, int countCondensator)
        {
            this.countResistor = countResistor;
            this.countCondensator = countCondensator;
            ClearLine = true;
            ElementScheme resistor = new ElementScheme();
            int countColumn = Convert.ToInt32(Math.Floor(SchemeGrid.ActualWidth / (resistor.Width + 20)));
            int countRow = Convert.ToInt32(Math.Ceiling((double)countResistor / countColumn));
            int numElement = 1;

            for (int i = 0; i < countRow; i++)
            {
                double height = i * (resistor.Height + 20)+ 20;
                double columns = (countResistor - i * countColumn) < countColumn ? countResistor % countColumn : countColumn;
                for (int j = 0; j < columns; j++)
                {
                    resistor = new ElementScheme();
                    resistor.Margin = new Thickness(j * (resistor.Width + 20) + 20, height, 0, 0);
                    resistor.Title = $"R{numElement}";
                    numElement++;

                    resistor.LeftEnd.PreviewMouseLeftButtonDown += End_MouseLeftButtonDown;
                    resistor.RightEnd.PreviewMouseLeftButtonDown += End_MouseLeftButtonDown;
                    resistor.Body.MouseLeftButtonDown += Control_MouseLeftButtonDown;
                    resistor.Body.MouseLeftButtonUp += Control_MouseLeftButtonUp;
                    resistor.Body.MouseMove += Control_MouseMove;
                    SchemeGrid.Children.Add(resistor);
                }
            }

            double nextPosition = countRow * (resistor.Height + 20);
            resistor = new ElementScheme();
            resistor.Type = 1;
            countColumn = Convert.ToInt32(Math.Floor(SchemeGrid.ActualWidth / (resistor.Width + 20)));
            countRow += Convert.ToInt32(Math.Ceiling((double)countCondensator / countColumn));
            numElement = 1;

            for (int i = 0; i < countRow; i++)
            {
                double height = i * (resistor.Height + 20) + 20 + nextPosition;
                double columns = (countCondensator - i * countColumn) < countColumn ? countCondensator % countColumn : countColumn;
                for (int j = 0; j < columns; j++)
                {
                    resistor = new ElementScheme();
                    resistor.Type = 1;
                    resistor.Margin = new Thickness(j * (resistor.Width + 20) + 20, height, 0, 0);
                    resistor.Title = $"C{numElement}";
                    numElement++;

                    resistor.LeftEnd.PreviewMouseLeftButtonDown += End_MouseLeftButtonDown;
                    resistor.RightEnd.PreviewMouseLeftButtonDown += End_MouseLeftButtonDown;
                    resistor.Body.MouseLeftButtonDown += Control_MouseLeftButtonDown;
                    resistor.Body.MouseLeftButtonUp += Control_MouseLeftButtonUp;
                    resistor.Body.MouseMove += Control_MouseMove;
                    SchemeGrid.Children.Add(resistor);
                }
            }
        }

        private void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Node node)
            {
                Point nodePosition = node.TransformToAncestor(SchemeGrid).Transform(new Point(0, 0));
                var mousePosition = Mouse.GetPosition(SchemeGrid);
                DeltaX = mousePosition.X - nodePosition.X;
                DeltaY = mousePosition.Y - nodePosition.Y;
                node.CaptureMouse();
            }
            else
            {
                ElementScheme resistor = ((sender as Rectangle).Parent as Grid).Parent as ElementScheme;
                Point resistorPosition = resistor.TransformToAncestor(SchemeGrid).Transform(new Point(0, 0));
                var mousePosition = Mouse.GetPosition(SchemeGrid);
                DeltaX = mousePosition.X - resistorPosition.X;
                DeltaY = mousePosition.Y - resistorPosition.Y;
                (sender as Rectangle).CaptureMouse();
            }
        }

        private void Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Node node)
            {
                node.ReleaseMouseCapture();
            }
            else
            {
                (sender as Rectangle).ReleaseMouseCapture();
            }
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is Node node)
            {
                if (!node.IsMouseCaptured) return;

                var mousePoint = Mouse.GetPosition(SchemeGrid);

                var offsetX = mousePoint.X - DeltaX;
                var offsetY = mousePoint.Y - DeltaY;
                if (offsetX < 10 || offsetY < 10 || offsetX > SchemeGrid.ActualWidth - node.ActualWidth - 10 || offsetY > SchemeGrid.ActualHeight - node.ActualHeight - 10)
                {
                    return;
                }

                node.Margin = new Thickness(offsetX, offsetY, 0, 0);
                Point nodePosition = node.TransformToAncestor(SchemeGrid).Transform(new Point(0, 0));
                double width = nodePosition.X + node.ActualWidth / 2;
                double height = nodePosition.Y + node.ActualHeight / 2;
                foreach (var unionLine in node.UnionLines)
                {
                    unionLine.Finish = new Point(width, height);
                }
            }
            else
            {
                ElementScheme resistor = ((sender as Rectangle).Parent as Grid).Parent as ElementScheme;
                if (!(sender as Rectangle).IsMouseCaptured) return;

                var mousePoint = Mouse.GetPosition(SchemeGrid);

                var offsetX = mousePoint.X - DeltaX;
                var offsetY = mousePoint.Y - DeltaY;

                if (offsetX < 10 || offsetY < 10 || offsetX > SchemeGrid.ActualWidth - resistor.ActualWidth - 10 || offsetY > SchemeGrid.ActualHeight - resistor.ActualHeight - 10)
                {
                    return;
                }

                resistor.Margin = new Thickness(offsetX, offsetY, 0, 0);
                Point resistorPosition = resistor.TransformToAncestor(SchemeGrid).Transform(new Point(0, 0));
                double height = resistorPosition.Y + resistor.ActualHeight / 2;
                if (resistor.LeftLine != null)
                {
                    resistor.LeftLine[resistor.LeftLineEnd] = new Point(resistorPosition.X + resistor.LeftEnd.ActualWidth / 2 + 1, height);
                }
                if (resistor.RightLine != null)
                {
                    resistor.RightLine[resistor.RightLineEnd] = new Point(resistorPosition.X + resistor.ActualWidth - resistor.LeftEnd.ActualWidth / 2 - 1, height);
                }
            }
        }

        private void End_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClearLine = false;
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
                ElementScheme resistor = (CurrentEllipse.Parent as Grid).Parent as ElementScheme;

                if (ellipse.Parent is Node node)
                {
                    if (CurrentEllipse.Name == "LeftEnd")
                    {
                        if (resistor.RightNode == node)
                        {
                            return;
                        }
                        resistor.LeftNode = node;
                        resistor.LeftLineEnd = "Start";
                        resistor.LeftLine = CurrentLine;
                    }
                    else
                    {
                        if (resistor.LeftNode == node)
                        {
                            return;
                        }
                        resistor.RightNode = node;
                        resistor.RightLineEnd = "Start";
                        resistor.RightLine = CurrentLine;
                    }

                    node.UnionLines.Add(CurrentLine);
                    Union(ellipse, CurrentLine);
                    (CurrentEllipse.Parent as Grid).Children.Remove(CurrentEllipse);
                    CurrentEllipse = null;
                }
                else
                {
                    ElementScheme endResistor = (ellipse.Parent as Grid).Parent as ElementScheme;
                    if (resistor == endResistor)
                    {
                        return;
                    }

                    if (CurrentEllipse.Name != ellipse.Name)
                    {
                        if (CurrentEllipse.Name == "LeftEnd")
                        {
                            resistor.LeftElement = endResistor;
                            resistor.LeftLineEnd = "Start";
                            endResistor.RightElement = resistor;
                            endResistor.RightLineEnd = "Finish";
                            resistor.LeftLine = endResistor.RightLine = CurrentLine;
                        }
                        else
                        {
                            resistor.RightElement = endResistor;
                            resistor.RightLineEnd = "Start";
                            endResistor.LeftElement = resistor;
                            endResistor.LeftLineEnd = "Finish";
                            resistor.RightLine = endResistor.LeftLine = CurrentLine;
                        }

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

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (CurrentLine != null)
            {
                Grid grid = sender as Grid;
                CurrentLine.X2 = e.GetPosition(grid).X;
                CurrentLine.Y2 = e.GetPosition(grid).Y;
            }
        }

        private void SchemeGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ClearLine)
            {
                if (CurrentLine != null)
                {
                    SchemeGrid.Children.Remove(CurrentLine);
                    CurrentLine = null;
                }
            }
            ClearLine = true;
        }

        public void AddNode()
        {
            Node node = new Node();
            node.Margin = new Thickness(SchemeGrid.ActualWidth / 2, SchemeGrid.ActualHeight / 2, 0, 0);

            node.UnionLines = new List<UnionLine>();
            node.NodeEllipse.PreviewMouseLeftButtonDown += End_MouseLeftButtonDown;
            node.MouseLeftButtonDown += Control_MouseLeftButtonDown;
            node.MouseLeftButtonUp += Control_MouseLeftButtonUp;
            node.MouseMove += Control_MouseMove;
            SchemeGrid.Children.Add(node);
        }

        public void Refresh()
        {
            List<Control> children = SchemeGrid.Children.OfType<Control>().ToList();
            foreach (Control child in children)
            {
                SchemeGrid.Children.Remove(child);
            }

            AddElement(countResistor, countCondensator);
        }
    }
}
