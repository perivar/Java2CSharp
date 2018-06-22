using System;
using System.IO;
using Java2CSharp.Configuration;
using Microsoft.Extensions.Configuration;

namespace Java2CSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Missing Argument - file name");
                return;
            }

            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var config = builder.Build();

            RuleEngine re = new RuleEngine(config);
            re.LoadRules();
            re.Run(args[0]);
        }
    }
}
