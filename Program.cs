using System;
using System.IO;
using Java2CSharp.Configuration;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;

namespace Java2CSharp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Name = "Java2CSharp";
            app.Description = ".NET Core Java to CSharp Converter";
            app.HelpOption();

            app.Command("dir", (command) =>
                {
                    command.Description = "Convert all files in directory to another directory";
                    command.HelpOption();

                    // arguments
                    var srcDirArgument = command.Argument("[source-directory]", "Source-directory with Java files");
                    var targetDirArgument = command.Argument("[target-directory]", "Target-directory for CSharp files");

                    // options
                    //var optionSkipDuration = command.Option("-d|--skipduration <NUMBER>", "Skip files longer than x seconds", CommandOptionType.SingleValue);
                    //var argumentSilent = command.Option("-s|--silent", "Do not output so much info", CommandOptionType.NoValue);

                    command.OnExecute(() =>
                        {
                            if (!string.IsNullOrEmpty(srcDirArgument.Value)
                            && !string.IsNullOrEmpty(targetDirArgument.Value))
                            {
                                var builder = new ConfigurationBuilder()
                                                .SetBasePath(Directory.GetCurrentDirectory())
                                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                                var config = builder.Build();

                                RuleEngine re = new RuleEngine(config);
                                re.LoadRules();
                                re.ConvertDirectory(srcDirArgument.Value, targetDirArgument.Value);

                                return 0;
                            }
                            else
                            {
                                command.ShowHelp();
                                return 1;
                            }
                        });
                });

            app.Command("file", (command) =>
                {
                    command.Description = "Convert a java-file to a csharp-file (.cs)";
                    command.HelpOption();

                    // arguments
                    var srcFileArgument = command.Argument("[source-file]", "Java filepath");
                    var targetFileArgument = command.Argument("[target-file]", "Target filepath");

                    // options
                    //var optionMatchThreshold = command.Option("-t|--threshold <NUMBER>", "Threshold votes for a match. Default: 4", CommandOptionType.SingleValue);
                    //var optionMaxNumber = command.Option("-n|--num <NUMBER>", "Maximal number of matches to return when querying. Default: 25", CommandOptionType.SingleValue);

                    command.OnExecute(() =>
                        {
                            if (!string.IsNullOrEmpty(srcFileArgument.Value)
                            && !string.IsNullOrEmpty(targetFileArgument.Value))
                            {
                                if (!File.Exists(srcFileArgument.Value))
                                {
                                    Console.WriteLine("Source-file not found.");
                                    return 1;
                                }

                                var builder = new ConfigurationBuilder()
                                                .SetBasePath(Directory.GetCurrentDirectory())
                                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                                var config = builder.Build();

                                RuleEngine re = new RuleEngine(config);
                                re.LoadRules();
                                re.ConvertFile(srcFileArgument.Value, targetFileArgument.Value);

                                return 0;
                            }
                            else
                            {
                                command.ShowHelp();
                                return 1;
                            }
                        });
                });

            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 1;
            });

            app.Execute(args);
        }
    }
}
