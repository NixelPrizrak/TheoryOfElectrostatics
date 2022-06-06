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

namespace TheoryOfElectrostatics
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
        private int countResistor;

        public Scheme()
        {
            InitializeComponent();
        }

        public void AddResistor(int count)
        {
            countResistor = count;
            ClearLine = true;
            Resistor resistor = new Resistor();
            int countColumn = Convert.ToInt32(Math.Floor(SchemeGrid.ActualWidth / (resistor.Width + 20)));
            int countRow = Convert.ToInt32(Math.Ceiling((double)count / countColumn));
            int numResistor = 1;

            for (int i = 0; i < countRow; i++)
            {
                double height = i * (resistor.Height + 20) + 100;
                double columns = (count - i * countColumn) < countColumn ? count % countColumn : countColumn;
                for (int j = 0; j < columns; j++)
                {
                    resistor = new Resistor();
                    resistor.Margin = new Thickness(j * (resistor.Width + 20) + 20, height, 0, 0);
                    resistor.Title = $"R{numResistor}";
                    numResistor++;

                    resistor.LeftEnd.PreviewMouseLeftButtonDown += End_MouseLeftButtonDown;
                    resistor.RightEnd.PreviewMouseLeftButtonDown += End_MouseLeftButtonDown;
                    resistor.Body.MouseLeftButtonDown += Control_MouseLeftButtonDown;
                    resistor.Body.MouseLeftButtonUp += Control_MouseLeftButtonUp;
                    resistor.Body.MouseMove += Control_MouseMove;
                    SchemeGrid.Children.Add(resistor);
                }
            }

            foreach (var node in SchemeGrid.Children.OfType<Node>())
            {
                node.UnionLines = new List<UnionLine>();
                node.NodeEllipse.PreviewMouseLeftButtonDown += End_MouseLeftButtonDown;
                node.MouseLeftButtonDown += Control_MouseLeftButtonDown;
                node.MouseLeftButtonUp += Control_MouseLeftButtonUp;
                node.MouseMove += Control_MouseMove;
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
                Resistor resistor = ((sender as Rectangle).Parent as Grid).Parent as Resistor;
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
                Resistor resistor = ((sender as Rectangle).Parent as Grid).Parent as Resistor;
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
                Resistor resistor = (CurrentEllipse.Parent as Grid).Parent as Resistor;

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
                    Resistor endResistor = (ellipse.Parent as Grid).Parent as Resistor;
                    if (resistor == endResistor)
                    {
                        return;
                    }

                    if (CurrentEllipse.Name != ellipse.Name)
                    {
                        if (CurrentEllipse.Name == "LeftEnd")
                        {
                            resistor.LeftResistor = endResistor;
                            resistor.LeftLineEnd = "Start";
                            endResistor.RightResistor = resistor;
                            endResistor.RightLineEnd = "Finish";
                            resistor.LeftLine = endResistor.RightLine = CurrentLine;
                        }
                        else
                        {
                            resistor.RightResistor = endResistor;
                            resistor.RightLineEnd = "Start";
                            endResistor.LeftResistor = resistor;
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

        private void AddNodeButton_Click(object sender, RoutedEventArgs e)
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

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            List<Control> children = SchemeGrid.Children.OfType<Control>().ToList();
            foreach (Control child in children)
            {
                SchemeGrid.Children.Remove(child);
            }

            AddResistor(countResistor);
        }

        static public bool CheckScheme(Dictionary<string, Resistor> resistors, int variant)
        {
            switch (variant)
            {
                case 0:
                    if ((resistors["R1"].RightResistor == resistors["R2"] && resistors["R2"].RightResistor == resistors["R3"]) ||
                        (resistors["R1"].RightResistor == resistors["R3"] && resistors["R3"].RightResistor == resistors["R2"]) ||
                        (resistors["R2"].RightResistor == resistors["R3"] && resistors["R3"].RightResistor == resistors["R1"]) ||
                        (resistors["R2"].RightResistor == resistors["R1"] && resistors["R1"].RightResistor == resistors["R3"]) ||
                        (resistors["R3"].RightResistor == resistors["R2"] && resistors["R2"].RightResistor == resistors["R1"]) ||
                        (resistors["R3"].RightResistor == resistors["R1"] && resistors["R1"].RightResistor == resistors["R2"]))
                    {
                        return true;
                    }
                    break;
                case 1:
                    if (resistors["R1"].LeftNode != null && resistors["R1"].RightNode != null)
                    {
                        if (resistors["R1"].LeftNode == resistors["R2"].LeftNode && resistors["R3"].LeftNode == resistors["R2"].LeftNode &&
                            resistors["R1"].RightNode == resistors["R2"].RightNode && resistors["R3"].RightNode == resistors["R2"].RightNode)
                        {
                            return true;
                        }
                    }
                    break;
                case 2:
                    if (resistors["R1"].LeftNode != null && resistors["R1"].RightNode != null)
                    {
                        if (resistors["R1"].LeftNode == resistors["R2"].LeftNode && resistors["R3"].LeftNode == resistors["R2"].LeftNode &&
                            resistors["R1"].RightNode == resistors["R2"].RightNode && resistors["R3"].RightNode == resistors["R2"].RightNode)
                        {
                            if ((resistors["R1"].LeftNode == resistors["R4"].RightNode && resistors["R1"].RightNode != resistors["R4"].LeftNode) ||
                                (resistors["R1"].LeftNode != resistors["R4"].RightNode && resistors["R1"].RightNode == resistors["R4"].LeftNode))
                            {
                                return true;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            return false;
        }
    }
}
