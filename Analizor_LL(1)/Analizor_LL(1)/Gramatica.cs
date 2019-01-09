using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Analizor_LL_1_
{
    public class Gramatica
    {
        string                  _start;
        List<string>            _neterminale;
        List<string>            _terminale; 
        int pNr;
        List<ReguliDeProductie> _reguliProductie; 
        List<string>[]          SimboliDirectori;
        DataTable               _tabelaAnalizaSintactica;


        #region GET METHODS
        
        public string Start => _start;

        public List<string> Neterminale => _neterminale;

        public List<string> Terminale => _terminale;

        public int NrReguliProductie => pNr;

        public List<ReguliDeProductie> ReguliProductie => _reguliProductie;

        public DataTable Tab => _tabelaAnalizaSintactica;

        public void GetData(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {

                // Preluare Simbol de _start
                _start = sr.ReadLine();

                // Preluare _neterminale
                _neterminale = new List<string>();
                string[] netString = sr.ReadLine().Split(' ');
                InitNeterminate(netString);

                // Preluare terminale
                _terminale = new List<string>();
                string[] termString = sr.ReadLine().Split(' ');
                InitTerminale(termString);

                // Preluarea numarului regulilor de productie
                pNr = int.Parse(sr.ReadLine());

                // Preluarea regulilor de productie
                _reguliProductie = new List<ReguliDeProductie>();
                int count = 0;
                while (count < pNr)
                {
                    string[] PRule = sr.ReadLine().Split(' ');
                    InitRegProd(PRule);
                    count++;
                }
            }

        }

        #endregion

        #region INITIALIZATION

        private void InitNeterminate(string[] neterminale)
        {
            int i = 0;
            foreach (var net in neterminale)
            {
                _neterminale.Add(neterminale[i]);
                i++;  
            }
        }

        private void InitTerminale(string[] terminale)
        {
            int j = 0;
            foreach (var term in terminale)
            {
                if (CheckTerminal(term) == false)
                {
                    Console.WriteLine("Terminal invalid");
                    break;
                }
                _terminale.Add(terminale[j]);
                j++;
            }
        }

        private void InitRegProd(string[] reguliProd)
        {
            ReguliDeProductie x = new ReguliDeProductie();
            x.SetLeft(reguliProd[0]);
            x.InitializeRightPartOfExistingRules(reguliProd);
            _reguliProductie.Add(x);
           
            Console.WriteLine();
        }

        #endregion

        #region CHECK METHODS


        private static bool CheckTerminal(string terminal)
        {
            foreach (char c in terminal)
            {
                if (c >= 'A' && c <= 'Z')
                    return false;
            }
            return true;
        }

        private void CheckElements(List<ReguliDeProductie> rList, int index, List<ReguliDeProductie> cReguliProductie, bool[] checkArr)
        {
            // Contor utiliat in denumirea noilor neterminale
            int contor = 0; 

            // Fiecare regula de productie specifica aceluiasi neterminal 
            for (int i = 0; i < rList.Count; i++)
            {
                for (int j = i + 1; j < rList.Count; j++)
                {
                    // Se compara inceputul partii drepte a regulii de productie cu cel al urmatoarelor neterminale 
                    int NumberOfCommonElements = Helper.Compare(rList[i].GetRight(), rList[j].GetRight());

                    // Daca se gaseste cel putin un element comun 
                    if (NumberOfCommonElements != 0)
                    {
                        contor++;

                        // Doua reguli pentru A1
                        ReguliDeProductie newElement1 = new ReguliDeProductie();
                        ReguliDeProductie newElement2 = new ReguliDeProductie();
                        
                       
                        // Se seteaza partea stanga a celor doua reguli corespunzatoare neterminalului A1
                        newElement1.SetLeft(rList[i].GetLeft() + contor.ToString());
                        newElement2.SetLeft(rList[i].GetLeft() + contor.ToString());

                        // Se adauga A1 in lista de neterminale
                        _neterminale.Add(newElement1.GetLeft());

                        // Se seteaza partea dreapta a primei reguli corespunzatoare lui A1 
                        string[] right_1 = rList[i].GetRight().Reverse().Take(rList[i].GetRightPartNumberOfElements() - NumberOfCommonElements).Reverse().ToArray();
                        newElement1.SetRight(right_1);

                        // Se seteaza partea dreapta pentru a doua regula corespunzatoare lui A1 
                        string[] right_2 = rList[j].GetRight().Reverse().Take(rList[j].GetRightPartNumberOfElements() - NumberOfCommonElements).Reverse().ToArray();
                        newElement2.SetRight(right_2);

                        //// Adauugam cele doua reugli de productie la lista
                        cReguliProductie.Add(newElement1);
                        cReguliProductie.Add(newElement2);
                        
                        // Se ia partea comuna a celor coua reguli (a)
                        string[] right = rList[i].GetRight().Take(NumberOfCommonElements).ToArray();

                        // Se concateneaza cu neterminalul introdus (A1)
                        ReguliDeProductie newElement0 = new ReguliDeProductie();
                        newElement0.SetLeft(rList[i].GetLeft());
                        newElement0.SetRight(Helper.Concatenate(right, newElement1.GetLeft()));

                        // Se adauga noua regula de productie in lista 
                        cReguliProductie.Add(newElement0);

                        // se bifeaza regula de productie folosita din sirul original
                        checkArr[index+i] = true;

                        // Se elimina A -> ac
                        if (checkArr[index + j] == false)
                        {
                            //_reguliProductie.RemoveAt(index + j);
                            checkArr[index + j] = true;
                        }

                    }
                }
                if(checkArr[index+i] == false)
                {
                    cReguliProductie.Add(rList[i]);
                    checkArr[index + i] = true;
                }
            }
        }

        private void CheckLeftRecv(List<ReguliDeProductie> rList, int index, List<ReguliDeProductie> cReguliProductie, bool[] checkArr)
        {
            for (int i = 0; i < rList.Count; i++)
            {
                int contor = rList[i].GetRight().Length;
                string first = rList[i].GetRight().FirstOrDefault();
                if (rList[i].GetLeft() == first)
                {
                    if (rList.Count < 2)
                    {
                        Console.WriteLine("Imposibil de eliminar recursivitatea stanga!");
                        break;
                    }

                    int l_contor = rList[i].GetRight().Length;

                    // Introducem 2 reguli de productie noi
                    ReguliDeProductie newRule1 = new ReguliDeProductie();
                    ReguliDeProductie newRule2 = new ReguliDeProductie();

                    // Ambele reguli de productie vor fi pentru acelasi neterminal
                    newRule1.SetLeft(rList[i].GetLeft().ToString() + 'x');
                    newRule2.SetLeft(rList[i].GetLeft().ToString() + 'x');


                    // Se adauga noul neterminal in lista
                    _neterminale.Add(newRule1.GetLeft());

                    // Initializare prima regula de productie introdusa
                    string[] right_1 = rList[i].GetRight().Reverse().Take(l_contor - 1).Reverse().ToArray();
                    newRule1.SetRight(Helper.Concatenate(right_1, newRule1.GetLeft()));


                    // Initializare a doua regula de productie introdusa
                    string[] right_2 = new string[] { };
                    newRule2.SetRight(right_2);



                    // Introducem noile reguli de productie in lista
                    cReguliProductie.Add(newRule1);
                    cReguliProductie.Add(newRule2);

                    ReguliDeProductie newRule0 = new ReguliDeProductie();
                    newRule0.SetLeft(rList[i].GetLeft());

                    // Aplicam prima parte a regulii de eliminare a recursivitatii
                    if (i == rList.Count - 1)
                    {
                        string[] right = rList[i - 1].GetRight().Take(1).Select(ind => ind.ToString()).ToArray();
                        string[] str = Helper.Concatenate(right, newRule1.GetLeft());
                        newRule0.SetRight(str);
                        
                        //_reguliProductie.RemoveAt(index + i - 1);
                        if (checkArr[index + i - 1] == false)
                        {
                            checkArr[index + i - 1] = true;
                        }
                    }
                    else
                    if(i<rList.Count-1)
                    {
                        string[] right = rList[i + 1].GetRight().Take(1).Select(ind => ind.ToString()).ToArray();
                        string[] str = Helper.Concatenate(right, newRule1.GetLeft());
                        newRule0.SetRight(str);

                        if (checkArr[index + i + 1] == false)
                        {
                            checkArr[index + i + 1] = true;
                        }
                        //_reguliProductie.RemoveAt(index + i + 1);

                    }
                    cReguliProductie.Add(newRule0);
                    checkArr[index + i] = true;

                }

            }

            for (int i = 0; i < rList.Count; i++)
            {
                if (checkArr[index + i] == false)
                {
                    cReguliProductie.Add(rList[i]);
                    checkArr[index + i] = true;
                }
            }

        }

        private bool Check(int neterminal1, int neterminal2)
        {
            foreach (var simbol1 in SimboliDirectori[neterminal1])
            {
                foreach (var simbol2 in SimboliDirectori[neterminal2])
                {
                    if (simbol1 == simbol2)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion

        #region CONDITII ANALIZA SINTACTICA DESCENDENTA
     
        public void EliminareReguliIdentice()
        {
            // Indice pentru lista regulilor de productie
            int it = 0;

            // Facem o copie la lista regulilor de productie
            List<ReguliDeProductie> cReguliProductie = new List<ReguliDeProductie>();

            bool[] arr = new bool[_reguliProductie.Count];
            Array.Clear(arr, 0, arr.Length);

            // Indicele nu trebuie sa depaseasca numarul regulilor de productie
            while (it < _reguliProductie.Count)
            {
                // Se determina toate regulile de produtie corespunzatoare unui neterminal
                ReguliDeProductie regula = _reguliProductie[it]; 
                IEnumerable<ReguliDeProductie> results = _reguliProductie.Where(s => s.GetLeft() == regula.GetLeft());

                // Se salveaza rezultatul intr-o lista
                List<ReguliDeProductie> rList = results.ToList();

                // Numarul regulilor de productie corespunzatoare aceluiasi neterminal
                int counter = rList.Count;
                
                // Functie care elimina regulile de productie care incep la fel
                CheckElements(rList, it, cReguliProductie, arr);

                // Se trece la contorizarea urmatorului set de reguli de productie pentru urmatorul neterminal
                it = it + counter;
                
            }

            _reguliProductie.Clear();
            _reguliProductie = new List<ReguliDeProductie>(cReguliProductie);
            pNr = _reguliProductie.Count;

        }

        public void EliminareRecursivitateStanga()
        {
            // Indice pentru lista regulilor de productie
            int it = 0;
            // Facem o copie la lista regulilor de productie
            List<ReguliDeProductie> cReguliProductie = new List<ReguliDeProductie>();

            bool[] arr = new bool[_reguliProductie.Count];
            Array.Clear(arr, 0, arr.Length);

            // Indicele nu trebuie sa depaseasca numarul regulilor de productie
            while (it < _reguliProductie.Count)
            {
                // Se determina toate regulile de produtie corespunzatoare unui neterminal
                ReguliDeProductie regula = _reguliProductie[it];
                IEnumerable<ReguliDeProductie> results = _reguliProductie.Where(s => s.GetLeft() == regula.GetLeft());

                // Se salveaza rezultatul intr-o lista
                List<ReguliDeProductie> rList = results.ToList();

                // Numarul regulilor de productie corespunzatoare aceluiasi neterminal
                int counter = rList.Count;
                CheckLeftRecv(rList, it, cReguliProductie, arr);
                it = it + counter;
            }
            _reguliProductie.Clear();
            _reguliProductie = new List<ReguliDeProductie>(cReguliProductie);
            pNr = _reguliProductie.Count;

        }

        #endregion

        #region PRINT METHODS

        public void PrintReguliProductie()
        {
            foreach (var regula in _reguliProductie)
            {
                //regula.Print();
            }
        }

        public void PrintNeterminale()
        {
            Console.Write("Neterminalele sunt: ");
            foreach(var neterminal in _neterminale)
            {
                Console.Write(neterminal + " ");
            }
            Console.WriteLine();
        }

        public void PrintTerminale()
        {
            Console.Write("Terminalele sunt: ");
            foreach(var terminal in _terminale)
            {
                Console.Write(terminal + " ");
            }
            Console.WriteLine();
        }

        public void PrintNrReguli()
        {
            Console.WriteLine("Numarul regulilor de productie: " + pNr);
        }

        public void PrintSimboliDirectori()
        {
            Console.WriteLine("Simboli Directori: \n");
            for(int i = 0; i < _reguliProductie.Count; i++)
            {
                Console.Write('R'+(i+1).ToString() + " pentru neterminalul " + _reguliProductie[i].GetLeft() +" "+ " -> ");
                foreach(var j in SimboliDirectori[i])
                {
                    Console.Write(j + " ");
                }
                Console.WriteLine();
            }
        }

        private static void PrintTable(DataTable dt)
        {
            foreach (var column in dt.Columns)
            {
                Console.Write(column + "\t");
            }
            Console.WriteLine();
            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn column in dt.Columns)
                    Console.Write("{0}\t", row[column]);

                Console.WriteLine();
            }

        }

        #endregion

        #region SIMBOLI DIRECTORI

        public void MultimeSimboliDirectori()
        {
            SimboliDirectori = new List<string>[_reguliProductie.Count];

            List<string> temp = new List<string>();

            for (int i = 0; i < _reguliProductie.Count; i++)
            {
              //Console.Write(_reguliProductie[i].GetLeft() + " cu simbolii directori ");
                if (_reguliProductie[i].GetRight().Count() > 1)
                {
                    string firstEl = _reguliProductie[i].GetRight().FirstOrDefault();
                    SimboliDirectori[i] = FIRST(firstEl);
                }
                else
                {
                    SimboliDirectori[i] = FOLLOW(_reguliProductie[i].GetLeft(),temp);
                    temp.Clear();
                }

                //foreach(var j in SimboliDirectori[i])
                //{
                //    Console.Write(j + " ");
                //}
                //Console.WriteLine();
            }

            
        }

        private List<string> FIRST(string simbol)
        {
            List<string> simboluri = new List<string>();
            if (_terminale.Contains(simbol))
            {
                simboluri.Add(simbol);
            }
            else
            {
                for (int i = 0; i < _reguliProductie.Count; i++)
                    if (_reguliProductie[i].GetLeft() == simbol)
                    {
                        string firstEl = _reguliProductie[i].GetRight().FirstOrDefault();
                       //simboluri.AddRange(FIRST(firstEl));
                        if (firstEl != null)
                            simboluri.AddRange(FIRST(firstEl));
                            //FIRST(firstEl);

                    }
            }
            return simboluri;
        }

        private List<string> FOLLOW(string neterminal,List<string> checkList)
        {
            List<string> simboluri = new List<string>();

            for (int i = 0; i < _reguliProductie.Count; i++)
            {
                for (int j = 0; j < _reguliProductie[i].GetRight().Length; j++)
                    {

                        if ((_reguliProductie[i].GetRight())[j] == neterminal)
                        {
                            if (j != _reguliProductie[i].GetRight().Length - 1)
                            {
                                simboluri.AddRange(FIRST(_reguliProductie[i].GetRight()[j + 1]));
                            }
                            else
                            if(!checkList.Contains(_reguliProductie[i].GetLeft()))
                            {
                                checkList.Add(_reguliProductie[i].GetLeft());
                                simboluri.AddRange(FOLLOW(_reguliProductie[i].GetLeft(),checkList));
                            }
                        }
                    
                }
            }
            simboluri.Add("$");
            return simboluri.Distinct().ToList();
        }

        #endregion

        #region CONDITII GRAMATICA LL(1)

        public bool MultimiDisjuncte()
        {
            
            for(int i = 0; i < _reguliProductie.Count-1; i++)
            {
                for(int j = i + 1; j < _reguliProductie.Count; j++)
                {
                    if(_reguliProductie[i].GetLeft() == _reguliProductie[j].GetLeft())
                    {
                        if (!Check(i, j))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        #endregion

        #region TAB

        public void TabelaDeAnalizaSintactica()
        {
            _tabelaAnalizaSintactica = new DataTable("TabelaAnalizaSintactica");
            int columnIndex = 0;

            _tabelaAnalizaSintactica.Columns.Add(new DataColumn
            {
                ColumnName = columnIndex.ToString(),
                Caption = "Row_ID",
                DataType = typeof(object),
            });
            columnIndex++;
           foreach (var terminal in _terminale)
            {
                _tabelaAnalizaSintactica.Columns.Add(new DataColumn
                {
                    ColumnName = columnIndex.ToString(),
                    //ColumnName = terminal,
                    Caption = terminal,
                    DataType=typeof(object)
                });
                columnIndex++;

            }
            _tabelaAnalizaSintactica.Columns.Add(new DataColumn
            {
                ColumnName = columnIndex.ToString(),
               //ColumnName = "$",
                Caption = "$",
                DataType = typeof(object),
            });
            columnIndex++;
            int i = 0;
            foreach (var neterminal in _neterminale)
            {
                DataRow dr = _tabelaAnalizaSintactica.NewRow();
                dr[0] = neterminal;
                _tabelaAnalizaSintactica.Rows.InsertAt(dr, i);
                i++;

            }
            i += _neterminale.Count;
            
            foreach(var terminal in _terminale)
            {
                DataRow dr = _tabelaAnalizaSintactica.NewRow();
                dr[0] = terminal;
                _tabelaAnalizaSintactica.Rows.InsertAt(dr, i);
                i++;
            }
            i += _terminale.Count;
            
            DataRow d = _tabelaAnalizaSintactica.NewRow();
            d[0] = "$";
            _tabelaAnalizaSintactica.Rows.InsertAt(d, i);


           // PrintTable(_tabelaAnalizaSintactica);
          
        }

        public void PopulareTabela()
        {
            for (int i=0; i < _neterminale.Count + _terminale.Count + 1; i++)
            {
                for(int j=1;j<= _terminale.Count + 1; j++)
                {
                    if (_tabelaAnalizaSintactica.Rows[i][0].ToString() == _tabelaAnalizaSintactica.Columns[j].Caption)
                    {
                        _tabelaAnalizaSintactica.Rows[i][j] = 'P';
                    }

                    for (int k = 0; k < _reguliProductie.Count; k++)
                    {
                        if (_tabelaAnalizaSintactica.Rows[i][0].ToString() == _reguliProductie[k].GetLeft())
                        {
                            foreach (var simbol in SimboliDirectori[k])
                            {
                                if (_tabelaAnalizaSintactica.Columns[j].Caption == simbol)
                                {
                                    _tabelaAnalizaSintactica.Rows[i][j] = _reguliProductie[k];
                                }
                            }
                        }
                    }
                }
                
            }
            _tabelaAnalizaSintactica.Rows[_neterminale.Count + _terminale.Count][_terminale.Count+1] = 'A';

            
            PrintTable(_tabelaAnalizaSintactica);
        }

        #endregion


    }
}

    

