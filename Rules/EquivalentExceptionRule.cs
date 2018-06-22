using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Java2CSharp.Rules
{
    public abstract class EquivalentExceptionRule : EquivalentRule
    {
        public override string GetReplacement()
        {
            return this.Pattern;
        }
    }

    public class EquivalentRuleException1 : EquivalentExceptionRule
    {

        public override string Pattern
        {
            get { return @"RuntimeException"; }
        }

        protected override string ReplaceString(Match match)
        {
            return "Exception";
        }
    }

    public class EquivalentRuleException2 : EquivalentExceptionRule
    {

        public override string Pattern
        {
            get { return @"ClassCastException"; }
        }

        protected override string ReplaceString(Match match)
        {
            return "InvalidCastException";
        }
    }

    public class EquivalentRuleException3 : EquivalentExceptionRule
    {

        public override string Pattern
        {
            get { return @"UnsupportedEncodingException"; }
        }

        protected override string ReplaceString(Match match)
        {
            return "EncoderFallbackException";
        }
    }

    public class EquivalentRuleException4 : EquivalentExceptionRule
    {
        public override string Pattern
        {
            get { return @"AssertionFailedError"; }
        }

        protected override string ReplaceString(Match match)
        {
            return "AssertFailedException";
        }
    }

    public class EquivalentRuleException5 : EquivalentExceptionRule
    {
        public override string Pattern
        {
            get { return @"IllegalArgumentException"; }
        }

        protected override string ReplaceString(Match match)
        {
            return "ArgumentException";
        }
    }
    public class EquivalentRuleException6 : EquivalentExceptionRule
    {
        public override string Pattern
        {
            get { return @"NumberFormatException"; }
        }

        protected override string ReplaceString(Match match)
        {
            return "FormatException";
        }
    }
    public class EquivalentRuleException7 : EquivalentExceptionRule
    {
        public override string Pattern
        {
            get { return @"IllegalAccessError|IllegalStateException"; }
        }

        protected override string ReplaceString(Match match)
        {
            return "InvalidOperationException";
        }
    }

}
