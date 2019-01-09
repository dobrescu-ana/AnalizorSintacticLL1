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
    /// Interaction logic for ShowGrammarPage.xaml
    /// </summary>
    public partial class ShowGrammarPage : Page
    {
        public ShowGrammarPage(Gramatica gramatica)
        {
            InitializeComponent();
            simbolstartTB.Text = gramatica.Start;
            string tmp1 = Helper.Multime(gramatica.Neterminale, " ");
            neterminaleTB.Text = tmp1;
            string tmp2 = Helper.Multime(gramatica.Terminale, " ");
            terminaleTB.Text = tmp2;
            nrReguliTB.Text = gramatica.NrReguliProductie.ToString();
            string tmp3 = Helper.MultimeReguli(gramatica.ReguliProductie, " ");
            reguliTB.Text = tmp3;
        }
    }
}
