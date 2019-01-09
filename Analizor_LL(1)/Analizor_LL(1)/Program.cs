using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;



namespace Analizor_LL_1_
{
    class Program
    {
        static void Main(string[] args)
        {
          
            Gramatica x = new Gramatica();
            x.GetData(@"D:\input.txt");
           // x.PrintNrReguli();
            x.PrintReguliProductie();
            Console.WriteLine("\n\n\n");
           x.EliminareReguliIdentice();
            

            //x.PrintNeterminale();
           // x.PrintTerminale();
           // x.PrintNrReguli();
           // Console.WriteLine();
           //x.PrintReguliProductie();
           // Console.WriteLine();
           // Console.WriteLine();

            x.EliminareRecursivitateStanga();
           
            //Console.WriteLine("\n\n\n");
            x.PrintReguliProductie();

            x.PrintNrReguli();
            x.PrintNeterminale();
            Console.WriteLine("\n\n\n");
            x.MultimeSimboliDirectori();
            x.PrintSimboliDirectori();
            x.MultimiDisjuncte();
            x.TabelaDeAnalizaSintactica();
            x.PopulareTabela();


            CodeGenerator codeGenerator = new CodeGenerator();
            codeGenerator.AddEntryPoint(x);
            codeGenerator.AddFields();
            codeGenerator.AddMethods(x);
            codeGenerator.GenerateCSharpCode("output.cs");
            //x.PrintNrReguli();
        }

        
       
    }
}
