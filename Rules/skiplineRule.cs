using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Java2CSharp.Rules
{
    public class skiplineRule : Rule
    {
        public override bool Execute(string strOrigin, out string strOutput, int iRowNumber)
        {
            Regex regex = new Regex(@"(import|using)\s(javax?\.)", RegexOptions.IgnoreCase);

            if (regex.IsMatch(strOrigin))
            {
                strOutput = string.Empty;
                return true;
            }
            strOutput = strOrigin;
            return false;
        }
        public override string RuleName
        {
            get { return "Skip this line"; }
        }
    }
}
