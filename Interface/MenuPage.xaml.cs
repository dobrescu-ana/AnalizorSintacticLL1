using Analizor_LL_1_;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

namespace Interface
{
    /// <summary>
    /// Interaction logic for MenuPage.xaml
    /// </summary>
    public partial class MenuPage : Page
    {
        private Gramatica _gramatica;
        bool CheckLL1Conditions = false;
        public MenuPage(string fileName)
        {
            InitializeComponent();
            _gramatica = new Gramatica();
            _gramatica.GetData(fileName);
            mainContentFrame.Content = new GreetingPage();
        }

        private void GrammarButton_Click(object sender, RoutedEventArgs e)
        {
            mainContentFrame.Content = new ShowGrammarPage(_gramatica);
        }

        private void CheckGrammar_Click(object sender, RoutedEventArgs e)
        {
            _gramatica.EliminareReguliIdentice();
            _gramatica.EliminareRecursivitateStanga();
        }

        private void CheckLL1_Click(object sender, RoutedEventArgs e)
        {
            _gramatica.MultimeSimboliDirectori();
            if (_gramatica.MultimiDisjuncte())
            {
                MessageBox.Show("Multimile de simboli directori sunt disjuncte. Gramatica data este de tip LL1!");
                CheckLL1Conditions = true;
            }
            else
            {
                MessageBox.Show("Multimile de simboli directori nu sunt disjuncte!", "Eroare", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
            
        }

        private void SeeTAB_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckLL1Conditions)
            {
                MessageBox.Show("Nu se poate construi tabela de analiza sintactica!", "Eroare", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
            else
            {
                mainContentFrame.Content = new ShowTABPage(_gramatica);
            }
            
        }

        private void CheckExample_Click(object sender, RoutedEventArgs e)
        {
            
            Analizor_LL_1_.CodeGenerator codeGenerator = new Analizor_LL_1_.CodeGenerator();
            codeGenerator.AddEntryPoint(_gramatica);
            codeGenerator.AddFields();
            codeGenerator.AddMethods(_gramatica);
            codeGenerator.GenerateCSharpCode("output.cs");

            CompilerParameters cp = new CompilerParameters
            {
                GenerateExecutable = true,
                IncludeDebugInformation = true,
                GenerateInMemory = false,
                WarningLevel = 4,
                TreatWarningsAsErrors = false,
                CompilerOptions = "/optimize",
                OutputAssembly = "output.exe",
            };
            cp.ReferencedAssemblies.Add("System.dll");
            CodeDomProvider provider = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider();
            CompilerResults cr = provider.CompileAssemblyFromFile(cp, "output.cs");
            Process.Start("output.exe");
        }
    }
}
