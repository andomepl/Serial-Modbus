using ModbusTool.control;
using ModbusTool.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModbusTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();


            var mainWindowViewModel = new MainWindowViewModel();

            mainWindowViewModel.iPAddressTextBox = IpaddressMaster;

            this.DataContext = mainWindowViewModel;
        }

        private void CheckMasterSlaveAddress(object sender, KeyEventArgs e)
        {
            bool continueInut = !("D1D2D3D4D5D6D7D8D9D0".Contains(e.Key.ToString()));

            if(continueInut)
            {
                MessageBox.Show("Input a number");
            }

            e.Handled = continueInut;
           
        }
    }
}