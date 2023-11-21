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

using MoonSharp;
using MoonSharp.Interpreter;

namespace LuaC
{
    class Program
    {
        public static float Version = 1.01f;

        static readonly string ArgMagic = "-";
        static bool isDPL = false;

        static readonly string fatalErr = "FATAL ERROR: ";

        static readonly string programDir = Directory.GetCurrentDirectory();

        static string Title = $"# Driv3r / Driver: PL Lua Compiler\n# Version: {Version:F2}\n# By BuilderDemo7";
        static void ShowAllArguments()
        {
            Console.WriteLine(" -dpl (driver4) --> Tells the Lua compiler this is a Driver: PL Lua mission file");
            Console.WriteLine(" -o (output) --> Set the output file (default: mission.mpc)");
        }
        public static bool ProcessCompilationForLuaMissionScript(LuaMissionScript luaMission,string output)
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
                Console.WriteLine(fatalErr+"template.mpc was not found in the directory of the tool or in a local directory!");
                Console.WriteLine(" Solutions:");
                Console.WriteLine($"     Copy a .mpc file as {output}");
                Console.WriteLine($"OR   Copy a .mpc file in {Path.GetDirectoryName(output)}/{Path.GetFileName(output)}");
                return false;
            }
            else
            {
                if (!File.Exists(output))
                   File.Copy(programDir + "/template.mpc", output);
            }*/
                if (!File.Exists(output))
                {
                    FileStream newFile = new FileStream(output, FileMode.Create, FileAccess.Write);
                    //byte[] template = ReadResource("templateExportMission");
                    newFile.Write(Properties.Resources.templateMissionFile);

                    newFile.Close();
                    newFile.Dispose();
                }

                if (File.Exists(output))
                {
                    MissionScriptFile MissionPackage = new MissionScriptFile("template.mpc");
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
                    MissionPackage.MissionSummary.CityType = luaMission.missionSummary.GetCityTypeByName(luaMission.missionSummary.Level);
                    MissionPackage.MissionSummary.MissionId = luaMission.missionSummary.MoodId;

                    bool stat = MissionPackage.Save(output);
                    MissionPackage.Dispose();
                    return stat;
                }
            return false;
        }
        static void Main(string[] args)
        {
            string outputFileName;
            string inputFileName;

            Console.WriteLine(Title);
            Console.WriteLine("");
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
                                Console.WriteLine($"WARNING: Unknown command '{command}'");
                                continue;
                            case "dpl":
                            case "driver4":
                                isDPL = true;
                                continue;
                            case "output":
                            case "o":
                                outputFileName = args[argId+1];
                                if (outputFileName=="") {
                                    Console.WriteLine(fatalErr+$"Output file can't be empty!");
                                    Environment.Exit(0);
                                    return;
                                }
                                continue;
                        }
                    }
                }

                // now with all arguments set up, let's do our thing!
                LuaMissionScript luaMission = new LuaMissionScript();
                Console.WriteLine("Compiling script...");
                try
                {
                    if (!isDPL)
                    {
                        luaMission = Zartex.Main.importLuaFromFile(inputFileName);
                    }
                    else
                    {
                        luaMission = Zartex.Main.importLuaFromFileDPL(inputFileName);
                    }
                }
                catch (ScriptRuntimeException ex)
                {
                    Console.WriteLine("LUA COMPILER ERROR:  ");
                    Console.WriteLine(ex.DecoratedMessage);
                    Console.WriteLine("\t" + ex.Source);
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(); // prevent console from closing up so we can see the error
                }
                Console.WriteLine("Making .mpc file...");
                if (ProcessCompilationForLuaMissionScript(luaMission, outputFileName)==true)
                {
                    Console.WriteLine("Success!\noutputed to " + Path.GetFullPath(outputFileName));
                }
                else
                {
                    Console.WriteLine("Something went wrong, please try again later.");
                }
                Console.WriteLine("");
            }
            else
            {
                Console.WriteLine("Nothing inputed");
                Console.WriteLine("Valid arguments list:");
                ShowAllArguments();
            }
        }
    }
}
