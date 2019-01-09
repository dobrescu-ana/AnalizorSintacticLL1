using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analizor_LL_1_
{
    public class ReguliDeProductie
    {
        string leftPart;
        string[] rightPart = null;

        #region INITIALIZARE

        public void InitializeRightPartOfExistingRules(string[] right)
        {
            rightPart = new string[right.Length - 2];
            Array.Copy(right, 2, rightPart, 0, right.Length - 2);
        }

        #endregion
  
        #region SET METHODS

        public void SetLeft(string left) => leftPart = left;

        public void SetRight(string[] right)
        {
            rightPart = new string[right.Length];
            Array.Copy(right, 0, rightPart, 0, right.Length);
        }

        #endregion

        #region GET METHODS
        public string GetLeft() => leftPart;

        public string[] GetRight() => rightPart;

        public int GetRightPartNumberOfElements() => rightPart.Count();

        public string GET()
        {
            string st = leftPart + " -> ";
            foreach(var i in rightPart)
            {
                st = st + i + " ";
            }
            return st;}

        #endregion

        #region PRINT METHODS

        public override string ToString()
        {
            string result = leftPart + " -> ";
            foreach(var item in rightPart)
            {
                result += (item + " ");
            }
            return result;
        }

        #endregion
    }
}
