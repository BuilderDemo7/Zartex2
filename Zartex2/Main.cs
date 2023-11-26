using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;
using System.Windows.Forms;

using DSCript;

using MoonSharp;
using MoonSharp.Interpreter;

using Zartex.Converters;
using Zartex.Settings;

using MemoryEdit;

using System.Windows.Forms.Integration; // WPF hack

// HACK: Fix discrepencies between "Form.DialogResult" and "System.Windows.Forms.DialogResult"
using DialogResult = System.Windows.Forms.DialogResult;

namespace Zartex
{
    public partial class Main : Form
    {
        public string info_Version = "1.0.47"; // version of the tool
        //public int LuaContext = 1396790604; // "LUAS"
        public InspectorWidget Widget;

        public Process gameProcess;
        public Vector3 lastPosition = new Vector3(0,0,0);
        public bool isDriverPLMission = false;

        static Main()
        {
            DSC.VerifyGameDirectory("Driv3r", "Zartex");

            TypeDescriptor.AddAttributes(typeof(Vector2), new TypeConverterAttribute(typeof(VectorTypeConverter)));
            TypeDescriptor.AddAttributes(typeof(Vector3), new TypeConverterAttribute(typeof(VectorTypeConverter)));
            TypeDescriptor.AddAttributes(typeof(Vector4), new TypeConverterAttribute(typeof(VectorTypeConverter)));

            var culture = new CultureInfo("en-US", false);
            var thread = Thread.CurrentThread;

            thread.CurrentCulture = culture;
            thread.CurrentUICulture = culture;
        }

        string title;

        MissionScriptFile MissionPackage;

        OpenFileDialog ScriptFile = new OpenFileDialog() {
            Title = "Select a mission script",
            Filter = "Mission Script|*.mpc;*.mps;*.mxb|All files|*.*",
            InitialDirectory = MPCFile.GetMissionScriptDirectory(),
        };
        
        string Filename;

        static Color darkThemeColor1 = Color.FromArgb(50,50,50);
        static Color darkThemeColor2 = Color.White;

        static Color DeactivatedarkThemeColor1;
        static Color DeactivatedarkThemeColor2 = Color.Black;
        public void foreachItemInSetColors(ToolStripItemCollection col, Color c1,Color c2)
        {
            foreach (var btn in col)
            {
                if (btn is ToolStripMenuItem)
                {
                    var button = btn as ToolStripMenuItem;
                    button.BackColor = c1;
                    button.ForeColor = c2;
                }
                if (btn is ToolStripSeparator)
                {
                    var sep = btn as ToolStripSeparator;
                    sep.BackColor = c1;
                    sep.ForeColor = c2;
                }
                if (btn is ToolStripMenuItem)
                {
                    ToolStripMenuItem button = btn as ToolStripMenuItem;
                    // loop loop loop
                    foreachItemInSetColors(button.DropDownItems, c1, c2);
                }
            }
        }
        public void toggleDarkTheme()
        {
            if (DeactivatedarkThemeColor1==null)
            DeactivatedarkThemeColor1 = MenuBar.BackColor;

            Color c1;
            Color c2;
            if (darkThemeBTN.Checked)
            {
                c1 = darkThemeColor1;
                c2 = darkThemeColor2;
            }
            else
            {
                c1 = DeactivatedarkThemeColor1;
                c2 = DeactivatedarkThemeColor2;
            }
            // set menu bar color
            MenuBar.BackColor = c1;
            foreachItemInSetColors(MenuBar.Items, c1, c2);
        }

        public Main()
        {
            InitializeComponent();
            PopulateMainMenu();

            title = this.Text;
            
            foreach (Control control in Controls.Find("LeftMenu", true)[0].Controls)
            {
                if (control.Name.StartsWith("btn"))
                {
                    var b = BitConverter.ToInt32(Encoding.UTF8.GetBytes(control.Name.Substring(3, (control.Name.Length - 3))), 0);

                    control.Click += (o, e) => ChunkButtonClick((Button)o, b);

                    Console.WriteLine("Added an event handler to {0}", control.Name);
                }
            }
        }

        private void InitTools()
        {
            if (MissionPackage.IsLoaded)
            {
                Text = String.Format("{0} - {1}", title, Filename);
                GenerateLogicNodes();
            }

            mnFile_Save.Enabled = MissionPackage.IsLoaded;
            mnFile_SaveAs.Enabled = MissionPackage.IsLoaded;

            importMPCBTN.Enabled = true;
            importLuaScript.Enabled = true;

            exportAsBTN.Enabled = true;
        }

        private void LoadScriptFile(int missionId)
        {
            LoadScriptFile(Driv3r.GetMissionScript(missionId));
        }

        private void LoadScriptFile(string filename,bool isDriverPLMission = false,int missionId=0)
        {
            Filename = filename;
            MissionPackage = new MissionScriptFile(Filename, isDriverPLMission, missionId);

            MissionPackage.FileName = filename;
            MissionPackage.IsLoaded = true;
            InitTools();
        }
        
        public void MenuLoadMission(object sender, EventArgs e)
        {
            int missionID = (int)((ToolStripMenuItem)sender).Tag;

            // close the old file
            if (MissionPackage != null)
            {
                MissionPackage.Dispose();
                MissionPackage = null;
            }
            isDriverPLMission = false; // tell it's Driv3r mission
            LoadScriptFile(missionID);
        }

        private void MenuLoadFile(object sender, EventArgs e)
        {
            var result = ScriptFile.ShowDialog();

            if (result == DialogResult.OK)
            {
                // close the old file

                // but before get some stuff
                //var isDPL = MissionPackage.isDriverPLMission;
                //var mIdx = MissionPackage.MissionIndex;

                if (MissionPackage != null)
                {
                    MissionPackage.Dispose();
                    MissionPackage = null;
                }
                ScriptFile.InitialDirectory = Path.GetDirectoryName(ScriptFile.FileName);

                LoadScriptFile(ScriptFile.FileName/*, isDPL, mIdx*/);
            }
        }

        private void MenuSaveFileAs(object sender, EventArgs e)
        {
            SaveFileDialog mpcFile = new SaveFileDialog()
            {
                Title = "Save mission script file as",
                Filter = "Mission Script|*.mpc;*.mps;*.mxb|Mission Package|*.sp|Any|*.*",
            };
            if (mpcFile.ShowDialog() == DialogResult.OK)
            {
                var filename = mpcFile.FileName;

                // backup
                var bakFile = filename + ".bak";

                if (File.Exists(bakFile))
                {
                    var idx = 1;

                    while (File.Exists(bakFile + idx))
                        idx++;

                    bakFile += idx;
                }

                File.Copy(MissionPackage.FileName, bakFile);
                //Debug.WriteLine($"[ZARTEX] Is Parent from logic LEPR equals to LEND: {MissionPackage.MissionData.LogicData.Nodes.Spooler.Parent == MissionPackage.MissionData.LogicData.Spooler}");

                if (MissionPackage.Save(filename))
                {
                    var result = MessageBox.Show(String.Format("Successfully saved to \"{0}\"!", Path.GetFullPath(filename).ToString()),
                        "Zartex", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (result == DialogResult.OK)
                    {
                        //var filename = MissionPackage.FileName;

                        var isDPL = MissionPackage.isDriverPLMission;
                        var mIdx = MissionPackage.MissionIndex;

                        // close the old file
                        MissionPackage.Dispose();
                        MissionPackage = null;

                        // reopen it
                        LoadScriptFile(filename, isDPL, mIdx);
                    }
                }
                else
                {
                    MessageBox.Show("Failed to save file!",
                        "Zartex", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void MenuSaveFile(object sender, EventArgs e)
        {
            var bakFile = MissionPackage.FileName + ".bak";
            
            if (File.Exists(bakFile))
            {
                var idx = 1;

                while (File.Exists(bakFile + idx))
                    idx++;

                bakFile += idx;
            }
            
            File.Copy(MissionPackage.FileName, bakFile);

            // prevent errors on saving with "unknown property type" error
            fixNullProperitiesInActors(); 
            fixNullTypesInObjects();

            if (MissionPackage.Save())
            {
                var result = MessageBox.Show(String.Format("Successfully saved to \"{0}\"!", MissionPackage.FileName),
                    "Zartex", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (result == DialogResult.OK)
                {
                    var filename = MissionPackage.FileName;

                    var isDPL = MissionPackage.isDriverPLMission;
                    var mIdx = MissionPackage.MissionIndex;

                    // close the old file
                    MissionPackage.Dispose();
                    MissionPackage = null;

                    // reopen it
                    LoadScriptFile(filename, isDPL, mIdx);
                }
            }
            else
            {
                MessageBox.Show("Failed to save file!",
                    "Zartex", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public enum CityType
        {
            Miami,
            Nice,
            Istanbul,
        }

        public enum GameModeType
        {
            Undercover,

            TakeARide,

            QuickChase,
            QuickGetaway,
            TrailBlazer,
            Survival,
            CheckpointRace,
            GateRace,
        }

        public struct MissionDescriptor
        {
            public string Name { get; }

            public GameModeType GameMode { get; }
            public CityType City { get; }

            public int[] MissionIds { get; }

            public bool HasSubMissions
            {
                get { return MissionIds.Length > 1; }
            }
            
            // driving games/take a ride
            public MissionDescriptor(GameModeType gameMode, CityType city, params int[] missionIds)
                : this($"{gameMode.ToString()}, {city.ToString()}", gameMode, city, missionIds) { }

            // undercover
            public MissionDescriptor(string name, CityType city, params int[] missionIds)
                : this(name, GameModeType.Undercover, city, missionIds) { }

            public MissionDescriptor(string name, GameModeType gameMode, CityType city, params int[] missionIds)
            {
                Name = name;
                City = city;
                GameMode = gameMode;
                MissionIds = missionIds;
            }
        }

        public static readonly MissionDescriptor[] MissionDescriptors = new[] {
            /*
                Undercover (Miami)
            */
            new MissionDescriptor("Police HQ",                  CityType.Miami,         1,  101, 102),
            new MissionDescriptor("Lead on Baccus",             CityType.Miami,         2,  103),
            new MissionDescriptor("The Siege",                  CityType.Miami,         3,  105),
            new MissionDescriptor("Rooftops",                   CityType.Miami,         4,  106, 107),
            new MissionDescriptor("Impress Lomaz",              CityType.Miami,         5,  108, 109, 121),
            new MissionDescriptor("Gator's Yacht",              CityType.Miami,         6,  110, 111),
            new MissionDescriptor("The Hit",                    CityType.Miami,         7,  112, 122),
            new MissionDescriptor("Trapped",                    CityType.Miami,         8,  113, 114, 115),
            new MissionDescriptor("Dodge Island",               CityType.Miami,         9,  116, 117),
            new MissionDescriptor("Retribution",                CityType.Miami,         10, 118, 119, 120),
            /*
                Undercover (Nice)
            */
            new MissionDescriptor("Welcome to Nice",            CityType.Nice,          11, 130, 131),
            new MissionDescriptor("Smash and Run",              CityType.Nice,          13, 134),
            new MissionDescriptor("18-wheeler",                 CityType.Nice,          14, 135, 150),
            new MissionDescriptor("Hijack",                     CityType.Nice,          15, 136),
            new MissionDescriptor("Arms Deal",                  CityType.Nice,          16, 137, 138, 139, 149),
            new MissionDescriptor("Booby Trap",                 CityType.Nice,          17, 140, 151, 152),
            new MissionDescriptor("Calita in Trouble",          CityType.Nice,          18, 141, 142),
            new MissionDescriptor("Rescue Dubois",              CityType.Nice,          19, 143, 144),
            new MissionDescriptor("Hunted",                     CityType.Nice,          21, 146, 147, 148),
            /*
                Undercover (Istanbul)
            */
            new MissionDescriptor("Surveillance",               CityType.Istanbul,      22, 160, 161, 162),
            new MissionDescriptor("Tanner Escapes",             CityType.Istanbul,      24, 164, 165, 180),
            new MissionDescriptor("Another Lead",               CityType.Istanbul,      25, /*166, 167,*/ 168, 181),
            new MissionDescriptor("Alleyway",                   CityType.Istanbul,      27, 171, 172),
            new MissionDescriptor("The Chase",                  CityType.Istanbul,      28, 173, 174),
            new MissionDescriptor("Bomb Truck",                 CityType.Istanbul,      30, 176),
            new MissionDescriptor("Chase the Train",            CityType.Istanbul,      31, 177, 178, 179),
            /*
                Driving games
            */
            new MissionDescriptor(GameModeType.QuickChase,      CityType.Miami,         32, 33),
            new MissionDescriptor(GameModeType.QuickChase,      CityType.Nice,          34, 35),
            new MissionDescriptor(GameModeType.QuickChase,      CityType.Istanbul,      36, 37),
            new MissionDescriptor(GameModeType.QuickGetaway,    CityType.Miami,         38, 39, 40),
            new MissionDescriptor(GameModeType.QuickGetaway,    CityType.Nice,          42, 43, 44),
            new MissionDescriptor(GameModeType.QuickGetaway,    CityType.Istanbul,      46, 47, 48),
            new MissionDescriptor(GameModeType.TrailBlazer,     CityType.Miami,         50, 51),
            new MissionDescriptor(GameModeType.TrailBlazer,     CityType.Nice,          52, 53),
            new MissionDescriptor(GameModeType.TrailBlazer,     CityType.Istanbul,      54, 55),
            new MissionDescriptor(GameModeType.Survival,        CityType.Miami,         56),
            new MissionDescriptor(GameModeType.Survival,        CityType.Nice,          57),
            new MissionDescriptor(GameModeType.Survival,        CityType.Istanbul,      58),
            new MissionDescriptor(GameModeType.CheckpointRace,  CityType.Miami,         59, 60, 61),
            new MissionDescriptor(GameModeType.CheckpointRace,  CityType.Nice,          62, 63, 64),
            new MissionDescriptor(GameModeType.CheckpointRace,  CityType.Istanbul,      65, 66, 67),
            new MissionDescriptor(GameModeType.GateRace,        CityType.Miami,         71, 72),
            new MissionDescriptor(GameModeType.GateRace,        CityType.Nice,          73, 74),
            new MissionDescriptor(GameModeType.GateRace,        CityType.Istanbul,      75, 76),
            /*
                Take a Ride
            */
            new MissionDescriptor(GameModeType.TakeARide,       CityType.Miami,         77, 78),
            new MissionDescriptor(GameModeType.TakeARide,       CityType.Nice,          80, 81),
            new MissionDescriptor(GameModeType.TakeARide,       CityType.Istanbul,      83, 84),
        };

        private ToolStripMenuItem GetMenuItemByCity(CityType city)
        {
            switch (city)
            {
            case CityType.Miami:    return mnMiami;
            case CityType.Nice:     return mnNice;
            case CityType.Istanbul: return mnIstanbul;
            }

            return null;
        }

        private string GetItemNameForMission(MissionDescriptor mission)
        {
            switch (mission.GameMode)
            {
            case GameModeType.TakeARide:        return "Take A Ride";
            case GameModeType.QuickChase:       return "Quick Chase";
            case GameModeType.QuickGetaway:     return "Quick Getaway";
            case GameModeType.TrailBlazer:      return "Trail Blazer";
            case GameModeType.Survival:         return "Survival";
            case GameModeType.CheckpointRace:   return "Checkpoint Race";
            case GameModeType.GateRace:         return "Gate Race";
            }

            return mission.Name;
        }

        private ToolStripMenuItem BuildMenuItem(MissionDescriptor mission)
        {
            var itemText = GetItemNameForMission(mission);
            var menuItem = new ToolStripMenuItem(itemText) {
                Tag = mission.MissionIds[0]
            };

            menuItem.Click += MenuLoadMission;

            return menuItem;
        }

        private ToolStripMenuItem BuildMenuItem(MissionDescriptor mission, List<ToolStripMenuItem> subItems)
        {
            var itemText = GetItemNameForMission(mission);
            var menuItem = new ToolStripMenuItem(itemText);

            foreach (var subItem in subItems)
            {
                if (subItem.Tag != null)
                    subItem.Click += MenuLoadMission;

                menuItem.DropDownItems.Add(subItem);
            }

            return menuItem;
        }

        public void PopulateMainMenu()
        {
            CityType[] cityTypes = {
                CityType.Miami,
                CityType.Nice,
                CityType.Istanbul,
            };

            foreach (var city in cityTypes)
            {
                var menu = GetMenuItemByCity(city);

                if (menu == null)
                    throw new NullReferenceException($"FATAL: Could not find menu item for city '{city.ToString()}'!");

                var cityMissions = MissionDescriptors.Where((m) => m.City == city);

                GameModeType?[] gameModes = {
                    GameModeType.Undercover,
                    null,
                    GameModeType.TakeARide,
                    null,
                    GameModeType.QuickChase,
                    GameModeType.QuickGetaway,
                    GameModeType.TrailBlazer,
                    GameModeType.Survival,
                    GameModeType.CheckpointRace,
                    GameModeType.GateRace,
                };

                foreach (var gameMode in gameModes)
                {
                    IEnumerable<MissionDescriptor> missions = (gameMode != null) 
                        ? cityMissions.Where((m) => m.GameMode == gameMode)
                        : null;

                    if (missions == null)
                    {
                        menu.DropDownItems.Add(new ToolStripSeparator());
                        continue;
                    }

                    ToolStripMenuItem menuItem = null;

                    switch (gameMode)
                    {
                    case GameModeType.Undercover:
                        {
                            menuItem = new ToolStripMenuItem("Undercover");

                            foreach (var mission in missions)
                            {
                                var subItems = new List<ToolStripMenuItem>() {
                                    new ToolStripMenuItem("Intro") { Tag = mission.MissionIds[0] }
                                };

                                for (int i = 1; i < mission.MissionIds.Length; i++)
                                    subItems.Add(new ToolStripMenuItem($"Part {i}") { Tag = mission.MissionIds[i] });

                                var subMenuItem = BuildMenuItem(mission, subItems);
                                menuItem.DropDownItems.Add(subMenuItem);
                            }

                            menu.DropDownItems.Add(menuItem);
                        } break;
                    case GameModeType.TakeARide:
                        {
                            foreach (var mission in missions)
                            {
                                if (menuItem != null)
                                    throw new InvalidOperationException($"Too many Take a Ride missions defined for {city.ToString()}!");

                                var subItems = new List<ToolStripMenuItem>() {
                                    new ToolStripMenuItem("Default")    { Tag = mission.MissionIds[0] },
                                    new ToolStripMenuItem("Semi-truck") { Tag = mission.MissionIds[1] },
                                };

                                menuItem = BuildMenuItem(mission, subItems);
                                menu.DropDownItems.Add(menuItem);
                            }
                        } break;
                    case GameModeType.QuickChase:
                    case GameModeType.QuickGetaway:
                    case GameModeType.TrailBlazer:
                    case GameModeType.CheckpointRace:
                    case GameModeType.GateRace:
                        {
                            foreach (var mission in missions)
                            {
                                if (menuItem != null)
                                    throw new InvalidOperationException($"Too many {gameMode.ToString()} driving games defined for {city.ToString()}!");

                                var subItems = new List<ToolStripMenuItem>();

                                for (int i = 0; i < mission.MissionIds.Length; i++)
                                    subItems.Add(new ToolStripMenuItem($"Sub-game {i + 1}") { Tag = mission.MissionIds[i] });

                                menuItem = BuildMenuItem(mission, subItems);
                                menu.DropDownItems.Add(menuItem);
                            }
                        } break;
                    case GameModeType.Survival:
                        {
                            foreach (var mission in missions)
                            {
                                if (menuItem != null)
                                    throw new InvalidOperationException($"Too many Survival's defined for {city.ToString()}!");

                                menuItem = BuildMenuItem(mission);
                                menu.DropDownItems.Add(menuItem);
                            }
                        } break;
                    }
                }
            }
        }
        
        private void GenerateExportedMissionObjects()
        {
            var widget = new InspectorWidget();
            var nodes = widget.Nodes;

            Cursor = Cursors.WaitCursor;
            
            var objectsData = MissionPackage.MissionData.Objects;
            var numObjects = objectsData.Objects.Count;
            
            for (int i = 0; i < numObjects; i++)
            {
                var obj = objectsData[i];

                nodes.Nodes.Add(new TreeNode() {
                    Text = String.Format("[{0}]: {1}", i, ExportedMissionObjects.GetObjectNameById(obj.TypeId)),
                    Tag = obj
                });
            }
            
            SafeAddControl(widget);

            Cursor = Cursors.Default;
        }

        private void GenerateMissionSummary()
        {
            SummaryEditor summaryEdit = new SummaryEditor();
            PropertyGrid inspector = summaryEdit.Inspector;

            inspector.SelectedObject = MissionPackage.MissionSummary;

            summaryEdit.Show();
        }
        
        private void GenerateWireCollection()
        {
            // Get widget ready
            Widget = new InspectorWidget();
            TreeView Nodes = Widget.Nodes;

            Cursor = Cursors.WaitCursor;

            List<NodeDefinition> nodeDefs = MissionPackage.MissionData.LogicData.Nodes.Definitions;
            var wireCollections = MissionPackage.MissionData.LogicData.WireCollection.WireCollections;
            int nWires = wireCollections.Count;
            
            for (int w = 0; w < nWires; w++)
            {
                bool hasUID = MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValue("UID") != -1;
                var wId = hasUID ? 1 : 0;
                var wires = wireCollections[w].Wires;
                var lNodeIdx = nodeDefs.FindIndex(0, (def) => (int)def.Properties[wId].Value == w);
                if (lNodeIdx < 0)
                    continue;
                if (lNodeIdx > nodeDefs.Count-1)
                    continue;

                var lNode = nodeDefs[lNodeIdx];
                var lNodeName = MissionPackage.MissionData.LogicData.StringCollection[lNode.StringId];

                var typ = isDriverPLMission == true ? NodeTypes.GetNodeTypeDPL(lNode.TypeId) : NodeTypes.GetNodeType(lNode.TypeId);
                var text = $"[{lNodeIdx}]: {typ}";

                if (!String.IsNullOrEmpty(lNodeName))
                    text = $"{text} \"{lNodeName}\"";
                
                var wireGroupNode = new TreeNode() {
                    BackColor = enlightenNodesBTN.Checked ? new NodeColor(200, 200, 200, 255) : lNode.Color,
                    Text = $"[{w}]: <{text}>",
                    Tag = lNode,
                };

                for (int n = 0; n < wires.Count; n++)
                {
                    var wire = wires[n];
                    
                    var node = nodeDefs[wire.NodeId];
                    var nodeName = MissionPackage.MissionData.LogicData.StringCollection[node.StringId];

                    var type = isDriverPLMission == true ? NodeTypes.GetNodeTypeDPL(lNode.TypeId) : NodeTypes.GetNodeType(lNode.TypeId);
                    var wireText = $"[{wire.NodeId}]: {type}";

                    if (!String.IsNullOrEmpty(lNodeName))
                        wireText = $"{wireText} \"{nodeName}\"";

                    var wireNode = new TreeNode() {
                        BackColor = enlightenNodesBTN.Checked ? new NodeColor(200,200,200,255) : node.Color,
                        Text = $"[{n}]: {wire.GetWireNodeType()}: <{wireText}>",
                        Tag = wire,
                    };
                    
                    wireGroupNode.Nodes.Add(wireNode);
                }

                Nodes.Nodes.Add(wireGroupNode);
            }

            Nodes.ExpandAll();

            SafeAddControl(Widget);
            
            Cursor = Cursors.Default;
        }
        
        private void AddNodeProperty(TreeNode node, NodeProperty prop)
        {
            var propName = MissionPackage.MissionData.LogicData.StringCollection[prop.StringId];
            var propValue = prop.ToString();
            
            if (prop is IntegerProperty)
            {
                var value = (int)prop.Value;
                
                switch (prop.TypeId)
                {
                case 7:
                        // actor is not negative, etc. etc.
                    if (value > -1) // != -1
                    {
                        var actor = MissionPackage.MissionData.LogicData.Actors[value];
                        var actorName = NodeTypes.GetActorType(actor.TypeId);
                        var actorText = MissionPackage.MissionData.LogicData.StringCollection[actor.StringId];

                        if (actorText != "Unknown" && actorText != "Unnamed")
                            actorName = String.Format("{0} \"{1}\"", actorName, MissionPackage.MissionData.LogicData.StringCollection[actor.StringId]);

                        propValue = String.Format("<[{0}]: {1}>", value, actorName);
                    }
                    if (value == -2)
                    {
                            propValue = "(Player)";
                    }
                    break;
                case 9:
                    propValue = String.Format("0x{0:X8}", value);
                    break;
                case 20:
                    if (value != -1)
                    {
                        if (MissionPackage.HasLocaleString(value))
                            propValue = String.Format("\"{0}\"", MissionPackage.GetLocaleString(value));
                    }
                    break;
                }
            }
            else
            {
                switch (prop.TypeId)
                {
                case 2:
                    propValue = String.Format("{0:0.0###}", (float)prop.Value);
                    break;
                case 15:
                    propValue = String.Format("0x{0:X8}", prop.Value);
                    break;
                case 3:
                case 8:
                    {
                        var strId = (short)prop.Value;

                        // wut
                        if (strId < 0)
                            strId &= 0xFF;

                        propValue = String.Format("\"{0}\"", MissionPackage.MissionData.LogicData.StringCollection[strId]);

                        if (prop.TypeId == 8)
                            propValue = String.Format("{{ {0}, {1} }}", propValue, ((TextFileItemProperty)prop).Index);
                    } break;
                }
            }
            
            var propNode = new TreeNode() {
                Text = (prop.TypeId != 19) ? $"{propName}: {propValue}" : propName,
                Tag = prop
            };

            // Add property node to main node
            node.Nodes.Add(propNode);
        }

        private void StyleNode(TreeNode node, NodeDefinition def)
        {
            string nodeType = isDriverPLMission ? NodeTypes.GetNodeTypeDPL(def.TypeId) : NodeTypes.GetNodeType(def.TypeId);

            var text = (def is ActorDefinition) ? NodeTypes.GetActorType(def.TypeId) : nodeType;
            var name = MissionPackage.MissionData.LogicData.StringCollection[def.StringId];

            if (name != "Unknown" && name != "Unnamed")
                text = String.Format("{0} \"{1}\"", text, name);

            node.Text = text;
        }
        
        private void StyleWireNode(TreeNode node, TreeNode defNode, WireNode wire)
        {
            node.Text = String.Format("{0}: <{1}>", wire.GetWireNodeType(), defNode.Text);
        }

        private TreeNode CreateNode(NodeDefinition def)
        {
            var node = new TreeNode() {
                BackColor = enlightenNodesBTN.Checked ? new NodeColor(200, 200, 200, 255) : def.Color,
                Tag = def
            };

            StyleNode(node, def);

            foreach (var prop in def.Properties)
                AddNodeProperty(node, prop);

            return node;
        }

        private void CreateNodes<T>(List<T> definitions, int selected = -1, List<MissionObject> objects = null, List<MissionInstance> instances = null, bool is3D = false)
            where T : NodeDefinition
        {
            // Build 3D viewport
            _3D.viewport _3dviewport = new _3D.viewport();
            if (definitions is List<ActorDefinition>)
               _3dviewport.sceneActors = definitions as List<ActorDefinition>;
            if (objects!=null & objects.Count!=0)
            {
                _3dviewport.sceneObjects = objects;
                _3dviewport.StartPosition[0] = MissionPackage.MissionSummary.StartPosition.X;
                _3dviewport.StartPosition[1] = MissionPackage.MissionSummary.StartPosition.Y;
            }
            if (instances != null)
            {
                _3dviewport.sceneInstances = instances;
                Debug.WriteLine($"non localised-string or no localised string availble {instances.Count}");
            }
            if (_3dviewport.sceneActors.Count!=0)
                _3dviewport.UpdateScene();

            // Get widget ready
            var inspector = is3D ? _3dviewport.Inspector : Activator.CreateInstance<InspectorWidget>();
            Widget = inspector;
            var nodes = inspector.Nodes;
            if (selected != -1)
                inspector.Nodes.SelectedNode = inspector.Nodes.Nodes[selected];

            inspector.Nodes.NodeMouseDoubleClick += (o, e) => {
                var tag = e.Node.Tag;

                if (tag is ActorProperty)
                {
                    var prop = tag as ActorProperty;

                    if (prop.Value < 0)
                        return;

                    if (e.Node.Nodes.Count == 0)
                    {
                        var actor = MissionPackage.MissionData.LogicData.Actors[prop.Value] as ActorDefinition;
                        
                        foreach (var actorProp in actor.Properties)
                            AddNodeProperty(e.Node, actorProp);

                        e.Node.Expand();
                    }
                    else
                    {
                        e.Node.Nodes.Clear();
                    }
                }
            };

            inspector.Nodes.KeyDown += (o, e) =>
            {
                if (e.KeyCode == Keys.D & e.Modifiers == Keys.Control)
                {
                    var node = inspector.Nodes.SelectedNode;
                    var tag = node.Tag;
                    bool duplicated = false;

                    if (tag is WireNode)
                    {
                        var wiren = tag as WireNode;

                        foreach(var wire in MissionPackage.MissionData.LogicData.WireCollection.WireCollections)
                        {
                            if (duplicated)
                                break; // breaks loop if already duplicated (task done)
                            foreach (WireNode wnode in wire.Wires)
                            {
                                // duplicates if this is the source
                                if (wnode.Equals(wiren)) {
                                    wire.Wires.Add(wiren);
                                    duplicated = true;

                                    var clone = (TreeNode)node.Clone();
                                    node.Parent.Nodes.Add(clone);
                                    inspector.Nodes.SelectedNode = clone;
                                    break;
                                } 
                            }
                        }
                    }
                    if (tag is ActorDefinition)
                    {
                        var nodedef = tag as ActorDefinition;

                        MissionPackage.MissionData.LogicData.Actors.Definitions.Add(nodedef);
                        //MissionPackage.MissionData.LogicData.Nodes.Definitions.RemoveAt(MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1); // hack
                        var clone = (TreeNode)node.Clone();
                        inspector.Nodes.Nodes.Add(clone);
                        inspector.Nodes.SelectedNode = clone;
                        duplicated = true;
                        return; // prevent the other one to be enabled
                    }
                    if (tag is NodeDefinition)
                    {
                        var nodedef = tag as NodeDefinition;
                        MissionPackage.MissionData.LogicData.Nodes.Definitions.Add(nodedef);
                        var clone = (TreeNode)node.Clone();
                        inspector.Nodes.Nodes.Add(clone);
                        inspector.Nodes.SelectedNode = clone;
                        duplicated = true;
                    }
                    //if (duplicated)
                        //MessageBox.Show("Definition duplicated with success!", "Duplicated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                if (e.KeyCode == Keys.G & e.Modifiers == Keys.Control)
                {
                    Form prompt = new Form()
                    {
                        Width = 500,
                        Height = 150,

                        FormBorderStyle = FormBorderStyle.FixedDialog,
                        StartPosition = FormStartPosition.CenterScreen,

                        Text = "Go to node"
                    };

                    Label textLabel = new Label()
                    {
                        Left = 50,
                        Top = 20,

                        Text = "Enter a node ID:"
                    };

                    TextBox textBox = new TextBox()
                    {
                        Left = 50,
                        Top = 50,

                        Width = 400,

                        SelectedText = "0"
                    };

                    Button confirmation = new Button()
                    {
                        Left = 250,
                        Top = 70,

                        Width = 100,
                        DialogResult = DialogResult.OK,

                        Text = "OK"
                    };

                    Button cancel = new Button()
                    {
                        Left = 350,
                        Top = 70,

                        Width = 100,
                        DialogResult = DialogResult.Cancel,

                        Text = "Cancel"
                    };

                    confirmation.Click += delegate { prompt.Close(); };

                    prompt.Controls.Add(textBox);
                    prompt.Controls.Add(confirmation);
                    prompt.Controls.Add(cancel);
                    prompt.Controls.Add(textLabel);
                    prompt.AcceptButton = confirmation;

                    DialogResult dialogResult = prompt.ShowDialog();
                    if (dialogResult == DialogResult.OK)
                    {
                        int id = 0;
                        try
                        {
                            id = Convert.ToInt32(textBox.Text);
                            if (id < 0)
                                id = 0;
                            if (id > inspector.Nodes.Nodes.Count-1)
                                id = inspector.Nodes.Nodes.Count-1;
                            inspector.Nodes.SelectedNode = inspector.Nodes.Nodes[id];
                        }
                        catch
                        {
                            MessageBox.Show("You didn't insert a valid node ID!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                    }
                }
                if (e.KeyCode == Keys.Delete)
                {
                    if (MessageBox.Show("Are you sure you want to delete this definition?", "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        var node = inspector.Nodes.SelectedNode;
                        var tag = node.Tag;

                        if (tag is WireNode)
                        {
                            var wiren = tag as WireNode;
                            bool del = false;

                            foreach (var wire in MissionPackage.MissionData.LogicData.WireCollection.WireCollections)
                            {
                                if (del)
                                    break; // breaks loop if already deleted (task done)
                                foreach (WireNode wnode in wire.Wires)
                                {
                                    // deleted if this is the source
                                    if (wnode.Equals(wiren))
                                    {
                                        wire.Wires.Remove(wiren);
                                        del = true;

                                        node.Parent.Nodes.Remove(node); // removes it in real time
                                        break;
                                    }
                                }
                            }
                        }
                        if (tag is ActorDefinition)
                        {
                            var def = tag as ActorDefinition;
                            MissionPackage.MissionData.LogicData.Actors.Definitions.Remove(def);
                            GenerateActors();
                            return;
                        }
                        if (tag is NodeDefinition)
                        {
                            var def = tag as NodeDefinition;
                            var id = node.Index; //MissionPackage.MissionData.LogicData.Nodes.Definitions.IndexOf(def);

                            bool del = false;
                            // prevent errors, logic node un-existent? remove it from the wire collection!
                            foreach (var wire in MissionPackage.MissionData.LogicData.WireCollection.WireCollections)
                            {
                                foreach (var wiredef in wire.Wires)
                                {
                                    if (del)
                                        break; // break look as task is done
                                    if (wiredef.NodeId == id) {
                                        wire.Wires.Remove(wiredef);
                                        del = true;
                                        break;
                                    }
                                }
                            }
                            // hack
                            foreach (var wire in MissionPackage.MissionData.LogicData.WireCollection.WireCollections)
                            {
                                foreach (var wiredef in wire.Wires)
                                {
                                    if (wiredef.NodeId > id)
                                        wiredef.NodeId -= 1;
                                }
                            }
                            MissionPackage.MissionData.LogicData.Nodes.Definitions.Remove(def);

                            useFlowgraph = false;
                            GenerateLogicNodes();
                            return;
                        }
                    }
                }
            };

            inspector.Nodes.NodeMouseClick += (o, e) => {
                if (e.Button == MouseButtons.Right)
                {
                    var node = e.Node;
                    var tag = e.Node.Tag;
                    
                    if (tag is NodeDefinition)
                    {
                        var def = tag as NodeDefinition;

                        Form prompt = new Form() {
                            Width   = 500,
                            Height  = 150,

                            FormBorderStyle = FormBorderStyle.FixedDialog,
                            StartPosition = FormStartPosition.CenterScreen,

                            Text = "Name"
                        };

                        Label textLabel = new Label() {
                            Left    = 50,
                            Top     = 20,

                            Text = "Enter a new name:"
                        };
                        
                        TextBox textBox = new TextBox() {
                            Left    = 50,
                            Top     = 50,

                            Width   = 400,

                            SelectedText = MissionPackage.MissionData.LogicData.StringCollection[def.StringId]
                        };

                        Button confirmation = new Button() {
                            Left    = 350,
                            Top     = 70,

                            Width   = 100,
                            DialogResult = DialogResult.OK,

                            Text = "Ok"
                        };

                        confirmation.Click += (sender, ee) => { prompt.Close(); };

                        prompt.Controls.Add(textBox);
                        prompt.Controls.Add(confirmation);
                        prompt.Controls.Add(textLabel);
                        prompt.AcceptButton = confirmation;

                        if (prompt.ShowDialog() == DialogResult.OK)
                        {
                            def.StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.AppendString(textBox.Text);

                            StyleNode(node, def);
                            node.Text = String.Format("[{0}]: {1}", nodes.Nodes.IndexOf(node), node.Text);
                        }
                    }
                }
            };

            Cursor = Cursors.WaitCursor;
            
            var count = definitions.Count;
            
            // Build main nodes
            for (int i = 0; i < count; i++)
            {
                var def = definitions[i];
                var node = CreateNode(def);

                node.Text = String.Format("[{0}]: {1}", i, node.Text);
                
                // Add main node to master node list
                nodes.Nodes.Add(node);
            }

            // Load wires (logic nodes only)
            for (int i = 0; i < count; i++)
            {
                // is driver PL mission or not?
                bool hasUID = MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValue("UID") != -1; //MissionPackage.MissionSummary.DPLBuffer[0x1B] != 0 | MissionPackage.MissionSummary.DPLBuffer[0x1A] != 0 | MissionPackage.MissionSummary.DPLBuffer[0x19] != 0;
                var magic = !hasUID ? 0 : 1;
                var def = definitions[i];

                if (def.Properties.Count < (!hasUID ? 1 : 2))
                    break;

                // Skip actor definitions
                if (def is ActorDefinition)
                    break;

                var prop = def.Properties[magic];
                var node = nodes.Nodes[i];

                if (node.Nodes.Count == 0)
                    continue;

                node = node.Nodes[magic]; // pWireCollection

                int wireId;
                try
                {
                    wireId = (int)prop.Value;
                }
                catch
                {
                    wireId = 0;
                    for (int nodeId=0;nodeId<definitions.Count;nodeId++)
                    {
                        var thisdef = definitions[nodeId];
                        if (thisdef is NodeDefinition)
                        {
                            if (thisdef.Properties[0].Value == prop.Value)
                                wireId = nodeId;
                        }
                    }
                }

                if (wireId < 0)
                    break;

                foreach (var wire in MissionPackage.MissionData.LogicData.WireCollection.WireCollections[wireId].Wires)
                {
                    var defNode = nodes.Nodes[wire.NodeId];

                    var wireNode = new TreeNode() {
                        BackColor = enlightenNodesBTN.Checked ? new NodeColor(200, 200, 200, 255) : defNode.BackColor,
                        Tag = wire
                    };

                    StyleWireNode(wireNode, defNode, wire);

                    node.Nodes.Add(wireNode);
                }
            }

            nodes.ExpandAll();

            if (is3D)
               SafeAddControl(_3dviewport);
            else
               SafeAddControl(inspector);
            
            Cursor = Cursors.Default;
        }

        private bool useFlowgraph = false;

        private void GenerateLogicNodes(int select=-1)
        {
            //if (useFlowgraph)
            if (useFlowgraphBTN.Checked)
            {
                CreateLogicNodesFlowgraph(MissionPackage.MissionData.LogicData.Nodes.Definitions);
            }
            else
            {
                CreateNodes(MissionPackage.MissionData.LogicData.Nodes.Definitions, select, (MissionPackage.MissionData.Objects.Objects != null) ? MissionPackage.MissionData.Objects.Objects : null);
            }

            // // Nest wires
            // for (int i = 0; i < nodeCount; i++)
            // {
            //     LogicDefinition def = MissionPackage.LogicNodeDefinitions[i];
            //     LogicProperty prop = def.Properties[0];
            // 
            //     int wireId = (int)prop.Value;
            // 
            //     for (int w = 0; w < MissionPackage.WireCollections[wireId].Count; w++)
            //     {
            //         WireCollectionEntry wire = MissionPackage.WireCollections[wireId].Entries[w];
            // 
            //         LogicNodes.Nodes[i].Nodes[0].Nodes[w] = LogicNodes.Nodes[wire.NodeId];
            //     }
            // }
        }

        private void GenerateActors(int selected = -1)
        {
            List<MissionInstance> GEBI = null;
            if (MissionPackage.MissionData.MissionInstances != null)
                GEBI = MissionPackage.MissionData.MissionInstances.Instances;
            CreateNodes(MissionPackage.MissionData.LogicData.Actors.Definitions,selected,MissionPackage.MissionData.Objects.Objects, GEBI, true);
        }

        public NodeWidget GenerateDefinition(FlowgraphWidget flowgraph, NodeDefinition def)
        {
            var LogicNodeTypes = isDriverPLMission == true ? NodeTypes.LogicNodeTypesDPL : NodeTypes.LogicNodeTypes;
            IDictionary<int, string> opcodes =
                (def.Properties[0].TypeId == 19)
                ? LogicNodeTypes
                : NodeTypes.ActorNodeTypes;

            string strName = MissionPackage.MissionData.LogicData.StringCollection[def.StringId];
            //string nodeName = (strName == "Unknown" || strName == "Unnamed") ? String.Empty : String.Format("\"{0}\"", strName);
            string opcodeName = opcodes.ContainsKey(def.TypeId) ? opcodes[def.TypeId] : def.TypeId.ToString();

            var node = new NodeWidget() {
                Flowgraph = flowgraph,
                //HeaderText = String.Format("{0}: {1} {2}", MissionPackage.MissionData.LogicData.Nodes.Definitions.IndexOf(def), opcodeName, nodeName),
                HeaderText = String.Format("{0}: {1}", MissionPackage.MissionData.LogicData.Nodes.Definitions.IndexOf(def), opcodeName),
                CommentText = (strName == "Unknown" || strName == "Unnamed") ? String.Empty : strName,
                Tag = def
            };

            var color = def.Color;

            node.Properties.BackColor = Color.FromArgb(255, enlightenNodesBTN.Checked ? 200 : color.R, 
                                                            enlightenNodesBTN.Checked ? 200 : color.G, 
                                                            enlightenNodesBTN.Checked ? 200 : color.B);

            if ((node.HeaderText.Length > 24) || (def.Properties.Count > 4))
                node.Width += 100;

            flowgraph.AddNode(node);
            
            for (int p = 0; p < def.Properties.Count; p++)
            {
                NodeProperty prop = def.Properties[p];

                string propName = MissionPackage.MissionData.LogicData.StringCollection[prop.StringId];

                // if (prop.Opcode == 20 && MissionPackage.HasLocale)
                // {
                //     int val = (int)prop.Value;
                //     string localeStr = (!MissionPackage.LocaleStrings.ContainsKey(val)) ? "<NULL>" : String.Format("\"{0}\"", MissionPackage.LocaleStrings[val]);
                // 
                //     propName = String.Format("{0} -> {1}", propName, localeStr);
                // }
                // if (prop.Opcode == 7 && ((int)prop.Value) != -1)
                // {
                //     int val = MissionPackage.ActorDefinitions[(int)prop.Value].Opcode;
                // 
                //     propName = String.Format("{0} -> {1}", propName, ((LogicData.Types.ActorDefinitionTypes.ContainsKey(val)) ? LogicData.Types.ActorDefinitionTypes[val] : prop.Value.ToString()));
                // }

                Label property = new Label() {
                    Text = String.Format("{0} = {1}", propName, prop.Value),
                    Font = new Font(Font.SystemFontName, 9F, FontStyle.Regular, GraphicsUnit.Pixel),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Tag = prop
                };

                node.Properties.Controls.Add(property);
            }
            
            return node;
        }

        public void CreateLogicNodesFlowgraph(IList<NodeDefinition> definition)
        {
            Widget = new InspectorWidget();

            SplitterPanel Panel1 = Widget.SplitPanel.Panel1;
            Panel1.Controls.Clear();
            Panel1.AutoScroll = true;

            FlowgraphWidget Flowgraph = new FlowgraphWidget() {
                Parent = Panel1
            };

            // Never forget.
            Flowgraph.GotFocus += (o, e) => { Flowgraph.Parent.Focus(); };
            Panel1.LostFocus += (o, e) => { Panel1.Focus(); };

            //Flowgraph.Dock = DockStyle.Fill;

            int nodeCount = definition.Count;

            var LogicNodeTypes = isDriverPLMission == true ? NodeTypes.LogicNodeTypesDPL : NodeTypes.LogicNodeTypes;
            IDictionary<int, string> opcodes =
                (definition[0].Properties[0].TypeId == 19)
                ? LogicNodeTypes
                : NodeTypes.ActorNodeTypes;
            
            var wireCollections = MissionPackage.MissionData.LogicData.WireCollection.WireCollections;
            
            var nodeLookup = new Dictionary<NodeDefinition, NodeWidget>();

            foreach (var def in definition.OrderBy((d) => d.Flags))
            {
                var node = GenerateDefinition(Flowgraph, def);

                nodeLookup.Add(def, node);
            }

            int x = 16, y = 16;

            foreach (var kv in nodeLookup)
            {
                var def = kv.Key;
                var node = kv.Value;

                node.Left = x;
                node.Top = y;
                
                y += (node.Height + 25);
            }

            var touchedNodes = new HashSet<NodeWidget>();

#if OLD_LOOKUP
            foreach (var kv in nodeLookup)
            {
                var def = kv.Key;
                var node = kv.Value;
#else
            foreach (var def in definition)
            {
                var node = nodeLookup[def];
#endif
                bool hasUID = MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValue("UID") != -1;

                var wireId = (int)def.Properties[hasUID ? 1 : 0].Value;
                var wireCol = wireCollections[wireId];

                var wireCollection = MissionPackage.MissionData.LogicData.WireCollection[wireId];

                var nWires = wireCollection.Wires.Count;

                if (node.Left > x)
                    node.Left += (node.Width + 35);
                if (node.Top > y)
                    node.Top += (node.Height + 25);

                x = node.Left + (node.Width + 35);
                y = node.Top - ((16 + 25) * nWires);

                if (y < 16)
                    y = 16;

                for (int w = 0; w < nWires; w++)
                {
                    var wire = wireCollection.Wires[w];
                    var wireDef = definition[wire.NodeId];

                    var wireNode = nodeLookup[wireDef];

                    wireNode.Left = x;
                    wireNode.Top = y;

                    Flowgraph.LinkNodes(node, wireNode, (WireNodeType)wire.WireType);

                    y += (node.Height + 25);

                    if (!touchedNodes.Contains(wireNode))
                        touchedNodes.Add(wireNode);
                }
            }
            
            Panel1.Controls.Add(Flowgraph);
            Panel1.Focus();

            SafeAddControl(Widget);
        }

        private void GenerateStringCollection()
        {
            DataGridWidget DataGridWidget = new DataGridWidget();
            DataGridView DataGrid = DataGridWidget.DataGridView;

            Cursor = Cursors.WaitCursor;

            for (int i = 0; i < MissionPackage.MissionData.LogicData.StringCollection.Count; i++)
            {
                DataGrid.Rows.Add(i, MissionPackage.MissionData.LogicData.StringCollection[i]);
            }

            SafeAddControl(DataGridWidget);

            Cursor = Cursors.Default;
        }

        private void GenerateActorSetTable()
        {
            /*
            DataGridWidget DataGridWidget = new DataGridWidget();
            DataGridView DataGrid = DataGridWidget.DataGridView;

            Cursor = Cursors.WaitCursor;

            for (int i = 0; i < MissionPackage.ActorSetTable.Count; i++)
            {
                DataGrid.Rows.Add(i, MissionPackage.ActorSetTable[i]);
            }
            
            SafeAddControl(DataGridWidget);
            
            Cursor = Cursors.Default;
            */
        }
        
        /// <summary> Safely adds a control to the form.</summary>
        private void SafeAddControl(Control control)
        {
            control.Parent = this;
            control.Dock = DockStyle.Fill;

            Content.SuspendLayout();

            foreach (Control c in Content.Controls)
                c.Dispose();

            Content.Controls.Add(control);
            Content.ResumeLayout(true);
        }
        
        private void ChunkButtonClick(Button button, int magic)
        {
            if (MissionPackage != null && MissionPackage.IsLoaded)
            {
                var cType = (ChunkType)magic;

                var inFlowgraph = useFlowgraph;

                useFlowgraph = false;

                switch (cType)
                {
                case ChunkType.ExportedMissionObjects:
                    GenerateExportedMissionObjects();
                    break;
                case ChunkType.LogicExportStringCollection:
                    GenerateStringCollection();
                    break;
                case ChunkType.LogicExportActorSetTable:
                    MessageBox.Show("Sorry, not implemented.", "Zartex", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //GenerateActorSetTable();
                    break;
                case ChunkType.LogicExportActorsChunk:
                    GenerateActors();
                    break;
                case ChunkType.LogicExportNodesChunk:
                    //if (!inFlowgraph)
                        //useFlowgraph = true;

                    GenerateLogicNodes();
                    break;
                case ChunkType.LogicExportWireCollections:
                    GenerateWireCollection();
                    break;
                case ChunkType.MissionSummary:
                        //MessageBox.Show("Sorry, not done yet.", "Zartex", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        GenerateMissionSummary();
                        break;
                default:
                    MessageBox.Show("Sorry, not implemented.", "Zartex", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                }

                Console.WriteLine($"Couldn't find anything for {cType.ToString()}...");
            }
            else
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            Widget = new InspectorWidget();

            SplitterPanel Panel1 = Widget.SplitPanel.Panel1;
            Panel1.AutoScroll = true;
            Panel1.BackColor = Color.White;
            
            Panel1.Controls.Clear();

            //Panel1.Paint += (o, p) => onPaintFlowgraph(o, p);
            //Panel1.Click += (o, pp) => {
            //    MouseEventArgs ee = (MouseEventArgs)pp;
            //
            //    using (Pen pen = new Pen(Color.Black, 2F))
            //    using (Graphics g = Panel1.CreateGraphics())
            //    {
            //        g.DrawEllipse(pen, ee.Location.X, ee.Location.Y, 10, 10);
            //    }
            //
            //    Console.WriteLine(((MouseEventArgs)pp).Location);
            //};
            //
            //Panel1.Controls.Add(new NodeWidget() {
            //    Parent = Panel1,
            //    Left = 3,
            //    Top = 6,
            //    Visible = true,
            //});
            //
            //Panel1.Controls.Add(new NodeWidget() {
            //    Parent = Panel1,
            //    Left = 3,
            //    Top = 203,
            //    Visible = true,
            //});
            //
            //Panel1.Controls.Add(new NodeWidget() {
            //    Parent = Panel1,
            //    Left = 327,
            //    Top = 91,
            //    Visible = true,
            //});

            FlowgraphWidget flowgraph = new FlowgraphWidget();

            NodeWidget n1 = new NodeWidget() {
                Flowgraph = flowgraph,
                Name = "Node1",
                HeaderText = "Logic Node #1",
                Left = 3,
                Top = 6,
            };
            
            NodeWidget n2 = new NodeWidget() {
                Flowgraph = flowgraph,
                Name = "Node2",
                HeaderText = "Logic Node #2",
                Left = 327,
                Top = 91,
            };
            
            NodeWidget n3 = new NodeWidget() {
                Flowgraph = flowgraph,
                Name = "Node3",
                HeaderText = "Logic Node #3",
                Left = 3,
                Top = 203,
            };
            
            NodeWidget n4 = new NodeWidget() {
                Flowgraph = flowgraph,
                Name = "Node4",
                HeaderText = "Logic Node #4",
                Left = 327,
                Top = 273,
            };

            n1.Properties.Controls.Add(new Label() {
                Parent = n1.Properties,
                Text = "pProperty = 5"
            });
            n1.Properties.Controls.Add(new Label() {
                Parent = n1.Properties,
                Text = "pProperty = 1"
            });
            n1.Properties.Controls.Add(new Label() {
                Parent = n1.Properties,
                Text = "pProperty = 3"
            });
            n1.Properties.Controls.Add(new Label() {
                Parent = n1.Properties,
                Text = "pProperty = 69"
            });
            n1.Properties.Controls.Add(new Label() {
                Parent = n1.Properties,
                Text = "pProperty = 1100548"
            });
            n1.Properties.Controls.Add(new Label() {
                Parent = n1.Properties,
                Text = "pProperty = 495544841"
            });
            n1.Properties.Controls.Add(new Label() {
                Parent = n1.Properties,
                Text = "pProperty = 0x185784"
            });
            n1.Properties.Controls.Add(new Label() {
                Parent = n1.Properties,
                Text = "pProperty = FFFFFFFF"
            });

            n1.NodeClicked += (o, m) => Widget.AfterNodeWidgetSelected(o, m);
            n2.NodeClicked += (o, m) => Widget.AfterNodeWidgetSelected(o, m);
            n3.NodeClicked += (o, m) => Widget.AfterNodeWidgetSelected(o, m);
            n4.NodeClicked += (o, m) => Widget.AfterNodeWidgetSelected(o, m);

            //flowgraph.Dock = DockStyle.Fill;

            //flowgraph.AddNodes(n1, n2, n3, n4);

            flowgraph.LinkNodes(n1, n2);
            flowgraph.LinkNodes(n3, n2);
            flowgraph.LinkNodes(n3, n4);

            Panel1.Controls.Add(flowgraph);

            SafeAddControl(Widget);
        }

        private void btnEMMS_Click(object sender, EventArgs e)
        {

        }

        private void fixNullProperitiesInActors()
        {
            int actorId = 0;
            foreach (var actor in MissionPackage.MissionData.LogicData.Actors.Definitions)
            {
                Debug.WriteLine($"Checking and repairing in actor {actorId} properities ...");
                int propId = 0;
                foreach(var prop in actor.Properties)
                {
                    if (prop.TypeId == 0)
                    {
                        Debug.WriteLine($"Removing property {propId} in actor {actorId}");
                        actor.Properties.Remove(prop);
                    }
                    propId++;
                }
                actorId++;
            }
        }

        private void fixNullTypesInObjects()
        {
            int objId = 0;
            foreach (var obj in MissionPackage.MissionData.Objects.Objects)
            {
                Debug.WriteLine($"Checking and repairing in object {objId} ...");
                    if (obj.TypeId == 0)
                    {
                        Debug.WriteLine($"Removing object {objId} as it's type is not recognized");
                        MissionPackage.MissionData.Objects.Objects.Remove(obj);
                    }
                objId++;
            }
            Debug.WriteLine("Done.");
        }

        private void stringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
                Form prompt = new Form()
            {
                Width = 500,
                Height = 150,

                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,

                Text = "Name"
            };

            Label textLabel = new Label()
            {
                Left = 50,
                Top = 20,

                Text = "Enter a new string:"
            };

            TextBox textBox = new TextBox()
            {
                Left = 50,
                Top = 50,

                Width = 400,

                SelectedText = "New String"
            };

            Button confirmation = new Button()
            {
                Left = 350,
                Top = 70,

                Width = 100,
                DialogResult = DialogResult.OK,

                Text = "Ok"
            };

            confirmation.Click += delegate { prompt.Close(); };

            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            if (prompt.ShowDialog() == DialogResult.OK)
            {
                MissionPackage.MissionData.LogicData.StringCollection.AppendString(textBox.Text); // add string

                GenerateStringCollection(); // update list
            }
        }

        private DialogResult logicAddToWireCollectionByUser(int nodeId)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return DialogResult.Abort;
            }
            NodeDefinition node = MissionPackage.MissionData.LogicData.Nodes.Definitions[nodeId];
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,

                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,

                Text = "Enter a wire collection to add your node"
            };

            Label textLabel = new Label()
            {
                Left = 50,
                Top = 20,

                Text = "Enter a wire:"
            };

            TextBox textBox = new TextBox()
            {
                Left = 50,
                Top = 50,

                Width = 400,

                SelectedText = MissionPackage.MissionData.LogicData.Nodes.Definitions[0].Properties[0].Value.ToString() //"0"
            };

            Button confirmation = new Button()
            {
                Left = 250,
                Top = 70,

                Width = 100,
                DialogResult = DialogResult.OK,

                Text = "OK"
            };

            Button cancel = new Button()
            {
                Left = 350,
                Top = 70,

                Width = 100,
                DialogResult = DialogResult.Cancel,

                Text = "Continue"
            };

            confirmation.Click += delegate { prompt.Close(); };

            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            DialogResult dialogResult = prompt.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                int id = 0;
                try
                {
                    id = Convert.ToInt32(textBox.Text);
                    WireNode wire = new WireNode();
                    wire.NodeId = (short)nodeId;
                    wire.OpCode = node.TypeId;
                    wire.WireType = 1;
                    MissionPackage.MissionData.LogicData.WireCollection.WireCollections[id].Wires.Add(wire);
                    //node.Properties[0].Value = id;
                }
                catch
                {
                    MessageBox.Show("You didn't insert a valid wire!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return DialogResult.Retry;
                }

                GenerateWireCollection(); // update list
            }
            return dialogResult;
        }

        private DialogResult askToAddSoundBank()
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return DialogResult.Abort;
            }
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,

                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,

                Text = "Enter a sound bank ID to load"
            };

            Label textLabel = new Label()
            {
                Left = 50,
                Top = 20,

                Text = "Enter a ID:"
            };

            TextBox textBox = new TextBox()
            {
                Left = 50,
                Top = 50,

                Width = 400,

                SelectedText = MissionPackage.MissionData.LogicData.Nodes.Definitions[0].Properties[0].Value.ToString() //"0"
            };

            Button confirmation = new Button()
            {
                Left = 250,
                Top = 70,

                Width = 100,
                DialogResult = DialogResult.OK,

                Text = "OK"
            };

            Button cancel = new Button()
            {
                Left = 350,
                Top = 70,

                Width = 100,
                DialogResult = DialogResult.Cancel,

                Text = "Continue"
            };

            confirmation.Click += delegate { prompt.Close(); };

            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            DialogResult dialogResult = prompt.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                try
                {
                    MissionPackage.MissionData.LogicData.SoundBankTable.AppendBank(Convert.ToInt32(textBox.Text));
                    MessageBox.Show("Added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show("You didn't insert a valid sound bank ID!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return DialogResult.Retry;
                }
            }
            return dialogResult;
        }

        private DialogResult wireNodeAddToWireCollectionByUser(int nodeId=0)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return DialogResult.Abort;
            }
            NodeDefinition node = MissionPackage.MissionData.LogicData.Nodes.Definitions[nodeId];
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,

                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,

                Text = "Enter a wire collection to add your wire node"
            };

            Label textLabel = new Label()
            {
                Left = 50,
                Top = 20,

                Text = "Enter wire:"
            };

            TextBox textBox = new TextBox()
            {
                Left = 50,
                Top = 50,

                Width = 400,

                SelectedText = MissionPackage.MissionData.LogicData.Nodes.Definitions[0].Properties[0].Value.ToString() //"0"
            };

            Button confirmation = new Button()
            {
                Left = 250,
                Top = 70,

                Width = 100,
                DialogResult = DialogResult.OK,

                Text = "OK"
            };

            Button cancel = new Button()
            {
                Left = 350,
                Top = 70,

                Width = 100,
                DialogResult = DialogResult.Cancel,

                Text = "Continue"
            };

            confirmation.Click += delegate { prompt.Close(); };

            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            DialogResult dialogResult = prompt.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                int id = 0;
                try
                {
                    id = Convert.ToInt32(textBox.Text);
                    WireNode wire = new WireNode();
                    wire.NodeId = (short)nodeId;
                    wire.OpCode = node.TypeId;
                    wire.WireType = 1;
                    MissionPackage.MissionData.LogicData.WireCollection.WireCollections[id].Wires.Add(wire);
                    //node.Properties[0].Value = id;
                }
                catch
                {
                    MessageBox.Show("You didn't insert a valid wire!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return DialogResult.Retry;
                }

                GenerateWireCollection(); // update list
            }
            return dialogResult;
        }

        // string ID returned
        private int createNewString(string str)
        {
            if (MissionPackage == null)
            {
                return 0;
            }
            return (int)MissionPackage.MissionData.LogicData.StringCollection.AppendString(str);
        }

        public NodeDefinition addNewLogicNode(byte typeId, short stringId = 0)
        {
            if (MissionPackage != null & stringId == 0) { stringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValue("Unknown"); }
            NodeDefinition node = new NodeDefinition();
            node.TypeId = typeId;
            node.Flags = 0x0;
            Random randomR = new Random(490348);
            Random randomG = new Random(232456);
            Random randomB = new Random(121029);
            switch (node.TypeId) {
                default:
                case 1:
                    MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Add(new WireCollection(0));
                    node.Properties = new List<NodeProperty>
                    {
                         new WireCollectionProperty(MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Count-1),
                    };
                    node.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection");
                    break;
                case 3:
                    MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Add(new WireCollection(0));
                    node.Properties = new List<NodeProperty>
                    {
                         new WireCollectionProperty(MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Count-1), // pWireCollection
                         new FloatProperty(1f),                                                                                   // pInterval
                         new FlagsProperty(1)                                                                                     // pFlags
                    };
                    node.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection");
                    node.Properties[1].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pInterval");
                    node.Properties[2].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags");
                    break;
                // mission failed / mission success
                case 6:
                    MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Add(new WireCollection(0));
                    node.Properties = new List<NodeProperty>
                    {
                         new WireCollectionProperty(MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Count-1), // pWireCollection
                         new FloatProperty(1f),                                                                                   // pFailedDelay
                         new LocalisedStringProperty(0),                                                                          // pMessage
                         new FlagsProperty(1)                                                                                     // pFlags
                    };
                    node.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection");
                    node.Properties[1].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFailedDelay");
                    node.Properties[2].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMessage");
                    node.Properties[3].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags");
                    break;
                case 5:
                    MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Add(new WireCollection(0));
                    node.Properties = new List<NodeProperty>
                    {
                         new WireCollectionProperty(MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Count-1), // pWireCollection
                         new BooleanProperty(true)                                                                                // pNoDelay                                                                        // pFlags
                    };
                    node.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection");
                    node.Properties[1].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pNoDelay");
                    break;
                case 13:
                    MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Add(new WireCollection(0));
                    node.Properties = new List<NodeProperty>
                    {
                         new WireCollectionProperty(MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Count-1),      // pWireCollection
                         new ActorProperty(0),               // pSwitch
                         new FlagsProperty(2)                // pFlags
                    };
                    node.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection");
                    node.Properties[1].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pSwitch");
                    node.Properties[2].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags");
                    break;
                case 14:
                    MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Add(new WireCollection(0));
                    node.Properties = new List<NodeProperty>
                    {
                         new WireCollectionProperty(MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Count-1), // pWireCollection
                         new ActorProperty(0),    // pTarget        
                         new EnumProperty(2),     // pCameraType
                         new FloatProperty(1.0f), // pDuration
                         new FloatProperty(0f),   // pBlendTime
                         new FloatProperty(1f),   // pThrillCamZoom
                         new FloatProperty(1f), // pThrillCamSpeed
                         new FloatProperty(0f),   // pThrillCamBlur
                         new FlagsProperty(0)     // pFlags
                    };
                    node.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection");
                    node.Properties[1].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pTarget");
                    node.Properties[2].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCameraType");
                    node.Properties[3].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pDuration");
                    node.Properties[4].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pBlendTime");
                    node.Properties[5].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pThrillCamZoom");
                    node.Properties[6].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pThrillCamSpeed");
                    node.Properties[7].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pThrillCamBlur");
                    node.Properties[8].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags");
                    break;
                case 19:
                    MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Add(new WireCollection(0));
                    node.Properties = new List<NodeProperty>
                    {
                         new WireCollectionProperty(MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Count-1),      // pWireCollection
                         new ActorProperty(0),               // pCharacter
                         new ActorProperty(1),               // pObject
                         new EnumProperty(1),                // pWatchover
                         new FloatProperty(0),               // pValue
                         new FlagsProperty(1)                // pFlags
                    };
                    node.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection");
                    node.Properties[1].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCharacter");
                    node.Properties[2].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pObject");
                    node.Properties[3].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWatchover");
                    node.Properties[4].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pValue");
                    node.Properties[5].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags");
                    break;
                case 24:
                    MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Add(new WireCollection(0));
                    node.Properties = new List<NodeProperty>
                    {
                         new WireCollectionProperty(MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Count-1),      // pWireCollection
                         new LocalisedStringProperty(0),     // pMessage
                         new IntegerProperty(5),             // pTimer
                         new EnumProperty(100),              // pPriority
                         new FlagsProperty(65536)            // pFlags
                    };
                    node.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection");
                    node.Properties[1].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMessage");
                    node.Properties[2].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pTimer");
                    node.Properties[3].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pPriority");
                    node.Properties[4].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags");
                    break;
                case 25:
                    MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Add(new WireCollection(0));
                    node.Properties = new List<NodeProperty>
                    {
                         new WireCollectionProperty(MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Count-1),      // pWireCollection
                         new StringProperty(0)               // pFMVFile
                    };
                    node.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection");
                    node.Properties[1].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFMVFile");
                    break;
                case 30:
                    MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Add(new WireCollection(0));
                    node.Properties = new List<NodeProperty>
                    {
                         new WireCollectionProperty(MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Count-1),      // pWireCollection
                         new ActorProperty(0),               // pActor1
                         new ActorProperty(1),               // pActor2
                         new FloatProperty(5),               // pThreshold
                         new EnumProperty(100),              // pCheckType
                         new FlagsProperty(1)                // pFlags
                    };
                    node.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection");
                    node.Properties[1].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pActor1");
                    node.Properties[2].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pActor2");
                    node.Properties[3].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pThreshold");
                    node.Properties[4].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCheckType");
                    node.Properties[5].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags");
                    break;
                case 100:
                    MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Add(new WireCollection(0));
                    node.Properties = new List<NodeProperty>
                    {
                         new WireCollectionProperty(MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Count-1),      // pWireCollection
                         new BooleanProperty(false),               // pDisableCivs
                         new BooleanProperty(false)                // pSubtitles
                    };
                    node.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection");
                    node.Properties[1].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pDisableCivs");
                    node.Properties[2].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pSubtitles");
                    break;
                case 101:
                    MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Add(new WireCollection(0));
                    node.Properties = new List<NodeProperty>
                    {   
                         new WireCollectionProperty(MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Count-1),      // pWireCollection
                         new ActorProperty(0),               // pActor
                         new EnumProperty(1),                // pActivity
                         new FlagsProperty(0)                // pFlags
                    };
                    node.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection");
                    node.Properties[1].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pActor");
                    node.Properties[2].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pActivity");
                    node.Properties[3].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags");
                    break;
                case 102:
                    MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Add(new WireCollection(0));
                    node.Properties = new List<NodeProperty>
                    {
                         new WireCollectionProperty(MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Count-1),      // pWireCollection
                         new EnumProperty(5),                // pChasers
                         new EnumProperty(1),                // pType
                         new EnumProperty(5),                // pAggression
                         new EnumProperty(2),                // pArmour
                         new EnumProperty(1),                // pGoonWeapon1
                         new EnumProperty(0),                // pGoonWeapon2
                         new EnumProperty(0),                // pGoonWeapon3
                         new FlagsProperty(524290)           // pFlags
                    };
                    node.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection");
                    node.Properties[1].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pChasers");
                    node.Properties[2].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pType");
                    node.Properties[3].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAggression");
                    node.Properties[4].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pArmour");
                    node.Properties[5].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pGoonWeapon1");
                    node.Properties[6].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pGoonWeapon2");
                    node.Properties[7].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pGoonWeapon3");
                    node.Properties[8].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags");
                    break;
                case 104:
                    MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Add(new WireCollection(0));
                    node.Properties = new List<NodeProperty>
                    {
                         new WireCollectionProperty(MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Count-1),      // pWireCollection
                         new EnumProperty(4),             // pDensity
                         new EnumProperty(4),             // pParkedCarDensity
                         new EnumProperty(-3),            // pAttractorParkedCarDensity
                         new FlagsProperty(0),            // pFlags
                         new FloatProperty(120f),         // pPingInRadius
                         new FloatProperty(125f)          // pPingOutRadius
                    };
                    node.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection");
                    node.Properties[1].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pDensity");
                    node.Properties[2].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pParkedCarDensity");
                    node.Properties[3].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAttractorParkedCarDensity");
                    node.Properties[4].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags");
                    node.Properties[5].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pPingInRadius");
                    node.Properties[6].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pPingOutRadius");
                    break;
                case 118:
                    MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Add(new WireCollection(0));
                    node.Properties = new List<NodeProperty>
                    {
                         new WireCollectionProperty(MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Count-1),      // pWireCollection
                         new EnumProperty(0),                // pMusicType
                         new FlagsProperty(1)                // pFlags
                    };
                    node.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection");
                    node.Properties[1].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMusicType");
                    node.Properties[2].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags");
                    break;
                case 122:
                    MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Add(new WireCollection(0));
                    node.Properties = new List<NodeProperty>
                    {
                         new WireCollectionProperty(MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Count-1),      // pWireCollection
                         new ActorProperty(0),               // pVehicle
                         new FloatProperty(22.36f),          // pMinimumSpeed
                         new IntegerProperty(5),             // pGraceTime
                         new AudioProperty(45,0)             // pWarningAudio
                    };
                    node.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection");
                    node.Properties[1].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pVehicle");
                    node.Properties[2].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMinimumSpeed");
                    node.Properties[3].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pGraceTime");
                    node.Properties[4].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWarningAudio");
                    break;
                case 123:
                    MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Add(new WireCollection(0));
                    node.Properties = new List<NodeProperty>
                    {
                         new WireCollectionProperty(MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Count-1),      // pWireCollection
                         new IntegerProperty(8),            // pNumberOfCars
                         new FloatProperty(17),             // pTopSpeed
                         new FloatProperty(1.5f),           // pAcceleration
                         new FloatProperty(0.015f),         // pStartingPos
                         new EnumProperty(1),               // pTrackSegment
                         new EnumProperty(2),               // pDirection
                         new EnumProperty(1),               // pMissionType
                         new BooleanProperty(false),        // pShowIcon
                         new EnumProperty(2),               // pWeapon1
                         new EnumProperty(5),               // pWeapon2
                         new FloatProperty(1),              // pVulnerability
                         new IntegerProperty(3),            // pNumPropModels
                         new IntegerProperty(0),            // pPropsPerCar
                         new FloatProperty(100),            // pStopDistance
                         new FloatProperty(100),            // pFailDistance
                         new ActorProperty(-1),             // pZone
                         new IntegerProperty(0),            // pPersonalityIndex
                         new LocalisedStringProperty(-1)    // pMessage
                    };
                    node.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection");
                    node.Properties[1].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pNumberOfCars");
                    node.Properties[2].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pTopSpeed");
                    node.Properties[3].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAcceleration");
                    node.Properties[4].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pStartingPos");
                    node.Properties[5].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pTrackSegment");
                    node.Properties[6].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pDirection");
                    node.Properties[7].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMissionType");
                    node.Properties[8].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pShowIcon");
                    node.Properties[9].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWeapon1");
                    node.Properties[10].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWeapon2");
                    node.Properties[11].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pVulnerability");
                    node.Properties[12].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pNumPropModels");
                    node.Properties[13].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pPropsPerCar");
                    node.Properties[14].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pStopDistance");
                    node.Properties[15].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFailDistance");
                    node.Properties[16].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pZone");
                    node.Properties[17].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pPersonalityIndex");
                    node.Properties[18].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMessage");
                    break;
                case 124:
                    MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Add(new WireCollection(0));
                    node.Properties = new List<NodeProperty>
                    {
                         new WireCollectionProperty(MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Count-1),      // pWireCollection
                         new FloatProperty(0),               // pFadeToColourTime
                         new FloatProperty(1f),              // pFadeFromColourTime
                         new FloatProperty(1f),              // pDuration
                         new Float3Property(new Vector4(0,0,0,0))            // pColour
                    };
                    node.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection");
                    node.Properties[1].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFadeToColourTime");
                    node.Properties[2].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFadeFromColourTime");
                    node.Properties[3].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pDuration");
                    node.Properties[4].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pColour");
                    break;
                case 131:
                    MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Add(new WireCollection(0));
                    node.Properties = new List<NodeProperty>
                    {
                         new WireCollectionProperty(MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Count-1),      // pWireCollection
                         new FlagsProperty(0)            // pFlags
                    };
                    node.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection");
                    node.Properties[1].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags");
                    break;
            }
            node.Color = new NodeColor((byte)randomR.Next(0,128), (byte)randomG.Next(0,255), (byte)randomB.Next(0,200), 255);
            node.StringId = stringId;
            MissionPackage.MissionData.LogicData.Nodes.Definitions.Add(node);
            return node;
        }

        public ActorDefinition addNewActor(byte typeId,bool askLogicNode=false,bool createObject=true,short stringId = 0)
        {
            if (MissionPackage!=null & stringId == 0) { stringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValue("Unknown"); }
            ActorDefinition actor = new ActorDefinition();            
            actor.TypeId = typeId;
            actor.Flags = 0x0;
            Random randomR = new Random(490348);
            Random randomG = new Random(232456);
            Random randomB = new Random(121029);
            if (createObject)
            {
                actor.ObjectId = MissionPackage.MissionData.Objects.Objects.Count; // as a new object was created, it's id the count of the objects now
            }
            switch (actor.TypeId) {
                case 2:
                    actor.Properties = new List<NodeProperty>
                    {
                        new IntegerProperty(1),          // pRole
                        new TextFileItemProperty(0,-1),  // pPersonality
                        new IntegerProperty(-1),         // pPersonalityIndex
                        new FloatProperty(1.0f),         // pHealth
                        new FloatProperty(0.0f),         // pFelony
                        new EnumProperty(0),             // pWeapon
                        new FloatProperty(1.0f),         // pVulnerability
                        new FlagsProperty(131073)        // pFlags 
                    };
                    actor.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pRole");
                    actor.Properties[1].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pPersonality");
                    actor.Properties[2].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pPersonalityIndex");
                    actor.Properties[3].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pHealth");
                    actor.Properties[4].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFelony");
                    actor.Properties[5].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWeapon");
                    actor.Properties[6].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pVulnerability");
                    actor.Properties[7].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags");
                    if (!createObject) { break; }
                    CharacterObject characterObject = new CharacterObject();
                    characterObject.CreationData = new byte[]
                    {
                        0,0,1,0x0D,
                        // reserved for position here
                        0,0,0,0,      0,0,0,0,     0,0,0,0,    0,0,0x80,0x3F,
                        // extra stuff
                        0,0,0,0, // ignore this as it's zero
                        // hmm..
                        0xBB,0x66,0x7F,0xBF // <-- character UID?
                    };
                    characterObject.UID = -12275; // 0x0DD001C0
                    //characterObject.HasCreationData = true;
                    MissionPackage.MissionData.Objects.Objects.Add(characterObject);
                    break;
                case 3:
                    actor.Properties = new List<NodeProperty>
                    {
                        new FloatProperty(1.0f),         // pWeight
                        new FloatProperty(1.0f),         // pSoftness
                        new FloatProperty(1.0f),         // pIFragility
                        new FloatProperty(1.0f),         // pDemoOnlySoftness
                        new IntegerProperty(0),          // pTintValue
                        new FlagsProperty(302186497)     // pFlags 
                    };
                    actor.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWeight");
                    actor.Properties[1].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pSoftness");
                    actor.Properties[2].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pIFragility");
                    actor.Properties[3].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pDemoOnlyExplosionSoftness");
                    actor.Properties[4].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pTintValue");
                    actor.Properties[5].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags");
                    if (!createObject) { break; }
                    VehicleObject vehicleObject = new VehicleObject();
                    // creation data is important on saving!
                    // oh crud, I picked the biggest one....
                    vehicleObject.CreationData = new byte[]
                    {
                        4,0x0C,0x28,0x0, // UID maybe?
                        0x0C,0x0,0x4C, // magic
                        0x0,0x0,0x0,0x0,0x0, // zeros......
                        0x01,0x1C,0,0, // part 2 magic
                        // direction of which the vehicle is facing to
                        0,0,0x80,0x3F,   0,0,0,0,   0,0,0,0,   0,0,0x80,0x3F,
                        // pack of zeros (I don't know what they mean)
                        0,0,0,0,0,0,0,0,
                        1, 0x24, 0,0,
                        // vehicle position (X,Y,Z,W=1) here but we're doing zeros this time.
                        0,0,0,0,    0,0,0,0,     0,0,0,0,    0,0,0x80,0x3F,
                        // more unknown floats stuff, guess what? ZEROS!
                        0,0,0,0,    0,0,0,0,     0,0,0,0,   0,0,0,0,
                        // unknown byte gang
                        5,0x20,0,0,
                        // more zeros.... and 1.0f again.
                        0,0,0,0,    0,0,0,0,     0,0,0,0,   0,0,0x80,0x3F,
                        // AND..... the end!
                        0,0,0x80,0x3F,  0,0,0,0,    0,0,0,0
                    };
                    //vehicleObject.HasCreationData = true;
                    MissionPackage.MissionData.Objects.Objects.Add(vehicleObject);
                    actor.Flags = (short)-21289; // default flags (0xACD7)
                    break;
                case 6:
                    actor.Properties = new List<NodeProperty>
                    {
                        new ActorProperty(-1),           // pCharacter
                        new FloatProperty(1.0f),         // pSpeed
                        new FloatProperty(1.0f),         // pBurnoutTime
                        new IntegerProperty(1),          // pDesiredTransport
                        new FlagsProperty(1)             // pFlags 
                    };
                    actor.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCharacter");
                    actor.Properties[1].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pSpeed");
                    actor.Properties[2].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pBurnoutTime");
                    actor.Properties[3].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pDesiredTransport");
                    actor.Properties[4].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags");
                    if (!createObject) { break; }
                    PathObject pathObject = new PathObject();
                    pathObject.CreationData = new byte[]
                    {
                        1, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                        0,0,0,0,      0,0,0,0,     0,0,0,0,     0,0,0x3F,0x80
                    };
                    MissionPackage.MissionData.Objects.Objects.Add(pathObject);
                    break;
                case 5:
                    actor.Properties = new List<NodeProperty>
                    {
                        new ActorProperty(-1),           // pAttachTo
                        new IntegerProperty(1),          // pHeight
                        new IntegerProperty(0),          // pMaxAdpativeHeight
                        new FlagsProperty(131072)        // pFlags 
                    };
                    actor.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAttachTo");
                    actor.Properties[1].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pHeight");
                    actor.Properties[2].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMaxAdpativeHeight");
                    actor.Properties[3].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags");
                    if (!createObject) { break; }
                    ObjectiveIconObject ObjectiveIcon = new ObjectiveIconObject();
                    // no creation data available for ObjectiveIcon object, only Position and Rotation fills up to 24 bytes
                    MissionPackage.MissionData.Objects.Objects.Add(ObjectiveIcon);
                    break;
                case 7:
                    actor.Properties = new List<NodeProperty>
                    {
                        new ActorProperty(0),            // pCharacter
                        new ActorProperty(-1),           // pAttachTo
                        new FloatProperty(1f),           // pWalkSpeed
                        new FloatProperty(5f),           // pVehicleSpeed
                        new EnumProperty(0),             // pDesiredTransport
                        new FlagsProperty(0)             // pFlags 
                    };
                    actor.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCharacter");
                    actor.Properties[1].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAttachTo");
                    actor.Properties[2].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWalkSpeed");
                    actor.Properties[3].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pVehicleSpeed");
                    actor.Properties[4].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pDesiredTransport");
                    actor.Properties[5].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags");
                    if (!createObject) { break; }
                    VolumeObject target = new VolumeObject();
                    // no creation data available for VolumeObject object, only Position and Reserved fills up to about 13 bytes
                    MissionPackage.MissionData.Objects.Objects.Add(target);
                    break;
                case 9:
                    actor.Properties = new List<NodeProperty>
                    {
                        new ActorProperty(-1),           // pLookAt
                        new ActorProperty(-1),           // pAttachTo
                        new FloatProperty(1),            // pCameraZoom
                        new FloatProperty(1),            // pSpeed
                        new FloatProperty(0),            // pBlur
                    };
                    actor.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pLookAt");
                    actor.Properties[1].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAttachTo");
                    actor.Properties[2].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCameraZoom");
                    actor.Properties[3].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pSpeed");
                    actor.Properties[4].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pBlur");
                    if (!createObject) { break; }
                    CameraObject camera = new CameraObject();
                    // no creation data available for VolumeObject object, only Position and Reserved fills up to about 13 bytes
                    MissionPackage.MissionData.Objects.Objects.Add(camera);
                    break;
                case 101:
                    actor.Properties = new List<NodeProperty>
                    {
                        new EnumProperty(10),             // pType
                    };
                    actor.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pType");
                    if (!createObject) { break; }
                    SwitchObject switchO = new SwitchObject();
                    switchO.UID = -1070959248; //0xC02A7570
                    // no creation data available for SwitchObject object, only Position and Reserved fills up to about 13 bytes
                    MissionPackage.MissionData.Objects.Objects.Add(switchO);
                    break;
            }
            actor.Color = new NodeColor((byte)randomR.Next(0,128), (byte)randomG.Next(0,255), (byte)randomB.Next(0,200), 255);
            actor.StringId = stringId;
            MissionPackage.MissionData.LogicData.Actors.Definitions.Add(actor);
            if (askLogicNode)
            {
                AskForAddActorToLogicNodes(MissionPackage.MissionData.LogicData.Actors.Definitions.Count-1);
            }
            return actor;
        }

        private void AskForAddActorToLogicNodes(int actorId)
        {
            if (MessageBox.Show("Would you like to add this actor to ActorCreation list in logic start?", "Add to Start", MessageBoxButtons.YesNo, MessageBoxIcon.Question)==DialogResult.Yes)
            {
                int logicStartWireId = (int)MissionPackage.MissionData.LogicData.Nodes.Definitions[0].Properties[0].Value;
                NodeDefinition logicNode = new NodeDefinition();
                logicNode.Color = new NodeColor(255, 0, 255, 255); // purple
                logicNode.Properties = new List<NodeProperty>
                {
                    new WireCollectionProperty(0),      // pWireCollection
                    new ActorProperty(actorId),         // pActor
                    new EnumProperty(1),                // pActivity
                    new FlagsProperty(0)                // pFlags
                };
                logicNode.Properties[0].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection");
                logicNode.Properties[1].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pActor");
                logicNode.Properties[2].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pActivity");
                logicNode.Properties[3].StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags");
                logicNode.TypeId = 101; // ActorCreation
                logicNode.StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); // set no name to it
                MissionPackage.MissionData.LogicData.Nodes.Definitions.Add(logicNode);
                WireNode wireNode = new WireNode();
                wireNode.OpCode = 101;
                wireNode.WireType = 1; // on success enable
                wireNode.NodeId = (short)(MissionPackage.MissionData.LogicData.Nodes.Definitions.Count-1); // set logicNode as it
                MissionPackage.MissionData.LogicData.WireCollection.WireCollections[logicStartWireId].Wires.Add(wireNode);
                MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Add(new WireCollection(0));
                logicNode.Properties[0].Value = MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Count - 1; // add an empty wire collection to this node to not call any more nodes
                GenerateLogicNodes();
            } 
        }

        private void mnFile_New_Click(object sender, EventArgs e)
        {
            if (MissionPackage != null)
            {
                DialogResult result = MessageBox.Show("Are you sure? all unsaved progress will be lost!!", "Zartex", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Cancel | result == DialogResult.Abort)
                {
                    return;
                }
            }
            // free this mission package
            /*
            if (MissionPackage != null) { 
              MissionPackage.Dispose();
              MissionPackage = null;
            }
            */

            Filename = Path.GetFullPath("untitled.mpc");

            //MissionPackage = new MissionScriptFile(); // does new mission package file

            //MissionPackage.MissionData = new ExportedMission(); // instantiate new exported mission
            //MissionPackage.MissionData.LogicData = new LogicExportData(); // set new logic export data

            // and you know... more "new" stuff
            //MissionPackage.MissionData.LogicData.StringCollection = new StringCollectionData();
            MissionPackage.MissionData.LogicData.StringCollection.Strings = new List<string>();

            MissionPackage.MissionData.LogicData.StringCollection.AppendString(""); // empty string; as "pPersonality" uses an empty string for normal character (like the player)
            MissionPackage.MissionData.LogicData.StringCollection.AppendString("Unknown"); // default string

            //MissionPackage.MissionData.Objects = new ExportedMissionObjects();
            MissionPackage.MissionData.Objects.Objects = new List<MissionObject>();

            // the most important for our nodes!

            //MissionPackage.MissionData.LogicData.WireCollection = new WireCollectionData();
            MissionPackage.MissionData.LogicData.WireCollection.WireCollections = new List<WireCollection>();

            short strId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Player");

            // prepare actors
            //MissionPackage.MissionData.LogicData.Actors = new LogicDataCollection<ActorDefinition>();
            MissionPackage.MissionData.LogicData.Actors.Definitions = new List<ActorDefinition>();

            // the first actor

            ActorDefinition player = addNewActor(2,false,true, strId); // main character : Player
            player.Color = new NodeColor(255, 0, 0, 255);

            // prepare nodes
            //MissionPackage.MissionData.LogicData.Nodes = new LogicDataCollection<NodeDefinition>();
            MissionPackage.MissionData.LogicData.Nodes.Definitions = new List<NodeDefinition>();

            // the first node

            NodeDefinition logicStart = addNewLogicNode(1);

            MissionPackage.FileName = Filename;

            // I made mission summary the last...
            MissionPackage.MissionSummary = new MissionSummaryData(); 
            MissionPackage.MissionSummary.CityType = MissionCityType.Miami_Day;
            MissionPackage.MissionSummary.MissionId = 1;

            MissionPackage.MissionSummary.HasDensityData = true;
            MissionPackage.MissionSummary.DensityOverride = true;
            MissionPackage.MissionSummary.PingInRadius = 150;
            MissionPackage.MissionSummary.PingOutRadius = 200;

            // children add
            //MissionPackage.MissionData.LogicData.Spooler.Children.Add(MissionPackage.MissionData.LogicData.Nodes.Spooler); // LEPR (logic)
            //MissionPackage.MissionData.Spooler.Children.Add(MissionPackage.MissionData.LogicData.Spooler); // LEND (parent to mission data)

            // logic data
            //MissionPackage.MissionData.LogicData.Actors.Spooler = new DSCript.Spooling.SpoolablePackage();
            /*
            DSCript.Spooling.ChunkEntry LEND = new DSCript.Spooling.ChunkEntry()
            {
                Context = (int)ChunkType.LogicExportData
            };
            */

            MissionPackage.IsLoaded = true; // finally, set status true to the mission to "IsLoaded"
            InitTools(); // prepare tools
            //Debug.WriteLine($"[ZARTEX] Is Parent from logic LEPR equals to LEND: {MissionPackage.MissionData.LogicData.Nodes.Spooler.Parent == MissionPackage.MissionData.LogicData.Spooler}");
        }

        // actors creation by user

        // on create new character
        private void characterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            addNewActor(2, true); // character
            GenerateActors();
        }

        // on create new vehicle
        private void vehicleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            addNewActor(3, true); // vehicle
            GenerateActors();
        }

        private void aITargetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            addNewActor(6, true); // ai path
            GenerateActors();
        }

        private void objectiveIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            addNewActor(5, true); // objective icon
            GenerateActors();
        }

        private void characterdrivingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            // vehicle first
            ActorDefinition car = addNewActor(3, false);
            int carObjectIdx = MissionPackage.MissionData.Objects.Objects.Count - 1;

            // now the character
            ActorDefinition chara = addNewActor(2, false);
            CharacterObject charObj = (CharacterObject)MissionPackage.MissionData.Objects.Objects[ chara.ObjectId ];
            charObj.CreationData = new byte[]
            {
                1,0,1,13, // speculation: the first digit of the first int4 = in car? (bool)
                0,0,0,0,   0,0,0,0,    0,0,0,0, // seat position offset, why didn't they made it automatic?
                2,0,0,0  
            };
            charObj.UID = carObjectIdx;
            chara.Flags = 16583; // 0x40C7
            GenerateActors();
        }

        private void followActorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            addNewActor(7, true); // ai target
            GenerateActors();
        }

        private void fadeScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            addNewLogicNode(124);
            logicAddToWireCollectionByUser(MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1);
            GenerateLogicNodes();
        }

        // logic nodes creation by user

        private void addNodeToWireCollection(int wireCollection, int nodeId, byte wireType = 1)
        {
            NodeDefinition node = MissionPackage.MissionData.LogicData.Nodes.Definitions[nodeId];
            MissionPackage.MissionData.LogicData.WireCollection.WireCollections[wireCollection].Wires.Add(new WireNode()
            {
                NodeId = (short)nodeId,
                OpCode = node.TypeId,
                WireType = wireType // on success enable
            });
        }

        private void addNodeToWireCollection(int wireCollection, List<int> nodeIdList, byte wireType = 1)
        {
            foreach (var nodeId in nodeIdList) {
                NodeDefinition node = MissionPackage.MissionData.LogicData.Nodes.Definitions[nodeId];
                MissionPackage.MissionData.LogicData.WireCollection.WireCollections[wireCollection].Wires.Add(new WireNode()
                {
                    NodeId = (short)nodeId,
                    OpCode = node.TypeId,
                    WireType = wireType // on success enable
                });
            }
        }

        // template cutscene
        private void templateCutsceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,

                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,

                Text = "Enter a wire collection to add your cutscene"
            };

            Label textLabel = new Label()
            {
                Left = 50,
                Top = 20,

                Text = "Enter a wire:"
            };

            TextBox textBox = new TextBox()
            {
                Left = 50,
                Top = 50,

                Width = 400,

                SelectedText = MissionPackage.MissionData.LogicData.Nodes.Definitions[0].Properties[0].Value.ToString() //"0"
            };

            Button confirmation = new Button()
            {
                Left = 250,
                Top = 70,

                Width = 100,
                DialogResult = DialogResult.OK,

                Text = "OK"
            };

            confirmation.Click += delegate { prompt.Close(); };

            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            DialogResult dialogResult = prompt.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                int id = 0;
                float cutsceneFadeSpeed = 0.2f;
                try
                {
                    id = Convert.ToInt32(textBox.Text);
                    NodeDefinition IGCS = addNewLogicNode(100);
                    int IGCSID = MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1;
                    IGCS.StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Widescreen ON/OFF");

                    NodeDefinition cutsceneTimer = addNewLogicNode(3);
                    cutsceneTimer.Properties[1].Value = 2.0f; // the cutscene lasts 2 seconds by default
                    int cutsceneTimerID = MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1;
                    cutsceneTimer.StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Cutscene Timer");

                    NodeDefinition fadein1 = addNewLogicNode(124);
                    fadein1.StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Cutscene Start Fade In");
                    // fade in
                    fadein1.Properties[1].Value = cutsceneFadeSpeed;
                    fadein1.Properties[2].Value = 0f;
                    fadein1.Properties[3].Value = cutsceneFadeSpeed+0.1f; // duration of the logic node
                    int fadein1ID = MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1;
                    NodeDefinition fadeout1 = addNewLogicNode(124);
                    fadeout1.StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Cutscene Start Fade Out");
                    // fade out
                    fadeout1.Properties[1].Value = 0f;
                    fadeout1.Properties[2].Value = cutsceneFadeSpeed;
                    fadeout1.Properties[3].Value = cutsceneFadeSpeed; // duration of the logic node
                    int fadeout1ID = MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1;
                    // 2
                    NodeDefinition fadein2 = addNewLogicNode(124);
                    fadein2.StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Cutscene End Fade In");
                    // fade in
                    fadein2.Properties[1].Value = cutsceneFadeSpeed;
                    fadein2.Properties[2].Value = 0f;
                    fadein2.Properties[3].Value = cutsceneFadeSpeed+0.1f; // duration of the logic node
                    int fadein2ID = MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1;
                    NodeDefinition fadeout2 = addNewLogicNode(124);
                    fadeout2.StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Cutscene End Fade Out");
                    // fade out
                    fadeout2.Properties[1].Value = 0f;
                    fadeout2.Properties[2].Value = cutsceneFadeSpeed;
                    fadeout2.Properties[3].Value = cutsceneFadeSpeed; // duration of the logic node
                    int fadeout2ID = MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1;
                    addNodeToWireCollection(id, fadein1ID, 1);
                    addNodeToWireCollection((int)fadein1.Properties[0].Value, cutsceneTimerID, 1); // the timer may begin now
                    addNodeToWireCollection((int)fadein1.Properties[0].Value, fadeout1ID, 1);
                    addNodeToWireCollection((int)fadein1.Properties[0].Value, IGCSID, 1); // on success enable widescreen
                    // 2
                    addNodeToWireCollection((int)cutsceneTimer.Properties[0].Value, fadein2ID, 1);
                    addNodeToWireCollection((int)fadein2.Properties[0].Value, fadeout2ID, 1);
                    addNodeToWireCollection((int)fadein2.Properties[0].Value, IGCSID, 2); // on success disable widescreen
                    //node.Properties[0].Value = id;
                }
                catch
                {
                    MessageBox.Show("You didn't insert a valid wire!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                useFlowgraph = false;
                GenerateLogicNodes(); // update list
            }
        }

        private void actorCreationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            addNewLogicNode(101);
            logicAddToWireCollectionByUser(MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1);
            GenerateLogicNodes();
        }

        private void missionFailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            addNewLogicNode(6);
            logicAddToWireCollectionByUser(MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1);
            GenerateLogicNodes();
        }

        private void missionSuccessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            addNewLogicNode(5);
            logicAddToWireCollectionByUser(MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1);
            GenerateLogicNodes();
        }

        private void timerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            addNewLogicNode(3);
            logicAddToWireCollectionByUser(MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1);
            GenerateLogicNodes();
        }

        // special

        private void bombCarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            addNewLogicNode(122);
            logicAddToWireCollectionByUser(MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1);
            GenerateLogicNodes();
        }

        // main

        private void displayMessageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            addNewLogicNode(24);
            logicAddToWireCollectionByUser(MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1);
            GenerateLogicNodes();
        }
        
        // conditions

        private void gettingAwayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            NodeDefinition proxm = addNewLogicNode(30);
            proxm.Properties[4].Value = 2; // pCheckType : away
            logicAddToWireCollectionByUser(MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1);
            GenerateLogicNodes();
        }

        private void gettingCloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            NodeDefinition proxm = addNewLogicNode(30);
            proxm.Properties[4].Value = 1; // pCheckType : close
            logicAddToWireCollectionByUser(MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1);
            GenerateLogicNodes();
        }

        private void characterIsDeadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            NodeDefinition charwatch = addNewLogicNode(19);
            charwatch.Properties[3].Value = 1; // pWatchover : health equals to
            charwatch.Properties[4].Value = 0; // pValue     : zero
            charwatch.Properties[5].Value = 5; // pFlags     : dead?
            logicAddToWireCollectionByUser(MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1);
            GenerateLogicNodes();
        }

        private void characterIsChasedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            NodeDefinition charwatch = addNewLogicNode(19);
            charwatch.Properties[3].Value = 9;  // pWatchover : chased
            charwatch.Properties[4].Value = 0;  // pValue     : zero
            charwatch.Properties[5].Value = 14; // pFlags     : felony flags?
            logicAddToWireCollectionByUser(MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1);
            GenerateLogicNodes();
        }

        private void characterIsArrestedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            NodeDefinition charwatch = addNewLogicNode(19);
            charwatch.Properties[3].Value = 3; // pWatchover : arrested
            charwatch.Properties[4].Value = 0; // pValue     : zero
            charwatch.Properties[5].Value = 5; // pFlags     : arrested?
            logicAddToWireCollectionByUser(MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1);
            GenerateLogicNodes();
        }

        private void copControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            addNewLogicNode(102);
            logicAddToWireCollectionByUser(MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1);
            GenerateLogicNodes();
        }

        // cutscene

        private void toggleWidescreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            addNewLogicNode(100);
            logicAddToWireCollectionByUser(MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1);
            GenerateLogicNodes();
        }

        private void cameraSelectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            addNewLogicNode(14);
            logicAddToWireCollectionByUser(MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1);
            GenerateLogicNodes();
        }

        // add wire node

        private void wireNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            wireNodeAddToWireCollectionByUser();
        }

        private void chaseObjectBaddieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 9961472
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            NodeDefinition n = addNewActor(7, true); // ai target
            n.Properties[n.Properties.Count - 1].Value = 9961472; // last = pFlags
            GenerateActors();
        }

        private void musicControllerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            addNewLogicNode(118);
            logicAddToWireCollectionByUser(MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1);
            GenerateLogicNodes();
        }

        private void playCinematicVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            addNewLogicNode(25);
            logicAddToWireCollectionByUser(MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1);
            GenerateLogicNodes();
        }

        private void cutsceneSkipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            addNewLogicNode(131);
            logicAddToWireCollectionByUser(MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1);
            GenerateLogicNodes();
        }

        private void civilianTrafficControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            addNewLogicNode(104);
            logicAddToWireCollectionByUser(MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1);
            GenerateLogicNodes();
        }

        private void switchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            addNewActor(101, true); // switch
            GenerateActors();
        }
        
        private void switchPressedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            addNewLogicNode(13);
            logicAddToWireCollectionByUser(MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1);
            GenerateLogicNodes();
        }

        private void appendBankTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            askToAddSoundBank();
        }

        private void cameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            ActorDefinition cam = addNewActor(9);
            NodeDefinition camSel = addNewLogicNode(14);
            camSel.Properties[2].Value = 1; // enum => object camera

            // attach the object to it
            camSel.Properties[1].Value = MissionPackage.MissionData.LogicData.Actors.Definitions.Count - 1;

            // set default matrix (looking forward)
            CameraObject camObj = (CameraObject)MissionPackage.MissionData.Objects.Objects[cam.ObjectId];
            camObj.V1 = new Vector4(1, 0, 0, 0);
            camObj.V2 = new Vector4(-1, 0, 0, 0);
            camObj.V3 = new Vector4(-1, 0, 0, 0);

            logicAddToWireCollectionByUser(MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1);
            GenerateLogicNodes();
        }

        private void trainControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            addNewLogicNode(123);
            logicAddToWireCollectionByUser(MissionPackage.MissionData.LogicData.Nodes.Definitions.Count - 1);
            GenerateLogicNodes();
        }

        // LUA
        public static LuaMissionScript importLuaFromFile(string filepath)
        {
            Script script = new Script();
            UserData.RegisterAssembly();

            LuaMissionScript LMS = new LuaMissionScript();
            script.Globals["MISSION"] = LMS;

            // enums and stuff
            script.Globals["COLLECTABLE_PISTOL"] = CollectableType.Pistol;
            script.Globals["COLLECTABLE_BERETTA"] = CollectableType.Beretta;
            script.Globals["COLLECTABLE_SILENCED"] = CollectableType.Silenced;
            script.Globals["COLLECTABLE_M4A1"] = CollectableType.M4A1;
            script.Globals["COLLECTABLE_SHOTGUN"] = CollectableType.Shotgun;
            script.Globals["COLLECTABLE_UZI"] = CollectableType.Uzi;
            script.Globals["COLLECTABLE_SMG"] = CollectableType.SubMachineGun;
            script.Globals["COLLECTABLE_MG"] = CollectableType.MachineGun;
            script.Globals["COLLECTABLE_MEDKIT"] = CollectableType.Medkit;

            script.Globals["PERSONALITY_UNDEFINED"] = -1;
            script.Globals["PERSONALITY_CIVILIANSTUCK"] = 0;
            script.Globals["PERSONALITY_NORMAL"] = 1;
            script.Globals["PERSONALITY_PATHANDVEHICLE"] = 2;
            script.Globals["PERSONALITY_PATHFACE"] = 9;

            //script.DoString("local logicStart = MISSION.logicStart()"); // logic start global variable

            DynValue res = script.DoFile(filepath);

            //script.DoString("LogicStart()"); // calls the logic start
            return LMS;
        }

        // LUA
        public static LuaMissionScriptDPL importLuaFromFileDPL(string filepath)
        {
            Script script = new Script();
            UserData.RegisterAssembly();

            LuaMissionScriptDPL LMS = new LuaMissionScriptDPL();
            script.Globals["MISSION"] = LMS;

            //script.DoString("local logicStart = MISSION.logicStart()"); // logic start global variable

            DynValue res = script.DoFile(filepath);

            //script.DoString("LogicStart()"); // calls the logic start
            return LMS;
        }

        private void importLuaScript_Click(object sender, EventArgs e)
        {
            OpenFileDialog luaFileDialog = new OpenFileDialog
            {
                Title = "Open Lua File for Driv3r Lua",
                Filter = "Lua file|*.lua"
            };

            if (luaFileDialog.ShowDialog() == DialogResult.OK)
            {
                LuaMissionScript luaMission;
                try
                {
                    luaMission = importLuaFromFile(luaFileDialog.FileName);
                }
                catch (ScriptRuntimeException ex)
                {
                    Console.WriteLine(ex.DecoratedMessage);
                    MessageBox.Show(ex.DecoratedMessage, "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // copy data from lua mission to current mission
                MissionPackage.MissionData.LogicData.Actors.Definitions = luaMission.missionData.LogicData.Actors.Definitions;
                MissionPackage.MissionData.LogicData.Nodes.Definitions = luaMission.missionData.LogicData.Nodes.Definitions;
                MissionPackage.MissionData.LogicData.WireCollection.WireCollections = luaMission.wireCollection;

                MissionPackage.MissionData.LogicData.ActorSetTable.Table = luaMission.actorSetup.Table;

                // mission instance data
                if (MissionPackage.MissionData.MissionInstances == null & luaMission.instanceData.Instances.Count != 0)
                {
                    MissionPackage.MissionData.MissionInstances = new MissionInstanceData();

                    MissionPackage.MissionData.MissionInstances.Spooler = new DSCript.Spooling.SpoolableBuffer()
                    {
                        Context = (int)ChunkType.BuildingInstanceData,
                        Description = "Custom Mission Instances data",
                    };
                    // add the spooler to mission data
                    MissionPackage.MissionData.Spooler.Children.Add(MissionPackage.MissionData.MissionInstances.Spooler);
                }
                if (luaMission.instanceData.Instances.Count != 0)
                {
                    MissionPackage.MissionData.MissionInstances.Instances = luaMission.instanceData.Instances;
                    // add it's objects for them ofc
                    luaMission.missionData.Objects.Objects.AddRange(luaMission.instanceData._props);
                }

                MissionPackage.MissionData.Objects.Objects = luaMission.missionData.Objects.Objects;

                MissionPackage.MissionData.LogicData.StringCollection.Strings = luaMission.missionData.LogicData.StringCollection.Strings;

                MissionPackage.MissionData.LogicData.SoundBankTable.Table = luaMission.missionData.LogicData.SoundBankTable.Table;

                if (MissionPackage.MissionSummary == null)
                    MissionPackage.MissionSummary = new MissionSummaryData();
                MissionPackage.MissionSummary.StartPosition = luaMission.missionSummary.StartPosition;
                MissionPackage.MissionSummary.CityType = luaMission.missionSummary.GetCityTypeByName(luaMission.missionSummary.Level);
                MissionPackage.MissionSummary.MissionId = luaMission.missionSummary.MoodId;

                MessageBox.Show("Success loading Lua mission script file!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void importLuaScriptDPL_Click(object sender, EventArgs e)
        {
            OpenFileDialog luaFileDialog = new OpenFileDialog
            {
                Title = "Open Lua File for Driver: Parallel Lines Lua",
                Filter = "Lua file|*.lua"
            };

            if (luaFileDialog.ShowDialog() == DialogResult.OK)
            {
                LuaMissionScriptDPL luaMission;
                //try
                //{
                    luaMission = importLuaFromFileDPL(luaFileDialog.FileName);
                //}
                //catch (ScriptRuntimeException ex)
                //{
                //    Console.WriteLine(ex.DecoratedMessage);
                //    MessageBox.Show(ex.DecoratedMessage+"\n / \n"+ex.Message, "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}

                // copy data from lua mission to current mission
                MissionPackage.MissionData.LogicData.Actors.Definitions = luaMission.missionData.LogicData.Actors.Definitions;
                MissionPackage.MissionData.LogicData.Nodes.Definitions = luaMission.missionData.LogicData.Nodes.Definitions;
                MissionPackage.MissionData.LogicData.WireCollection.WireCollections = luaMission.wireCollection;

                MissionPackage.MissionData.Objects.Objects = luaMission.missionData.Objects.Objects;

                MissionPackage.MissionData.LogicData.StringCollection.Strings = luaMission.missionData.LogicData.StringCollection.Strings;

                MissionPackage.MissionData.LogicData.SoundBankTable.Table = luaMission.missionData.LogicData.SoundBankTable.Table;

                // hack hacked succesfully
                var off = MissionPackage.MissionSummary.DPLBuffer.Length - 1;
                MissionPackage.MissionSummary.DPLBuffer[off] = 0;
                MissionPackage.MissionSummary.DPLBuffer[off-1] = 0;

                //MissionPackage.MissionSummary.StartPosition = luaMission.missionSummary.StartPosition;
                //MissionPackage.MissionSummary.CityType = luaMission.missionSummary.GetCityTypeByName(luaMission.missionSummary.Level);
                //MissionPackage.MissionSummary.MissionId = luaMission.missionSummary.MoodId;

                MessageBox.Show("Success loading Lua mission script file!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void getPlayerPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process[] d3p = Process.GetProcessesByName("Driv3r");
            Process[] dplp = Process.GetProcessesByName("DriverParallelLines");
            if ((d3p.Length==0 & dplp.Length==0) & gameProcess == null)
            {
                MessageBox.Show("Game not found in process list (Driv3r or DriverParallelLines)", "Error",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            MemoryEdit.Memory mem = new MemoryEdit.Memory();

            bool isDPL = dplp.Length!=0;

            uint addr1 = 0x008B85D8;
            uint addr2 = 0x10;
            uint addr3 = 0x3B0;
            // dpl
            uint addr4 = 0x006E8E28;
            uint addr5 = 0x58;
            uint addr6 = 0x40;

            Process prc = isDPL ? dplp[0] : d3p[0];

            if (mem.Attach(prc, 0x001F0FFF))
            {
                gameProcess = prc;
                gameProcess.Exited += delegate { gameProcess = null; };

                uint playerAddr = isDPL ? addr4 : addr1;
                uint address1 = isDPL ? addr5 : addr2;
                uint address2 = isDPL ? addr6 : addr3;

                uint addr = (uint)(mem.Read(playerAddr) + address1);
                addr = (uint)(mem.Read(addr) + address2);
                float x = mem.ReadFloat(addr);
                float y = mem.ReadFloat(addr+4);
                float z = mem.ReadFloat(addr+8);
                float fx = mem.ReadFloat(addr - 16);
                float fy = mem.ReadFloat(addr - 12);
                float fz = mem.ReadFloat(addr - 8);
                float deg = (180 / (float)Math.PI);
                float a = ((float)Math.Atan2(fz, fx)) * deg;
                Clipboard.SetText($"{x}, {y}, {z}, {a}");
                MessageBox.Show($"Player's position and angle has copied to your cilpboard (0x{addr:X8})", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Failed to attach memory editor to the game process!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void driverParallelLinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Open Driver: Parallel Lines mission script file",
                Filter = "Mission script file|*.mpc;*.sp|All files|*.*",
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                int missionId = 0;
                if (openFileDialog.FileName.ToLower().Contains(".sp"))
                {
                    Form prompt = new Form()
                    {
                        Width = 500,
                        Height = 150,

                        FormBorderStyle = FormBorderStyle.FixedDialog,
                        StartPosition = FormStartPosition.CenterScreen,

                        Text = "Enter a mission ID"
                    };

                    Label textLabel = new Label()
                    {
                        Left = 50,
                        Top = 20,

                        Text = "Enter a number:"
                    };

                    TextBox textBox = new TextBox()
                    {
                        Left = 50,
                        Top = 50,

                        Width = 400,

                        SelectedText = "0"
                    };

                    Button confirmation = new Button()
                    {
                        Left = 250,
                        Top = 70,

                        Width = 100,
                        DialogResult = DialogResult.OK,

                        Text = "OK"
                    };

                    Button cancel = new Button()
                    {
                        Left = 350,
                        Top = 70,

                        Width = 100,
                        DialogResult = DialogResult.Cancel,

                        Text = "Cancel"
                    };

                    confirmation.Click += delegate { prompt.Close(); };

                    prompt.Controls.Add(textBox);
                    prompt.Controls.Add(confirmation);
                    prompt.Controls.Add(cancel);
                    prompt.Controls.Add(textLabel);
                    prompt.AcceptButton = confirmation;

                    DialogResult dialogResult = prompt.ShowDialog();
                    if (dialogResult == DialogResult.OK)
                    {

                            missionId = Convert.ToInt32(textBox.Text);
                            isDriverPLMission = true;
                            LoadScriptFile(openFileDialog.FileName, isDriverPLMission, missionId);


                    }
                }
                else
                {
                    // else, just load the mission with mission ID set to zero
                    isDriverPLMission = true;
                    LoadScriptFile(openFileDialog.FileName, isDriverPLMission);
                }
            }
        }

        private void giveWeaponToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            // booly heck
            bool hasUID = MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValue("UID") != -1;

            MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Add(new WireCollection(0));
            int idx = MissionPackage.MissionData.LogicData.Nodes.Definitions.Count;
            MissionPackage.MissionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(70, 42, 247, 255),
                TypeId = 217,
                StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"),
                Properties = new List<NodeProperty>()
            });
            // hack
            if (hasUID)
                MissionPackage.MissionData.LogicData.Nodes.Definitions[idx].Properties.Add(new UIDProperty((ulong)(idx * 0x0FFFFFFF0))
                {
                    StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("UID")
                });
            MissionPackage.MissionData.LogicData.Nodes.Definitions[idx].Properties.AddRange(
                new List<NodeProperty>
                    {
                        new WireCollectionProperty(MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Count-1) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new EnumProperty(0) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Weapon")
                        }
                    });
            
            logicAddToWireCollectionByUser(idx);
            GenerateLogicNodes();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (Widget == null)
            {
                MessageBox.Show("Nothing to find", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,

                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,

                Text = "Find"
            };

            Label textLabel = new Label()
            {
                Left = 50,
                Top = 20,

                Text = "Enter something:"
            };

            TextBox textBox = new TextBox()
            {
                Left = 50,
                Top = 50,

                Width = 400,

                SelectedText = "LogicStart"
            };

            Button find = new Button()
            {
                Left = 250,
                Top = 70,

                Width = 100,

                Text = "Find"
            };

            Button next = new Button()
            {
                Left = 350,
                Top = 70,

                Width = 100,

                Text = "Find next"
            };

            //confirmation.Click += delegate { prompt.Close(); };

            prompt.Controls.Add(textBox);
            prompt.Controls.Add(find);
            prompt.Controls.Add(next);
            prompt.Controls.Add(textLabel);
            //prompt.AcceptButton = find;

            TreeNode oldNode = new TreeNode();
            find.Click += delegate
            {
                if (textBox.Text == "")
                {
                    MessageBox.Show("Inputed text is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                foreach (TreeNode node in Widget.Nodes.Nodes)
                {
                    if (node.Text.ToLower().Contains(textBox.Text.ToLower()))
                    {
                        
                        Widget.Nodes.SelectedNode = node;
                        if (oldNode==node)
                            MessageBox.Show($"You have already found a node as previously", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        oldNode = node;
                        return;
                    }
                }
                MessageBox.Show($"Couldn't find a node from your criteria: '{textBox.Text}'", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };
            next.Click += delegate
            {
                if (textBox.Text == "")
                {
                    MessageBox.Show("Inputed text is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                for (int id = Widget.Nodes.SelectedNode.Index+1; id<Widget.Nodes.Nodes.Count; id++)
                {
                    TreeNode node = Widget.Nodes.Nodes[id];
                    if (node.Text.ToLower().Contains(textBox.Text.ToLower()))
                    {
                        Widget.Nodes.SelectedNode = node; return;
                    }
                }
                MessageBox.Show($"Couldn't find any other node from your criteria: '{textBox.Text}'", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };
            prompt.Show(this);
        }

        // crap crap crap
        private void characterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            // booly heck
            bool hasUID = false; //MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValue("UID") != -1;

            int idx = MissionPackage.MissionData.LogicData.Actors.Definitions.Count;
            MissionPackage.MissionData.LogicData.Actors.Definitions.Add(new ActorDefinition()
            {
                Color = new NodeColor(170, 42, 247, 255),
                ObjectId = -1,
                TypeId = 2,
                StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"),
                Properties = new List<NodeProperty>()
            });
            // hack
            if (hasUID)
                MissionPackage.MissionData.LogicData.Nodes.Definitions[idx].Properties.Add(new UIDProperty((ulong)(idx * 0x0FFFFFFF0))
                {
                    StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("UID")
                });
            MissionPackage.MissionData.LogicData.Actors.Definitions[idx].Properties.AddRange(
                new List<NodeProperty>
                    {
                        new MatrixProperty(new Vector4(0,0,0,1),new Vector3(-1,0,0),new Vector3(0,1,0),new Vector3(0,0,1)) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Matrix")
                        },
                        new ObjectTypeProperty(29) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Character Skin Type")
                        },
                        new FloatProperty(1) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Initial Health")
                        },
                        new FloatProperty(0) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Initial Felony")
                        },
                        new EnumProperty(0) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Weapon")
                        },
                        new ActorProperty(-1) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Initial Vehicle")
                        },
                        new EnumProperty(0) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Initial Vehicle Seat")
                        },
                        new FlagsProperty((int)1) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Flags")
                        }           // pFlags
                    });

            GenerateActors();
        }

        private void copToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            // booly heck
            bool hasUID = false; //MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValue("UID") != -1;

            int idx = MissionPackage.MissionData.LogicData.Actors.Definitions.Count;
            MissionPackage.MissionData.LogicData.Actors.Definitions.Add(new ActorDefinition()
            {
                Color = new NodeColor(170, 42, 247, 255),
                ObjectId = -1,
                TypeId = 118,
                StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"),
                Properties = new List<NodeProperty>()
            });
            // hack
            if (hasUID)
                MissionPackage.MissionData.LogicData.Nodes.Definitions[idx].Properties.Add(new UIDProperty((ulong)(idx * 0x0FFFFFFF0))
                {
                    StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("UID")
                });
            MissionPackage.MissionData.LogicData.Actors.Definitions[idx].Properties.AddRange(
                new List<NodeProperty>
                    {
                        new MatrixProperty(new Vector4(0,0,0,1),new Vector3(-1,0,0),new Vector3(0,1,0),new Vector3(0,0,1)) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Matrix")
                        },
                        new EnumProperty(2) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Cop Type")
                        },
                        new FlagsProperty(1) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Flags")
                        }           // pFlags
                    });

            GenerateActors();
        }

        private void vehicleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            // booly heck
            bool hasUID = false; //MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValue("UID") != -1;

            int idx = MissionPackage.MissionData.LogicData.Actors.Definitions.Count;
            MissionPackage.MissionData.LogicData.Actors.Definitions.Add(new ActorDefinition()
            {
                Color = new NodeColor(170, 42, 247, 255),
                ObjectId = -1,
                TypeId = 3,
                StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"),
                Properties = new List<NodeProperty>()
            });
            // hack
            if (hasUID)
                MissionPackage.MissionData.LogicData.Nodes.Definitions[idx].Properties.Add(new UIDProperty((ulong)(idx * 0x0FFFFFFF0))
                {
                    StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("UID")
                });
            MissionPackage.MissionData.LogicData.Actors.Definitions[idx].Properties.AddRange(
                new List<NodeProperty>
                    {
                        new MatrixProperty(new Vector4(0,0,0,1),new Vector3(-1,0,0),new Vector3(0,1,0),new Vector3(0,0,1)) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Matrix")
                        },
                        new ObjectTypeProperty(47) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Vehicle Type")
                        },
                        new FloatProperty(0) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Initial Speed")
                        },
                        new FloatProperty(0) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Initial Felony")
                        },
                        new FloatProperty(1) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Impact Softness")
                        },
                        new FloatProperty(1) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Explosion Softness")
                        },
                        new FloatProperty(1) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Bullet Softness")
                        },
                        new FloatProperty(1) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Impact Fragility")
                        },
                        new VehicleTintProperty(0) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Vehicle Tint")
                        },
                        new IntegerProperty(0) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Tint Value")
                        },
                        new ActorProperty(-1) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Attached Vehicle")
                        },
                        new FlagsProperty((int)1) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Flags")
                        }           // pFlags
                    });

            GenerateActors();
        }

        private void actorCreationToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            // booly heck
            bool hasUID = MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValue("UID") != -1;

            MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Add(new WireCollection(0));
            int idx = MissionPackage.MissionData.LogicData.Nodes.Definitions.Count;
            MissionPackage.MissionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(70, 142, 247, 255),
                TypeId = 101,
                StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"),
                Properties = new List<NodeProperty>()
            });
            // hack
            if (hasUID)
                MissionPackage.MissionData.LogicData.Nodes.Definitions[idx].Properties.Add(new UIDProperty((ulong)(idx * 0x0FFFFFFF0))
                {
                    StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("UID")
                });
            MissionPackage.MissionData.LogicData.Nodes.Definitions[idx].Properties.AddRange(
                new List<NodeProperty>
                    {
                        new WireCollectionProperty(MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Count-1) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new ActorProperty(0) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Actor")
                        },
                        new EnumProperty(0) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Activity")
                        },
                        new FlagsProperty(0) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Flags")
                        }           // pFlags
                    });

            logicAddToWireCollectionByUser(idx);
            GenerateLogicNodes();
        }

        private void markerControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            // booly heck
            bool hasUID = MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValue("UID") != -1;

            MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Add(new WireCollection(0));
            int idx = MissionPackage.MissionData.LogicData.Nodes.Definitions.Count;
            MissionPackage.MissionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(70, 142, 247, 255),
                TypeId = 186,
                StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"),
                Properties = new List<NodeProperty>()
            });
            // hack
            if (hasUID)
                MissionPackage.MissionData.LogicData.Nodes.Definitions[idx].Properties.Add(new UIDProperty((ulong)(idx * 0x0FFFFFFF0))
                {
                    StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("UID")
                });
            MissionPackage.MissionData.LogicData.Nodes.Definitions[idx].Properties.AddRange(
                new List<NodeProperty>
                    {
                        new WireCollectionProperty(MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Count-1) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new ActorProperty(0) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Actor")
                        },
                        new EnumProperty(1) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Activity")
                        },
                        new EnumProperty(1) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Display Type")
                        },
                        new EnumProperty(2) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Minimap Display Type")
                        },
                        new Float3Property(new Vector4(0,0,0,0))
                        {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Minimap Icon Colour")
                        },
                        new EnumProperty(1) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Visibility")
                        },
                        new LocalisedStringProperty(-1) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Description")
                        },
                        new FlagsProperty(2) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Flags")
                        }           // pFlags
                    });

            logicAddToWireCollectionByUser(idx);
            GenerateLogicNodes();
        }

        private void darkThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toggleDarkTheme();
        }

        private void pursuitControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            // booly heck
            bool hasUID = MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValue("UID") != -1;

            MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Add(new WireCollection(0));
            int idx = MissionPackage.MissionData.LogicData.Nodes.Definitions.Count;
            MissionPackage.MissionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(190, 70, 147, 255),
                TypeId = 164,
                StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"),
                Properties = new List<NodeProperty>()
            });
            // hack
            if (hasUID)
                MissionPackage.MissionData.LogicData.Nodes.Definitions[idx].Properties.Add(new UIDProperty((ulong)(idx * 0x0FFFFFFF0))
                {
                    StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("UID")
                });
            MissionPackage.MissionData.LogicData.Nodes.Definitions[idx].Properties.AddRange(
                new List<NodeProperty>
                    {
                        new WireCollectionProperty(MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Count-1) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new ActorProperty(0) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Chaser")
                        },
                        new ActorProperty(1) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Chase Target")
                        },
                        new FloatProperty(22.352f) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Speed")
                        },
                        new FloatProperty(1.0f) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Acceleration")
                        },
                        new FloatProperty(1.0f) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Traction")
                        },
                        new FloatProperty(1.0f) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Hand Of Tom")
                        },
                        new EnumProperty(1) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Pursuit Mode")
                        },
                        new FlagsProperty(25165824) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Flags")
                        }           // pFlags
                    });

            logicAddToWireCollectionByUser(idx);
            GenerateLogicNodes();
        }

        private void wandererToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MissionPackage == null)
            {
                MessageBox.Show("No mission loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            // booly heck
            bool hasUID = MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValue("UID") != -1;

            MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Add(new WireCollection(0));
            int idx = MissionPackage.MissionData.LogicData.Nodes.Definitions.Count;
            MissionPackage.MissionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(190, 70, 147, 255),
                TypeId = 166,
                StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"),
                Properties = new List<NodeProperty>()
            });
            // hack
            if (hasUID)
                MissionPackage.MissionData.LogicData.Nodes.Definitions[idx].Properties.Add(new UIDProperty((ulong)(idx * 0x0FFFFFFF0))
                {
                    StringId = (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("UID")
                });
            MissionPackage.MissionData.LogicData.Nodes.Definitions[idx].Properties.AddRange(
                new List<NodeProperty>
                    {
                        new WireCollectionProperty(MissionPackage.MissionData.LogicData.WireCollection.WireCollections.Count-1) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new ActorProperty(0) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Wanderer")
                        },
                        new FloatProperty(22.352f) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Speed")
                        },
                        new FloatProperty(1.0f) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Acceleration")
                        },
                        new FloatProperty(1.0f) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Traction")
                        },
                        new FloatProperty(1.0f) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Hand Of Tom")
                        },
                        new FlagsProperty(25165824) {
                            StringId =  (short)MissionPackage.MissionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Flags")
                        }           // pFlags
                    });

            logicAddToWireCollectionByUser(idx);
            GenerateLogicNodes();
        }

        private void importMPCBTN_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Import external .mpc file to current mission",
                Filter = "Mission Script|*.mpc;*.mxb;*.mps"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                MissionScriptFile externalMissionPackage = new MissionScriptFile(openFileDialog.FileName,MissionPackage.isDriverPLMission);

                // import the data but don't change the spooler

                // wire collections
                MissionPackage.MissionData.LogicData.WireCollection.WireCollections =
                    externalMissionPackage.MissionData.LogicData.WireCollection.WireCollections;

                // logical definitions
                MissionPackage.MissionData.LogicData.Actors.Definitions =
                    externalMissionPackage.MissionData.LogicData.Actors.Definitions;
                MissionPackage.MissionData.LogicData.Nodes.Definitions =
                    externalMissionPackage.MissionData.LogicData.Nodes.Definitions;

                // strings
                MissionPackage.MissionData.LogicData.StringCollection.Strings =
                    externalMissionPackage.MissionData.LogicData.StringCollection.Strings;

                // sound bank table
                MissionPackage.MissionData.LogicData.SoundBankTable.Table =
                    externalMissionPackage.MissionData.LogicData.SoundBankTable.Table;

                // actor set table
                //MissionPackage.MissionData.LogicData.ActorSetTable.Spooler.SetBuffer(externalMissionPackage.MissionData.LogicData.ActorSetTable.Spooler.GetBuffer());
                MissionPackage.MissionData.LogicData.ActorSetTable.Table = externalMissionPackage.MissionData.LogicData.ActorSetTable.Table;

                externalMissionPackage.Dispose();
                useFlowgraph = false;
                GenerateLogicNodes();
            }
        }

        private void exportAsBTN_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "Export as external .mpc file from current mission",
                Filter = "Mission Script|*.mpc;*.mxb;*.mps"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (!File.Exists(saveFileDialog.FileName))
                {
                    FileStream newFile = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                    //byte[] template = ReadResource("templateExportMission");
                    newFile.Write(Properties.Resources.templateExportMission);

                    newFile.Close();
                    newFile.Dispose();
                }
                else
                {
                    FileStream overPoseFile = new FileStream(saveFileDialog.FileName, FileMode.Open, FileAccess.Write);
                    //byte[] template = ReadResource("templateExportMission");
                    overPoseFile.Write(Properties.Resources.templateExportMission);

                    overPoseFile.Close();
                    overPoseFile.Dispose();
                }

                MissionScriptFile externalMissionPackage = new MissionScriptFile(saveFileDialog.FileName, MissionPackage.isDriverPLMission);

                // import the data but don't change the spooler

                // wire collections
                externalMissionPackage.MissionData.LogicData.WireCollection.WireCollections = MissionPackage.MissionData.LogicData.WireCollection.WireCollections;

                // logical definitions
                externalMissionPackage.MissionData.LogicData.Actors.Definitions = MissionPackage.MissionData.LogicData.Actors.Definitions;
                externalMissionPackage.MissionData.LogicData.Nodes.Definitions = MissionPackage.MissionData.LogicData.Nodes.Definitions;

                // objects
                externalMissionPackage.MissionData.Objects.Objects = MissionPackage.MissionData.Objects.Objects;

                // strings
                externalMissionPackage.MissionData.LogicData.StringCollection.Strings = MissionPackage.MissionData.LogicData.StringCollection.Strings;

                // sound bank table
                externalMissionPackage.MissionData.LogicData.SoundBankTable.Table = MissionPackage.MissionData.LogicData.SoundBankTable.Table;

                externalMissionPackage.MissionSummary = MissionPackage.MissionSummary;

                // actor set table
                //externalMissionPackage.MissionData.LogicData.ActorSetTable.Spooler.SetBuffer(MissionPackage.MissionData.LogicData.ActorSetTable.Spooler.GetBuffer());
                externalMissionPackage.MissionData.LogicData.ActorSetTable.Table = MissionPackage.MissionData.LogicData.ActorSetTable.Table;

                DialogResult res = MessageBox.Show("Would you like to add a custom build info to your mission? :)", "Add Build Info", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                bool addBINF = (res == DialogResult.Yes);

                if (addBINF)
                {
                    var game = isDriverPLMission ? "Driver: Parallel Lines" : "Driv3r";
                    externalMissionPackage.BuildInfo = $"Generated by Zartex 2.0 v{info_Version}\nExport from: {MissionPackage.FileName}\nExport to: {saveFileDialog.FileName}\n\nGame: {game}\nAt date: {DateTime.Now.ToString()} ";
                }

                bool suc = false;
                //if (externalMissionPackage.Spooler != null)
                    suc = externalMissionPackage.Save();

                if (suc)
                    MessageBox.Show($"Successfully exported mission to '{saveFileDialog.FileName}'!","Success",MessageBoxButtons.OK,MessageBoxIcon.Information);
                else
                    MessageBox.Show("Something went wrong :(","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
    }
}
