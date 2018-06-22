using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using Java2CSharp.Configuration;
using System.Configuration;
using Java2CSharp.Rules;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using System.Linq;

namespace Java2CSharp
{
    public class RuleEngine
    {
        private IConfigurationRoot config = null;
        private List<Rule> rules = new List<Rule>();

        public RuleEngine(IConfigurationRoot config)
        {
            this.config = config;
        }

        public void AddRule(Rule rule)
        {
            rules.Add(rule);
        }

        /// <summary>
        /// Loads the rules.
        /// </summary>
        public void LoadRules()
        {
            // load from appsettings.json
            var rules = config.GetSection("J2CS:Rules:add").Get<List<RuleSection>>();

            foreach (RuleSection rs in rules)
            {
                EquivalentRule r = new EquivalentRule(rs.Name);
                r.Pattern = rs.Pattern;
                r.SetReplacement(rs.Replacement);
                AddRule(r);
            }

            // load from assembly
            Assembly asm = Assembly.GetExecutingAssembly();
            string nameSpace = "Java2CSharp.Rules";
            foreach (Type type in asm.GetTypes())
            {
                if (type.Namespace == nameSpace && !type.IsAbstract && type.Name != "EquivalentRule")
                    AddRule(Activator.CreateInstance(type) as Rule);
            }
        }

        protected static void Log(string message)
        {
            Console.WriteLine(message);
        }
        protected static void LogTip(string message)
        {
            Console.WriteLine(message);
        }

        protected void LogExecute(string ruleName, int iRowNumber)
        {
            Log(string.Format("[Line {0}] {1}", iRowNumber, ruleName));
        }

        public void Run(string sourceDirectory, string targetDirectory)
        {
            // read from file
            if (!Directory.Exists(sourceDirectory))
            {
                Console.WriteLine("Source-directory not found.");
                return;
            }
            if (!Directory.Exists(targetDirectory))
            {
                Console.WriteLine("Target-directory not found.");
                return;
            }

            IEnumerable<string> javaFiles =
                Directory.EnumerateFiles(sourceDirectory, "*", SearchOption.AllDirectories)
                .Where(f => ".java".Equals(Path.GetExtension(f).ToLower()));
            Console.Out.WriteLine("Found {0} files in scan directory.", javaFiles.Count());

            foreach (var javaFile in javaFiles)
            {
                string directoryPath = Path.GetDirectoryName(javaFile);
                string relativePath = Path.GetRelativePath(sourceDirectory, directoryPath);
                string newFileName = Path.GetFileNameWithoutExtension(javaFile) + ".cs";

                string destinationFilePath = "";
                if (!relativePath.Equals("."))
                {
                    destinationFilePath = Path.Combine(new string[] { targetDirectory, relativePath, newFileName });
                }
                else
                {
                    destinationFilePath = Path.Combine(new string[] { targetDirectory, newFileName });
                }

                ConvertFile(javaFile, destinationFilePath);
            }
        }

        private bool ConvertFile(string sourceFilePath, string destinationFilePath)
        {
            var sr = new StreamReader(sourceFilePath, true);
            string strOrigin = sr.ReadToEnd();
            sr.Close();

            // run rules
            var sb = new StringBuilder();
            var arrInput = strOrigin.Split(new char[] { '\n' });
            for (int i = 0; i < arrInput.Length; i++)
            {
                var tmp = arrInput[i].Replace("\r", "");

                int ruleNum = i + 1;
                foreach (Rule rule in rules)
                {
                    if (rule.Execute(tmp, out tmp, ruleNum))
                    {
                        LogExecute(rule.RuleName, ruleNum);
                    }
                }
                sb.AppendLine(tmp);
            }
            string convertedFile = sb.ToString();

            // remove double newlines
            string result = Regex.Replace(convertedFile, @"[\r\n]{3,}", "\r\n\r\n");

            // add using System;
            result = "using System;\r\nusing System.Text;\r\n\r\n" + result;

            // fix namespace like namespace com.mpatric.mp3agic;
            // ?: is non-capturing group
            // ?<element> is a named captured group
            var regNameSpace = new Regex(@"namespace (\w+(?:\.(?<element>\w+))*);", RegexOptions.Multiline);

            // match the input and write results
            Match match = regNameSpace.Match(result);
            if (match.Success)
            {
                var fullMatch = match.Groups[0].Value;

                // get number of matches
                int lastMatch = match.Groups["element"].Captures.Count;

                // get last group match
                var nameSpaceName = match.Groups[lastMatch].Value;      

                // create new namespace string and add { 
                var nameSpace = string.Format("namespace {0} {{", nameSpaceName);

                // replace
                result = Regex.Replace(result, fullMatch, nameSpace);          
            }

            // add } at the very end
            result += "\r\n}";

            // save result to file
            var sw = new StreamWriter(destinationFilePath, false);
            sw.Write(result);
            sw.Close();

            return true;
        }
    }
}
