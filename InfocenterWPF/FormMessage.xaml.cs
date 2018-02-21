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

namespace InfocenterWPF
{
    /// <summary>
    /// Логика взаимодействия для FormMessage.xaml
    /// </summary>
    public partial class FormMessage : Window
    {
        MainWindow formMain;
        public FormMessage(MainWindow mainWindow)
        {
            InitializeComponent();
            this.formMain = mainWindow;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
