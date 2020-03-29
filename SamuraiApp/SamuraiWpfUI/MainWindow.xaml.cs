using SamuraiApp.Data;
using SamuraiApp.Domain;
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

namespace SamuraiWpfUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ConnectedData _data = new ConnectedData();
        private Samurai _currentSamurai;
        private bool _isListChanging;
        private bool _isLoading;
        private ObjectDataProvider _samuraiViewSource;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoading = true;
            samuraiListBox.ItemsSource = _data.SamuraiListInMemory();
            var x = FindResource("samuraiViewSource");
            //(ObjectDataProvider)
            _samuraiViewSource = (ObjectDataProvider)FindResource("samuraiViewSource");
            _isLoading = false;
            samuraiListBox.SelectedIndex = 0;
        }

        private void samuraiListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoading)
            {
                _isListChanging = true;
                var x = samuraiListBox.SelectedValue;
                _currentSamurai = _data.LoadSamuraiGraph((int)samuraiListBox.SelectedValue);
                _samuraiViewSource.ObjectInstance = _currentSamurai;
                _isListChanging = false;
            }
        }
    }
}
