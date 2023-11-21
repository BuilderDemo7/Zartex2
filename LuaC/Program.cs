using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        static string Title = $"Driv3r / Driver: PL Lua Compiler\nVersion: {Version:F1}";
        static void ShowAllArguments()
        {
            Console.WriteLine(" -dpl (driver4) --> Tells the Lua compiler this is a Driver: PL Lua mission file");
            Console.WriteLine(" -o (output) --> Set the output file (default: mission.mpc)");
        }
        public static bool ProcessCompilationForLuaMissionScript(LuaMissionScript luaMission,string output)
        {
            FileStream outputFile = new FileStream(output, FileMode.OpenOrCreate);

            if (outputFile!=null)
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
                MissionPackage.MissionSummary.CityType = luaMission.missionSummary.GetCityTypeByName(luaMission.missionSummary.Level);
                MissionPackage.MissionSummary.MissionId = luaMission.missionSummary.MoodId;

                return MissionPackage.Save(output);
            }
            return false;
        }
        static void Main(string[] args)
        {

            const string fatalErr = "FATAL ERROR: ";

            string outputFileName;
            string inputFileName;

            Console.WriteLine(Title);
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
                                outputFileName = args[argId + 1];
                                if (!File.Exists(outputFileName)) {
                                    Console.WriteLine(fatalErr+"Output file name can't be empty!");
                                    Environment.Exit(0);
                                    return;
                                }
                                continue;
                        }
                    }
                    argId++;
                }

                // now with all arguments set up, let's do our thing!
                LuaMissionScript luaMission;
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
