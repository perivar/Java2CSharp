using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Java2CSharp.Rules
{
    public class RemoveGetRule : EquivalentRule
    {
        public override string GetReplacement()
        {
            return "GetXXX()";
        }

        public override string Pattern
        {
            get { return @"Get(FirstRow|Col|XFIndex|Data|LastRow|Column|Sid|Author|TotalSize|Row|FirstColumn|LastColumn|InnerValueEval|Height|Width|StringValue|ExternSheetIndex|Sheet|ColumnIndex|RowIndex)\(\)"; }
        }

        protected override string ReplaceString(Match match)
        {
            return match.Groups[1].Value;
        }
    }
}
