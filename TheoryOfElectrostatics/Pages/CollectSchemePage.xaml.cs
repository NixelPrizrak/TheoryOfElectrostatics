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
                    IfTextBlock.Text = "Соедините последовательно резисторы R1, R2 и R3";
                    CollectScheme.AddElement(3, 0);
                    break;
                case 1:
                    IfTextBlock.Text = "Соедините параллельно резисторы R1, R2 и R3";
                    CollectScheme.AddElement(3, 0);
                    break;
                case 2:
                    IfTextBlock.Text = "Соедините параллельно резисторы R1, R2 и R3, а затем соедините их последовательно с R4";
                    CollectScheme.AddElement(4, 0);
                    break;
                case 3:
                    IfTextBlock.Text = "Соедините последовательно конденсаторы C1, C2 и C3";
                    CollectScheme.AddElement(0, 3);
                    break;
                case 4:
                    IfTextBlock.Text = "Соедините параллельно конденсаторы C1, C2 и C3";
                    CollectScheme.AddElement(0, 3);
                    break;
                case 5:
                    IfTextBlock.Text = "Соедините параллельно конденсаторы C1, C2 и C3, а затем соедините их последовательно с C4";
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
            Dictionary<string, ElementScheme> resistors = CollectScheme.SchemeGrid.Children.OfType<ElementScheme>().ToDictionary(resistor => resistor.Title);

            try
            {
                switch (variant)
                {
                    case 0:
                        if ((resistors["R1"].RightElement == resistors["R2"] && resistors["R2"].RightElement == resistors["R3"]) ||
                            (resistors["R1"].RightElement == resistors["R3"] && resistors["R3"].RightElement == resistors["R2"]) ||
                            (resistors["R2"].RightElement == resistors["R3"] && resistors["R3"].RightElement == resistors["R1"]) ||
                            (resistors["R2"].RightElement == resistors["R1"] && resistors["R1"].RightElement == resistors["R3"]) ||
                            (resistors["R3"].RightElement == resistors["R2"] && resistors["R2"].RightElement == resistors["R1"]) ||
                            (resistors["R3"].RightElement == resistors["R1"] && resistors["R1"].RightElement == resistors["R2"]))
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
                    case 3:
                        if ((resistors["C1"].RightElement == resistors["C2"] && resistors["C2"].RightElement == resistors["C3"]) ||
                            (resistors["C1"].RightElement == resistors["C3"] && resistors["C3"].RightElement == resistors["C2"]) ||
                            (resistors["C2"].RightElement == resistors["C3"] && resistors["C3"].RightElement == resistors["C1"]) ||
                            (resistors["C2"].RightElement == resistors["C1"] && resistors["C1"].RightElement == resistors["C3"]) ||
                            (resistors["C3"].RightElement == resistors["C2"] && resistors["C2"].RightElement == resistors["C1"]) ||
                            (resistors["C3"].RightElement == resistors["C1"] && resistors["C1"].RightElement == resistors["C2"]))
                        {
                            return true;
                        }
                        break;
                    case 4:
                        if (resistors["C1"].LeftNode != null && resistors["C1"].RightNode != null)
                        {
                            if (resistors["C1"].LeftNode == resistors["C2"].LeftNode && resistors["C3"].LeftNode == resistors["C2"].LeftNode &&
                                resistors["C1"].RightNode == resistors["C2"].RightNode && resistors["C3"].RightNode == resistors["C2"].RightNode)
                            {
                                return true;
                            }
                        }
                        break;
                    case 5:
                        if (resistors["C1"].LeftNode != null && resistors["C1"].RightNode != null)
                        {
                            if (resistors["C1"].LeftNode == resistors["C2"].LeftNode && resistors["C3"].LeftNode == resistors["C2"].LeftNode &&
                                resistors["C1"].RightNode == resistors["C2"].RightNode && resistors["C3"].RightNode == resistors["C2"].RightNode)
                            {
                                if ((resistors["C1"].LeftNode == resistors["C4"].RightNode && resistors["C1"].RightNode != resistors["C4"].LeftNode) ||
                                    (resistors["C1"].LeftNode != resistors["C4"].RightNode && resistors["C1"].RightNode == resistors["C4"].LeftNode))
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
    }
}
