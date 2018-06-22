using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using Java2CSharp.Configuration;
using System.Configuration;
using Java2CSharp.Rules;
using Microsoft.Extensions.Configuration;

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

        public void Run(string path)
        {
            // read from file
            if (!File.Exists(path))
            {
                Console.WriteLine("File not found");
                return;
            }
            StreamReader sr = new StreamReader(path, true);
            string strOrigin = sr.ReadToEnd();
            sr.Close();

            // run rules
            StringBuilder sb = new StringBuilder();
            string[] arrInput = strOrigin.Split(new char[] { '\n' });
            for (int i = 0; i < arrInput.Length; i++)
            {
                string tmp = arrInput[i].Replace("\r", "");

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

            // save result to file
            StreamWriter sw = new StreamWriter(path + ".cs", false);
            sw.Write(sb.ToString());
            sw.Close();
        }
    }
}
