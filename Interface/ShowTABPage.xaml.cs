using Analizor_LL_1_;
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

namespace Interface
{
    /// <summary>
    /// Interaction logic for ShowTABPage.xaml
    /// </summary>
    public partial class ShowTABPage : Page
    {
        Gramatica gram;

        public ShowTABPage(Gramatica gramatica)
        {
            gram = gramatica;
            InitializeComponent();
            gram.TabelaDeAnalizaSintactica();
            gram.PopulareTabela();
            TAB.ItemsSource = gram.Tab.DefaultView;
            
        }

        private void TAB_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = gram.Tab.Columns[e.PropertyName].Caption;
        }
    }
}
