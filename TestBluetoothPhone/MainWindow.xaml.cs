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

namespace TestBluetoothPhone
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private BluetoothPhone.PhoneManager phone;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            phone = new BluetoothPhone.PhoneManager();
            phone.InitPhone();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            phone.Tel();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            phone.getBook();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            phone.Terminate();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            phone.Hook();
        }
    }
}
