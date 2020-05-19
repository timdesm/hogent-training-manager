using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InterfaceAppPresentationLayer
{
    /// <summary>
    /// Interaction logic for AddTrainingWindow.xaml
    /// </summary>
    public partial class AddTrainingWindow : Window
    {
        public AddTrainingWindow()
        {
            InitializeComponent();
        }

        private void MenuDashboardClickEvent(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Height = this.Height;
            window.Width = this.Width;
            window.Show();
            this.Close();
        }
    }
}
