using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analizor_LL_1_
{
    public class Helper
    {
        public static int Compare(string[] a, string[] b)
        {
            int checkNb = 0; // numarul primelor elemente comune alaturate
            int it = 0;
            int min = Math.Min(a.Count(), b.Count());

            while (it < min)
            {
                if (a[it].Equals(b[it]))
                {
                    it++;
                    checkNb++;
                }
                else
                {
                    break;
                }
            }

            return checkNb;
        }

        public static string[] Concatenate(string[] a, string b)
        {
            List<string> aux = new List<string>(a);
            aux.Add(b);
            string[] arr = aux.Select(i => i.ToString()).ToArray();

            return arr;
        }
        
        public static string Multime(List<string> myList, string separator)
        {
            string str = "{ ";
            int i;
            for(i=0;i<myList.Count-1;i++)
            {
                str = str + myList[i] + separator; 
            }
            str += myList[i];
            str = str + " }";
            return str;
        }

        public static string MultimeReguli(List<ReguliDeProductie> myList, string separator)
        {
            string str = "{ ";
            int i;
            for (i = 0; i < myList.Count - 1; i++)
            {
                str = str + myList[i].ToString() + separator + "\n";
            }
            str += myList[i];
            str = str + " }";
            return str;
        }
        
    }
}
