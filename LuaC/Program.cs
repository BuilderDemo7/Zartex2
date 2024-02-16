using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;

using Zartex;

using DSCript.Spooling;
using MoonSharp;
using MoonSharp.Interpreter;

namespace LuaC
{
    class Program
    {
        public static float Version = 1.03f;

        static readonly string ArgMagic = "-";
        static bool isDPL = false;

        static readonly string fatalErr = "FATAL ERROR: ";

        static readonly string programDir = Directory.GetCurrentDirectory();

        static bool Log = false;
        static string LogFileName = "compile";
        public static StringWriter consoleLog = new StringWriter();
        public static void WriteLine(string line)
        {
            consoleLog.WriteLine(line);
            Console.WriteLine(line);
        }

        // Eh.. I don't care about the title I think...
#if USE_TITLE
        static string Title = $"# Driv3r / Driver: PL Lua Compiler\n# Version: {Version:F2}\n# By BuilderDemo7";
#endif
        static void ShowAllArguments()
        {
            WriteLine(" -dpl (driver4) --> Tells the Lua compiler this is a Driver: PL Lua mission file");
            WriteLine(" -o (output) --> Set the output file (default: mission.mpc)");
        }
        public static bool ProcessCompilationForLuaMissionScript(LuaMissionScript luaMission, string output, string buildInfo = null)
        {
            /*
            if (!File.Exists(output))
            {
                FileStream templateFile = new FileStream("template.mpc", FileMode.Open, FileAccess.Read);
                FileStream outputFile = new FileStream(output, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                outputFile.Write(templateFile.ReadAllBytes());
                outputFile.Dispose();
            }
            */
            /*
            if (!File.Exists(programDir+"/template.mpc"))
            {
                WriteLine(fatalErr+"template.mpc was not found in the directory of the tool or in a local directory!");
                WriteLine(" Solutions:");
                WriteLine($"     Copy a .mpc file as {output}");
                WriteLine($"OR   Copy a .mpc file in {Path.GetDirectoryName(output)}/{Path.GetFileName(output)}");
                return false;
            }
            else
            {
                if (!File.Exists(output))
                   File.Copy(programDir + "/template.mpc", output);
            }*/
                if (!File.Exists(output))
                {
                    FileStream newFile = new FileStream(output, FileMode.OpenOrCreate, FileAccess.Write);
                    //byte[] template = ReadResource("templateExportMission");
                    newFile.Write(Properties.Resources.templateMissionFile);

                    newFile.Close();
                    newFile.Dispose();
                }

                if (File.Exists(output))
                {
                    MissionScriptFile MissionPackage = new MissionScriptFile(output);
                    // copy data from lua mission to current mission
                    MissionPackage.MissionData.LogicData.Actors.Definitions = luaMission.missionData.LogicData.Actors.Definitions;
                    MissionPackage.MissionData.LogicData.Nodes.Definitions = luaMission.missionData.LogicData.Nodes.Definitions;
                    MissionPackage.MissionData.LogicData.WireCollection.WireCollections = luaMission.wireCollection;

                    MissionPackage.MissionData.Objects.Objects = luaMission.missionData.Objects.Objects;

                    MissionPackage.MissionData.LogicData.StringCollection.Strings = luaMission.missionData.LogicData.StringCollection.Strings;

                    MissionPackage.MissionData.LogicData.SoundBankTable.Table = luaMission.missionData.LogicData.SoundBankTable.Table;

                    if (MissionPackage.MissionSummary == null)
                        MissionPackage.MissionSummary = new MissionSummaryData();
                    MissionPackage.MissionSummary.StartPosition = luaMission.missionSummary.StartPosition;
                    MissionPackage.MissionSummary.CityType = MissionSummary.GetCityTypeByName(luaMission.missionSummary.Level);
                    MissionPackage.MissionSummary.MissionId = luaMission.missionSummary.MoodId;

                    if (buildInfo != null)
                    {
                       MissionPackage.BuildInfo = buildInfo;
                    }

                    bool stat = MissionPackage.Save();

                    MissionPackage.Dispose();
                    return stat;
                }
            return false;
        }
        static void Main(string[] args)
        {
            string outputFileName;
            string inputFileName;

#if USE_TITLE
            WriteLine(Title);
            WriteLine("");
#endif
            if (args.Length > 0)
            {
                outputFileName = "mission.mpc"; // default
                inputFileName = args[0];

                int argId = 0;
                // argument processing
                foreach (var arg in args)
                {
                    if (arg.Contains(ArgMagic)) // contains magic for commands
                    {
                        argId++;
                        var command = arg.Replace(ArgMagic, "");
                        switch (command)
                        {
                            default:
                                // oh no, not recognized command!!
                                WriteLine($"WARNING: Unknown command '{command}'");
                                continue;
                            case "dpl":
                            case "driver4":
                                isDPL = true;
                                continue;
                            case "l":
                            case "log":
                                Log = true;
                                continue;
                            case "output":
                            case "o":
                                outputFileName = args[argId+1];
                                if (outputFileName=="") {
                                    WriteLine(fatalErr+$"Output file name can't be empty!");
                                    Environment.Exit(0);
                                    return;
                                }
                                continue;
                        }
                    }
                }

                // now with all arguments set up, let's do our thing!
                LuaMissionScript luaMission = new LuaMissionScript();
                WriteLine("1>------ Compilation started ------");
                try
                {
                    if (!isDPL)
                    {
                        luaMission = LuaMissionScript.LoadScriptFromFile(inputFileName);
                    }
                    else
                    {
                        luaMission = LuaMissionScript.LoadScriptFromFile(inputFileName);
                    }
                }
                catch (Exception ex)
                {
                    if (ex is ScriptRuntimeException)
                    {
                        WriteLine("!>Lua Runtime Error");
                        WriteLine(((ScriptRuntimeException)ex).DecoratedMessage);
                    }
                    else if (ex is SyntaxErrorException)
                    {
                        WriteLine("!>Lua Syntax Error");
                        WriteLine(((SyntaxErrorException)ex).DecoratedMessage);
                    }
                    else
                    {
                        WriteLine("!>Unhandled Exception");
                        WriteLine(ex.Message);

                    }
                    WriteLine("1> Exception Source -> " + ex.Source);
                    WriteLine("1> Fix your errors before you can compile!");
                    WriteLine("========== Compilation failed ==========");
                    return;
                    //WriteLine("Press any key to continue...");
                    //Console.ReadKey(); // prevent console from closing up so we can see the error
                }
                //WriteLine($"Outputing it to file {outputFileName}...");
                bool compileStatus = ProcessCompilationForLuaMissionScript(luaMission, outputFileName);// $"The following mission script was compiled with LuaC\n\tSource file name: {inputFileName}\n\tDate: {DateTime.Now.ToString()}");
                if (compileStatus == true)
                {
                    WriteLine($"1> {Path.GetFullPath(inputFileName)} -> {Path.GetFullPath(outputFileName)}");
                    WriteLine("========== Compilation success =========="); //\noutputed to " + Path.GetFullPath(outputFileName));
                }
                else
                {
                    WriteLine("========== Compilation failed ==========");
                }
                // output log
                if (Log)
                {
                    TextWriter writer = File.CreateText(LogFileName+".log");
                    writer.Write(consoleLog.ToString());
                    writer.Flush();
                    writer.Close();
                }
            }
            else
            {
                WriteLine("Nothing inputed");
                WriteLine("Valid arguments list:");
                ShowAllArguments();
            }
        }
    }
}
