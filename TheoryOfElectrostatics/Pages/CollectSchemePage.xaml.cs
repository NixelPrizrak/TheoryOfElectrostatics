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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TheoryOfElectrostatics.Controls;

namespace TheoryOfElectrostatics.Pages
{
    /// <summary>
    /// Логика взаимодействия для CollectSchemePage.xaml
    /// </summary>
    public partial class CollectSchemePage : Page
    {
        private int type = 0;

        public CollectSchemePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            type = new Random().Next(6);
            SelectType();
        }

        private void SelectType()
        {
            CollectScheme.SchemeGrid.Children.Clear();
            switch (type)
            {
                case 0:
                    IfTextBlock.Text = "Соберите схему, в которой резисторы R1, R2 и R3 соединены последовательно";
                    CollectScheme.AddElement(3, 0);
                    break;
                case 1:
                    IfTextBlock.Text = "Соберите схему, в которой резисторы R1, R2 и R3 соединены параллельно";
                    CollectScheme.AddElement(3, 0);
                    break;
                case 2:
                    IfTextBlock.Text = "Соберите схему, в которой резисторы R1, R2 и R3 соединены параллельно, а затем последовательно соединены с R4";
                    CollectScheme.AddElement(4, 0);
                    break;
                case 3:
                    IfTextBlock.Text = "Соберите схему, в которой конденсаторы C1, C2 и C3 соединены последовательно";
                    CollectScheme.AddElement(0, 3);
                    break;
                case 4:
                    IfTextBlock.Text = "Соберите схему, в которой конденсаторы C1, C2 и C3 соединены параллельно";
                    CollectScheme.AddElement(0, 3);
                    break;
                case 5:
                    IfTextBlock.Text = "Соберите схему, в которой конденсаторы C1, C2 и C3 соединены параллельно, а затем последовательно с C4";
                    CollectScheme.AddElement(0, 4);
                    break;
                default:
                    break;
            }
        }

        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckScheme(type))
            {
                MessageBox.Show("Схема собрана правильно", "Информация");
                return;
            }

            MessageBox.Show("Схема собрана неправильно", "Информация");
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            int newType = 0;
            do
            {
                newType = new Random().Next(6);
            } while (type == newType);
            type = newType;

            SelectType();
        }

        private void AddNodeButton_Click(object sender, RoutedEventArgs e)
        {
            CollectScheme.AddNode();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            CollectScheme.Refresh();
        }

        public bool CheckScheme(int variant)
        {
            Dictionary<string, ElementScheme> elements = CollectScheme.SchemeGrid.Children.OfType<ElementScheme>().ToDictionary(resistor => resistor.Title);

            try
            {
                switch (variant)
                {
                    case 0:
                        if ((elements["R1"].RightElement == elements["R2"] && elements["R2"].RightElement == elements["R3"]) ||
                            (elements["R3"].RightElement == elements["R2"] && elements["R2"].RightElement == elements["R1"]))
                        {
                            if (elements["R1"].LeftElement == elements["S1"] && elements["R3"].RightElement == elements["S1"] ||
                                elements["R1"].RightElement == elements["S1"] && elements["R3"].LeftElement == elements["S1"])
                            {
                                return true;
                            }
                        }
                        break;
                    case 1:
                        if (elements["R1"].LeftNode != null && elements["R1"].RightNode != null)
                        {
                            if (CheckParallel(elements["R1"], elements["R2"]) && CheckParallel(elements["R1"], elements["R3"]))
                            {
                                if (CheckParallel(elements["R1"], elements["S1"]))
                                {
                                    return true;
                                }
                            }
                        }
                        break;
                    case 2:
                        if (elements["R1"].LeftNode != null && elements["R1"].RightNode != null)
                        {
                            if (CheckParallel(elements["R1"], elements["R2"]) && CheckParallel(elements["R1"], elements["R3"]))
                            {
                                if ((elements["R1"].LeftNode == elements["R4"].RightNode && elements["R4"].LeftElement == elements["S1"]) ||
                                    (elements["R4"].RightElement == elements["S1"] && elements["R1"].RightNode == elements["R4"].LeftNode))
                                {
                                    return true;
                                }
                            }
                        }
                        break;
                    case 3:
                        if ((elements["C1"].RightElement == elements["C2"] && elements["C2"].RightElement == elements["C3"]) ||
                            (elements["C3"].RightElement == elements["C2"] && elements["C2"].RightElement == elements["C1"]))
                        {
                            if (elements["C1"].LeftElement == elements["S1"] && elements["C3"].RightElement == elements["S1"] ||
                                elements["C1"].RightElement == elements["S1"] && elements["C3"].LeftElement == elements["S1"])
                            {
                                return true;
                            }
                        }
                        break;
                    case 4:
                        if (elements["C1"].LeftNode != null && elements["C1"].RightNode != null)
                        {
                            if (CheckParallel(elements["C1"], elements["C2"]) && CheckParallel(elements["C1"], elements["C3"]))
                            {
                                return true;
                            }
                        }
                        break;
                    case 5:
                        if (elements["C1"].LeftNode != null && elements["C1"].RightNode != null)
                        {
                            if (CheckParallel(elements["C1"], elements["C2"]) && CheckParallel(elements["C1"], elements["C3"]))
                            {
                                if ((elements["C1"].LeftNode == elements["C4"].RightNode && elements["C4"].LeftElement == elements["S1"]) ||
                                    (elements["C4"].RightElement == elements["S1"] && elements["C1"].RightNode == elements["C4"].LeftNode))
                                {
                                    return true;
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return false;
        }

        public bool CheckParallel(ElementScheme firstElement, ElementScheme secondElement)
        {
            return (firstElement.LeftNode == secondElement.LeftNode && firstElement.RightNode == secondElement.RightNode) ||
                   (firstElement.LeftNode == secondElement.RightNode && firstElement.RightNode == secondElement.LeftNode);
        }
    }
}
