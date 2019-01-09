using MahApps.Metro.Controls;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace Interface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            string path = pathTextBox.Text;
            if (!File.Exists(path))
                MessageBox.Show("Nu exista fisierul!", "Eroare!", MessageBoxButton.OK, MessageBoxImage.Hand);
            this.Content = new MenuPage(path);
        }

        private void getFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != null)
                pathTextBox.Text = openFileDialog.FileName;
        }
    }
}
