/*
- D3M Mission script interpreter & compiler
- Made by BuilderDemo7

-- D3M means Driver 3 Mission

THIS IS A WORK IN PROGRESS, IT'S NOT READY TO USE !!! 
*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using DSCript;
using DSCript.Spooling;

namespace Zartex
{
    public class D3M_CompileException : Exception
    {
        public string SourceScriptName { get; protected set; }
        public int ErrorLine { get; protected set; }
        public D3M_CompileException(string error, string sourceScriptName, int line) : base($"{sourceScriptName}: line {line + 1} - {error}")
        {
            SourceScriptName = sourceScriptName;
            ErrorLine = line;
        }
        public D3M_CompileException(string error, int line) : base($"line {line} - {error}")
        {
            ErrorLine = line;
        }
        public D3M_CompileException(string message) : base(message)
        {
        }
    }
    // Virtual variable
    public class D3M_Variable
    {
        public D3M_VariableType Type = D3M_VariableType.Null;
        public string Name = "";

        public double Number;
        public string Text;
        public D3M_Object Object;
        public bool Boolean;
        public int CounterIndex;
        public string LabelName = "NOLABEL";

        /// <summary>
        /// If set to true it can't be written by any D3M script
        /// </summary>
        public bool ReadOnly;
    }
    public enum D3M_VariableType : int
    {
        Undefined = -2,
        Null = -1,

        Number = 0,
        Text = 1,
        Object = 2,
        Pointer = 3, // pointer is specified in 'Text' property
        Boolean = 4,
        Counter = 5,

        Label = 6
    }
    public enum D3M_OpcodeType : int
    {
        Nop = 0, // must always be 0
        DebugText = 1,
        MissionComplete = 2,
        MissionFailed = 3,

        CreateVehicle = 4,
        CreateCharacter = 5,

        // new opcodes should be here
        // better not try to sort them cuz it could break scripts
        MusicControl = 6,

        // anyways, don't need to set the number, just the name
        InstantiateActor, // a.k.a 'ActorCreation'
        FrameDelay, // a.k.a 'Converter'
        AreaWatch,
        ActionButtonWatch,
        CameraSelect,
        Accumulator,
        PlayFMV,
        PlayAudio,
        DisplayMessage,

        ShowCountdown,
        StopCountdown,
        HideCountdown,
        SetCountdownTime
    }
    public enum D3M_ObjectType : int
    {
        Null = -1,

        Vehicle = 0,
        Character = 1,
        Area = 2,

    }
    // Mission script translation: actors
    public abstract class D3M_Object
    {
        // default color of the node
        // (gold)
        protected const byte r = 255;
        protected const byte g = 201;
        protected const byte b = 14;

        public MissionObject MissionObject;
        public ActorDefinition Definition;

        public string ObjectName = ""; // identification in the script
        public D3M_ObjectType Type = D3M_ObjectType.Null;

        /// <summary>
        /// Position of the object
        /// 
        /// X = Altitude
        /// Y = Latitude
        /// Z = Depth
        /// </summary>
        public float X, Y, Z;

        /// <summary>
        /// Rotation of the object (in euler angles)
        /// 
        /// X = Pitch (facing down or up)
        /// Y = Yaw (turning left or right)
        /// Z = Roll (rolling left or right)
        /// </summary>
        public float RX, RY, RZ;

        public bool StartUninitialised = true; // means the object will be ready to be initialised later

        public virtual void CompileObject(ExportedMission exportedMission)
        {
            throw new NotImplementedException();
        }
    }
    public class D3M_Character : D3M_Object
    {
        public int Role = 1;
        public float InitialHealth = 1;
        public float InitialFelony = 0;
        public int InitialWeapon = 0;
        public float Vulnerability = 1;

        public string Personality = "";
        public int PersonalityIndex = -1;
        public uint SkinId = 0;

        public int Flags = 0;

        public D3M_Character(uint skinId = 0, int role = 1, float initialHealth = 1, float initialFelony = 0, int initialWeapon = 0, float vulnerability = 1, string personality = "", int personalityIdx = -1, int flags = 0)
        {
            ObjectName = "Unknown";
            Type = D3M_ObjectType.Character;

            Role = role;
            InitialHealth = initialHealth;
            InitialFelony = initialFelony;
            InitialWeapon = initialWeapon;
            Vulnerability = vulnerability;
            Personality = personality;
            PersonalityIndex = personalityIdx;
            SkinId = skinId;

            Flags = flags;
        }

        public override void CompileObject(ExportedMission exportedMission)
        {
            var a = (Math.PI / 180) * RY; // convert degrees to radians
            Vector3 fwd = new Vector3(
                (float)(Math.Cos(0) * Math.Cos(a)), // x

                0, //(float)-Math.Sin(angle), // altitude

                (float)(Math.Cos(0) * Math.Sin(a)) // z
            );
            int flags = Flags | (StartUninitialised ? 0 : 1);
            short stringId = (short)exportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew(ObjectName);
            exportedMission.LogicData.Actors.Definitions.Add(new ActorDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 2,
                StringId = stringId,
                ObjectId = exportedMission.Objects.Objects.Count,
                Flags = 0x2FD7,
                Properties = new List<NodeProperty>()
                {
                        new IntegerProperty((int)Role) {
                            StringId =  (short)exportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pRole")
                        },          // pRole
                        new TextFileItemProperty((short)exportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew(Personality),(short)PersonalityIndex) {
                            StringId =  (short)exportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pPersonality")
                        },          // pPersonality
                        new IntegerProperty(PersonalityIndex) {
                            StringId =  (short)exportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pPersonalityIndex")
                        },          // pPersonalityIndex
                        new FloatProperty((float)InitialHealth) {
                            StringId =  (short)exportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pHealth")
                        },          // pHealth
                        new FloatProperty((float)InitialFelony) {
                            StringId =  (short)exportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFelony")
                        },          // pFelony
                        new EnumProperty((int)InitialWeapon) {
                            StringId =  (short)exportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWeapon")
                        },          // pWeapon
                        new FloatProperty((float)Vulnerability) {
                            StringId =  (short)exportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pVulnerability")
                        },          // pVulnerability
                        new FlagsProperty((int)flags) {
                            StringId =  (short)exportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }           // pFlags 
                }
            });
            byte[] bX = BitConverter.GetBytes((float)X);
            byte[] bY = BitConverter.GetBytes((float)Y);
            byte[] bZ = BitConverter.GetBytes((float)Z);
            byte[] bFX = BitConverter.GetBytes((float)fwd.X);
            byte[] bFY = BitConverter.GetBytes((float)fwd.Y);
            byte[] bFZ = BitConverter.GetBytes((float)fwd.Z);
            exportedMission.Objects.Objects.Add(new CharacterObject()
            {
                UID = (int)-1073623027, // 0xC001D00D
                SkinId = SkinId,
                CreationData = new byte[]
                    {
                        0,0,1,0x0D,
                        // reserved for position here
                        bX[0],bX[1],bX[2],bX[3],      bY[0],bY[1],bY[2],bY[3],     bZ[0],bZ[1],bZ[2],bZ[3],
                        // reserved for forward (where the angle is already calculated)
                        bFX[0],bFX[1],bFX[2],bFX[3], bFY[0],bFY[1],bFY[2],bFY[3], bFZ[0],bFZ[1],bFZ[2],bFZ[3],
                        // hmm..
                        //0xBB,0x66,0x7F,0xBF // <-- character UID?
                },
                Position = new DSCript.Vector4((float)X, (float)Y, (float)Z, RZ)
            });
            int idx = exportedMission.LogicData.Actors.Definitions.Count - 1;
            Definition = exportedMission.LogicData.Actors.Definitions[idx];
            MissionObject = exportedMission.Objects.Objects[Definition.ObjectId];
        }
    }
    public class D3M_Vehicle : D3M_Object
    {
        public uint VehicleType;

        public float PartsDamage1 = 0;
        public float PartsDamage2 = 0;
        public float PartsDamage3 = 0;
        public float PartsDamage4 = 0;
        public float PartsDamage5 = 0;
        public float InitialDamage = 0;
        public float Weight = 1;
        public float Softness = 1;
        public float Fragility = 1;
        public int TintId = 0;

        public D3M_Vehicle()
        {
            VehicleType = 0;
            Type = D3M_ObjectType.Vehicle;

            InitialDamage = 0;
            Weight = 1;
            Softness = 1;
        }
        public D3M_Vehicle(uint vehicleType, float initX, float initY, float initZ, float initRX = 0, float initRY = 0, float initRZ = 0)
        {
            ObjectName = "Unknown";
            VehicleType = vehicleType;
            Type = D3M_ObjectType.Vehicle;

            InitialDamage = 0;
            Weight = 1;
            Softness = 1;

            this.X = initX;
            this.Y = initY;
            this.Z = initZ;
            this.RX = initRX;
            this.RY = initRY;
            this.RZ = initRZ;
        }
        public override void CompileObject(ExportedMission exportedMission)
        {
            float angle = RY;
            angle -= 25.5f;
            angle -= 1.0f; // y'know, math is hard
            var a = (Math.PI / 180) * (angle); // convert degrees to radians
            // convert player/character angle to vehicle angle
            a = (a / 2) + 45;
            Vector3 fwd = new Vector3(
                (float)(Math.Cos(0) * Math.Cos(a)), // x

                0, //(float)-Math.Sin(angle), // altitude

                (float)(Math.Cos(0) * Math.Sin(a)) // z
            );
            int flags = 302186496 | (StartUninitialised ? 0 : 1); // TODO: extend this feature
            short stringId = 0;
            if (ObjectName == "" | ObjectName == null) { stringId = (short)exportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)exportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew(ObjectName); }
            exportedMission.LogicData.Actors.Definitions.Add(new ActorDefinition()
            {
                Color = new NodeColor(r, g, b, 255), // gold
                TypeId = 3,
                StringId = stringId,
                ObjectId = exportedMission.Objects.Objects.Count,
                Properties = new List<NodeProperty>
                    {
                        new FloatProperty((float)Weight) {
                            StringId =  (short)exportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWeight")
                        },         // pWeight
                        new FloatProperty((float)Softness) {
                            StringId =  (short)exportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pSoftness")
                        },         // pSoftness
                        new FloatProperty((float)Fragility) {
                            StringId =  (short)exportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pIFragility")
                        },         // pIFragility
                        new FloatProperty(1.0f) {
                            StringId =  (short)exportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pDemoOnlySoftness")
                        },         // pDemoOnlySoftness
                        new IntegerProperty((int)TintId) {
                            StringId =  (short)exportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pTintValue")
                        },          // pTintValue
                        new FlagsProperty((int)flags) {
                            StringId =  (short)exportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }     // pFlags 
                    }
            });
            byte[] bX = BitConverter.GetBytes((float)X);
            byte[] bY = BitConverter.GetBytes((float)Y);
            byte[] bZ = BitConverter.GetBytes((float)Z);
            byte[] bFX = BitConverter.GetBytes((float)fwd.X);
            byte[] bFY = BitConverter.GetBytes((float)fwd.Y);
            byte[] bFZ = BitConverter.GetBytes((float)fwd.Z);
            byte[] bDMG = BitConverter.GetBytes((float)InitialDamage);
            byte[] bDMG1 = BitConverter.GetBytes((float)PartsDamage1);
            byte[] bDMG2 = BitConverter.GetBytes((float)PartsDamage2);
            byte[] bDMG3 = BitConverter.GetBytes((float)PartsDamage3);
            byte[] bDMG4 = BitConverter.GetBytes((float)PartsDamage4);
            byte[] bDMG5 = BitConverter.GetBytes((float)PartsDamage5);
            //byte[] bRX = BitConverter.GetBytes((float)rt.X);
            //byte[] bRY = BitConverter.GetBytes((float)rt.Y);
            //byte[] bRZ = BitConverter.GetBytes((float)rt.Z);
            exportedMission.Objects.Objects.Add(new VehicleObject()
            {
                UID = (int)VehicleType, // vehicle Id
                CreationData = new byte[]
                    {
                        4,0x0C,0x28,0x0, // UID maybe?
                        0x0C,0x0,0x4C, // magic
                        0x0,0x0,0x0,0x0,0x0, // zeros......
                        0x01,0x1C,0,0, // part 2 magic
                        // direction of which the vehicle is facing to (i guess)
                        //0,0,0x80,0x3F,   0,0,0,0,   0,0,0,0,   0,0,0x80,0x3F,
                        0,0,0x0,0x0,   0,0,0,0,   0,0,0,0,   0,0,0x0,0x0, // could be the height of the wheels
                        // pack of zeros (I don't know what they mean)
                        0,0,0,0,0,0,0,0,
                        1, 0x24, 0,0,
                        // vehicle position (X,Y,Z,W=1) here.
                        bX[0],bX[1],bX[2],bX[3],      bY[0],bY[1],bY[2],bY[3],     bZ[0],bZ[1],bZ[2],bZ[3], 0,0,0x80,0x3F,
                        // roll axis..?
                        0,0,0,0,
                        // direction...
                        bFX[0],bFX[1],bFX[2],bFX[3],    bFY[0],bFY[1],bFY[2],bFY[3],     bFZ[0],bFZ[1],bFZ[2],bFZ[3],
                        // more unknown floats stuff, guess what? ZEROS!
                        //0,0,0,0, 0,0,0x80,0x3F,
                        // unknown byte gang
                        5,0x20,0,0,
                        // more zeros.... and 1.0f again.
                        bDMG1[0],bDMG1[1],bDMG1[2],bDMG1[3],    bDMG2[0],bDMG2[1],bDMG2[2],bDMG2[3],     bDMG3[0],bDMG3[1],bDMG3[2],bDMG3[3],   bDMG4[0],bDMG4[1],bDMG4[2],bDMG4[3],
                        // AND..... the end!
                        bDMG5[0],bDMG5[1],bDMG5[2],bDMG5[3],  bDMG[0],bDMG[1],bDMG[2],bDMG[3],    0,0,0,0
                },
                Position = new DSCript.Vector4((float)X, (float)Y, (float)Z, RY)
            });

            int idx = exportedMission.LogicData.Actors.Definitions.Count - 1;
            Definition = exportedMission.LogicData.Actors.Definitions[idx];
            MissionObject = exportedMission.Objects.Objects[Definition.ObjectId];
        }
    }
    
    public class D3M_MissionScript
    {
        protected const byte r = 14;
        protected const byte g = 201;
        protected const byte b = 255;

        public List<D3M_Variable> GlobalVariables;
        public List<D3M_Object> Objects;

        public ExportedMission ExportedMission;
        public MissionSummary MissionSummary;

        public Dictionary<string, NodeDefinition> Labels;

        /// <summary>
        /// @MAIN node in D3M script
        /// </summary>
        public NodeDefinition LogicStart;

        // more logical stuff... damn, what a headache 
        public string PreviousLabel = "NOLABEL";
        public NodeDefinition LastCreatedNode;

        // used for "wait", "if", "else", etc. keywords
        // it's set to null after reached a "end" keyword or change of label
        public NodeDefinition LogicalNodeDefinition; 

        // root for adding "wait", "if", etc. in
        public NodeDefinition RootLogicalNodeDefinition; 

        /// <summary>
        /// Creates a logic node inside a exported mission
        /// </summary>
        /// <param name="exportedMission">The exported mission to add the node definition to</param>
        /// <param name="Type">The node type</param>
        /// <param name="nodeProperties">The node's properties</param>
        /// <returns>The created node definition</returns>
        NodeDefinition CreateLogicNode(ExportedMission exportedMission, byte Type, List<NodeProperty> nodeProperties, string note = "Unknown")
        {
            short stringId = (short)exportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew(note);

            int pWireCollection = ExportedMission.LogicData.WireCollection.WireCollections.Count;
            WireCollection wc = new WireCollection(0);
            ExportedMission.LogicData.WireCollection.WireCollections.Add(wc);

            NodeDefinition nd = new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = Type,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)exportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        }
                    }
            };
            exportedMission.LogicData.Nodes.Definitions.Add(nd);

            nd.Properties.AddRange(nodeProperties);

            int idx = exportedMission.LogicData.Nodes.Definitions.Count - 1;
            return exportedMission.LogicData.Nodes.Definitions[idx];
        }
        WireCollection GetNodeWireCollection(NodeDefinition node)
        {
            WireCollectionProperty wcp = node.Properties[0] as WireCollectionProperty;

            return ExportedMission.LogicData.WireCollection.WireCollections[wcp.Value];
        }
        int FindNodeId(NodeDefinition node)
        {
           for (int id = 0; id < ExportedMission.LogicData.Nodes.Definitions.Count; id++)
            {
                if (ExportedMission.LogicData.Nodes.Definitions[id] == node)
                    return id;
            }
            return -1; // return -1 if not found
        }
        int FindActorId(ActorDefinition node)
        {
            for (int id = 0; id < ExportedMission.LogicData.Actors.Definitions.Count; id++)
            {
                if (ExportedMission.LogicData.Actors.Definitions[id] == node)
                    return id;
            }
            return -1; // return -1 if not found
        }

        protected void CreateBasicMissionData()
        {
            // exported mission logic
            if (ExportedMission == null)
                ExportedMission = new ExportedMission();
            if (ExportedMission.LogicData == null)
                ExportedMission.LogicData = new LogicExportData();
            if (ExportedMission.LogicData.Actors == null)
                ExportedMission.LogicData.Actors = new LogicDataCollection<ActorDefinition>();
            if (ExportedMission.LogicData.ActorSetTable == null)
                ExportedMission.LogicData.ActorSetTable = new ActorSetTableData();
            if (ExportedMission.LogicData.ActorSetTable.Table == null)
                ExportedMission.LogicData.ActorSetTable.Table = new List<ActorSet>();
            if (ExportedMission.LogicData.Nodes == null)
                ExportedMission.LogicData.Nodes = new LogicDataCollection<NodeDefinition>();
            if (ExportedMission.LogicData.StringCollection == null)
                ExportedMission.LogicData.StringCollection = new StringCollectionData();
            if (ExportedMission.LogicData.StringCollection.Strings == null)
                ExportedMission.LogicData.StringCollection.Strings = new List<string>();
            if (ExportedMission.LogicData.WireCollection == null)
                ExportedMission.LogicData.WireCollection = new WireCollectionData();
            if (ExportedMission.LogicData.WireCollection.WireCollections == null)
                ExportedMission.LogicData.WireCollection.WireCollections = new List<WireCollection>();
            if (ExportedMission.Objects == null)
                ExportedMission.Objects = new ExportedMissionObjects();
            if (ExportedMission.Objects.Objects == null)
                ExportedMission.Objects.Objects = new List<MissionObject>();
            if (ExportedMission.MissionInstances == null)
                ExportedMission.MissionInstances = new MissionInstanceData();
            if (ExportedMission.MissionInstances.Instances == null)
                ExportedMission.MissionInstances.Instances = new List<MissionInstance>();
            if (ExportedMission.LogicData.SoundBankTable == null)
                ExportedMission.LogicData.SoundBankTable = new SoundBankTableData();
            if (ExportedMission.LogicData.SoundBankTable.Table == null)
                ExportedMission.LogicData.SoundBankTable.Table = new List<int>();

            // D3M logic
            if (GlobalVariables == null)
                GlobalVariables = new List<D3M_Variable>();
            if (Objects == null)
                Objects = new List<D3M_Object>();
            if (Labels == null)
                Labels = new Dictionary<string, NodeDefinition>();

            if (LogicStart == null) // a.k.a @MAIN label
                LogicStart = CreateLogicNode(ExportedMission, 1, new List<NodeProperty> { }, "MAIN");
            if (MissionSummary == null)
                MissionSummary = new MissionSummary();
        }

        public void CompileFile(string scriptFilePath)
        {
            TextReader txtRead = File.OpenText(scriptFilePath);
            string text = txtRead.ReadToEnd();

            CompileText(text, scriptFilePath);
        }

        public D3M_MissionScript() { }
        public D3M_MissionScript(string scriptFilePath)
        {
            CompileFile(scriptFilePath);
        }

        /// <summary>
        /// Parses the text to <see cref="D3M_Variable"/>
        /// </summary>
        /// <param name="text">The text to be parsed</param>
        /// <returns></returns>
        D3M_Variable ParseTextToVariable(string text)
        {
            // '@' indicates it's a label
            if (text.StartsWith("@"))
            {
                string name = text.Remove(0, 1);
                return new D3M_Variable() { Type = D3M_VariableType.Label, Name = name, LabelName = name, Object = null };
            }

            // '$' indicates it is a pointer or a object
            if (text.StartsWith("$"))
            {
                string name = text.Remove(0, 1);
                D3M_Object ob = null;
                foreach(D3M_Object obj in Objects)
                {
                    if (obj.ObjectName == name)
                    {
                        ob = obj;
                        // return the object in a D3M variable
                        return new D3M_Variable() { Type = D3M_VariableType.Object, Name = name, Object = ob };
                    }
                }
                // return pointer, because it doesn't exist (yet)
                return new D3M_Variable() { Type = D3M_VariableType.Pointer, Name = name, Object = null };
            }
            // test if it matches any global variables name
            foreach(D3M_Variable vari in GlobalVariables)
            {
                if (vari.Name == text)
                    return vari;
            }
            // test if it's a boolean
            if (text.ToLower() == "true")
                return new D3M_Variable() { Type = D3M_VariableType.Boolean, Boolean = true };
            if (text.ToLower() == "false")
                return new D3M_Variable() { Type = D3M_VariableType.Boolean, Boolean = false };
            // test if it's text/string
            if (text.StartsWith("\"") || text.StartsWith("'"))
            {
                string str = text.Split(text[0])[1];
                return new D3M_Variable { Type = D3M_VariableType.Text, Text = str };
            }
            // test if it's a single/double number
            string numberTestText = text.Replace(',', '.');
            try
            {
                if (numberTestText.Contains('.'))
                {
                    double number = Convert.ToDouble(text);
                    return new D3M_Variable() { Type = D3M_VariableType.Number, Number = number };
                }
            }
            catch (Exception ex) { } // no need to do something with the exception
            // test if it's a hexadecimal number
            try
            {
                if (text.StartsWith("0x"))
                {
                    string parseNum = text.Replace("0x", ""); // remove 0x for hexadecimals
                    int bytes = parseNum.Length / 2;
                    int bits = bytes * 8;
                    double number = 0;
                    D3M_Variable result = new D3M_Variable() { Type = D3M_VariableType.Number };
                    try
                    {
                        switch (bits)
                        {
                            case 8:
                                number = (int)Convert.ToByte(parseNum);
                                break;
                            case 16:
                                number = Convert.ToInt16(parseNum);
                                break;
                            case 32:
                                number = Convert.ToInt32(parseNum);
                                break;
                            case 64:
                                number = Convert.ToInt64(parseNum);
                                break;
                        }
                    }
                    // bruh
                    catch (Exception) { }
                    result.Number = number;
                    return result;
                }
            }
            catch (Exception ex){} // no need to do something with the exception
            // test if it's a integer number
            try
            {
                if (text.Contains("1") || text.Contains("2") || text.Contains("3") || text.Contains("4") || text.Contains("5") || text.Contains("6") || text.Contains("7") || text.Contains("8") || text.Contains("9") || text.Contains("0"))
                {
                    string parseNum = text.Replace("0x", ""); // remove 0x for hexadecimals
                    int number = Convert.ToInt32(parseNum);
                    return new D3M_Variable() { Type = D3M_VariableType.Number, Number = number };
                }
            }
            catch (Exception ex){} // no need to do something with the exception

            if (text.ToLower() == "null" || text.ToLower() == "nullptr")
                return new D3M_Variable() { Type = D3M_VariableType.Null }; // returns null var

            return new D3M_Variable() { Type = D3M_VariableType.Undefined }; // otherwise return undefined for keywords, parameter names, etc.
        }

        public NodeDefinition GetLabelNodeDefinition(string labelName, bool autoCreateIfNull = true)
        {
            try
            {
                if (labelName != "MAIN")
                    return Labels[labelName];
                else
                    return LogicStart;
            }
            // I want no problems, just return null :)
            catch(Exception ex)
            {
                if (!autoCreateIfNull)
                    return null;
                else
                {
                    NodeDefinition node = CreateLogicNode(ExportedMission, 8, new List<NodeProperty> { }, labelName);
                    Labels.Add(labelName, node);
                    return node;
                }
            }
        }

        public void CompileText(string text, string sourceScriptName = null)
        {
            // make it more understandable even with ';' in them
            // (makes sense right?)
            string understandableCode = text.Replace('\n', ';');
            string[] lines = understandableCode.Split(';');

            // create if null
            CreateBasicMissionData();

            string currentLabel = "NOLABEL";
            // compile each line
            for (int lineId = 0; lineId < lines.Length; lineId++)
            {
                // "optimized" way to detect if it's just a comment or empty and skip it
                if (string.IsNullOrWhiteSpace(lines[lineId]))
                    continue;
                if ((lines[lineId][0] == '/' && lines[lineId][1] == '/'))
                    continue;

                // gets the line without comments
                string line = lines[lineId].Split(new[] { "//" }, StringSplitOptions.None)[0].Trim();

                // remove spaces from the start of the line
                int spaceEndIndex = -1;
                for(int id = 0; id < line.Length; id++)
                {
                    char c = line[id];
                    if (c != ' ' || c != '\x09')
                    {
                        spaceEndIndex = id;
                        break;
                    }
                }
                // I kinda realized there could be a chance to break the line (like literally) but it doesn't :D
                line = line.Substring(spaceEndIndex, line.Length - spaceEndIndex);

                // test if the line is a label definition
                if (line.StartsWith(":"))
                {
                    RootLogicalNodeDefinition = null;
                    LogicalNodeDefinition = null;
                    PreviousLabel = currentLabel;
                    currentLabel = line.Remove(0, 1);
                    if (string.IsNullOrWhiteSpace(currentLabel))
                        throw new D3M_CompileException("Invalid label name, label names must not be empty or contain spaces", sourceScriptName, lineId);

                    // automatically create the label node definition
                    if (GetLabelNodeDefinition(currentLabel) == null && currentLabel != "MAIN")
                        Labels[currentLabel] = CreateLogicNode(ExportedMission, 8, new List<NodeProperty> { }, currentLabel);

                    continue;
                }
                // test the line if it's a opcode
                if (line.Length >= 5)
                {
                    if (line[4] == ':') // if it has 2 dots like this in the 4th character then it is a opcode
                    {
                        // TODO: compile line using CompileOpcode()
                        string opcodeStr = line.Substring(0, 4);
                        short opcode = 0;
                        try
                        {
                            opcode = Convert.ToInt16(opcodeStr);
                        }
                        catch (Exception)
                        {
                            try
                            {
                                string opcodeStr2 = line.Substring(2, 3);
                                opcode = Convert.ToInt16(opcodeStr2);
                            }
                            catch (Exception) { }
                        }

                        // TODO: parse parameters
                        List<D3M_Variable> parameters = new List<D3M_Variable>();
                        string[] divSpaces = line.Split(' ');
                        // skip 2 text entries divided by spaces, why? because the first one is the opcode definition and node description (ex.: create_character)
                        for (int paramId = 2; paramId < divSpaces.Length; paramId++)
                        {
                            D3M_Variable var = ParseTextToVariable(divSpaces[paramId]);

                            // isn't a keyword or it's not a parameter name
                            if (var.Type != D3M_VariableType.Undefined)
                                parameters.Add(var);
                        }

                        CompileOpcode(opcode, lineId, currentLabel, parameters);
                        continue;
                    }
                    else if (line[4] == '#') 
                    {
                        // TODO: compile line using CompileOpcode()
                        string opcodeStr = line.Substring(0, 4);
                        short opcode = Convert.ToInt16(opcodeStr);

                        // TODO: parse parameters
                        List<D3M_Variable> parameters = new List<D3M_Variable>();
                        List<string> parametersNames = new List<string>();
                        string[] divSpaces = line.Split(' ');
                        // skip 2 text entries divided by spaces, why? because the first one is the opcode definition and node description (ex.: create_character)
                        for (int paramId = 2; paramId < divSpaces.Length; paramId++)
                        {
                            D3M_Variable var = ParseTextToVariable(divSpaces[paramId]);

                            // isn't a keyword or it's not a parameter name
                            if (var.Type != D3M_VariableType.Undefined)
                                parameters.Add(var);
                            else
                                parametersNames.Add(divSpaces[paramId]);
                        }

                        CompilePureOpcode(opcode, lineId, currentLabel, parametersNames, parameters);
                        continue;
                    }
                }
                if (line.StartsWith("DEFINE"))
                {
                    string[] divSpaces = line.Split(' '); // parameters are after index 0
                    string defType = divSpaces[0].Split('_')[1];
                    switch (defType)
                    {
                        case "SPOOLPOSITION":
                            {
                                if (divSpaces.Length - 1 < 2)
                                    //throw new D3M_CompileException($"Not enough parameters for {divSpaces[0]} (expected 2)", sourceScriptName, lineId);
                                    // actually, I think saying the syntax would be better for newbies ...
                                    throw new D3M_CompileException($"Invalid syntax, example syntax: DEFINE_SPOOLPOSITION <float number: X> <float number: Y>", sourceScriptName, lineId);

                                MissionSummary.X = Convert.ToSingle(divSpaces[1]);
                                MissionSummary.Y = Convert.ToSingle(divSpaces[2]);
                            }
                            break;
                        case "CITY":
                            {
                                if (divSpaces.Length - 1 < 1)
                                    //throw new D3M_CompileException($"Not enough parameters for {divSpaces[0]} (expected 2)", sourceScriptName, lineId);
                                    // actually, I think saying the syntax would be better for newbies ...
                                    throw new D3M_CompileException($"Invalid syntax, example syntax: DEFINE_SPOOLPOSITION <float number: X> <float number: Y>", sourceScriptName, lineId);

                                string cityType = divSpaces[1];
                                MissionCityType city = MissionCityType.Miami_Day;
                                switch (cityType)
                                {
                                    case "Miami_Day":
                                        city = MissionCityType.Miami_Day;
                                        break;
                                    case "Miami_Night":
                                        city = MissionCityType.Miami_Night;
                                        break;
                                    case "Nice_Day":
                                        city = MissionCityType.Nice_Day;
                                        break;
                                    case "Nice_Night":
                                        city = MissionCityType.Nice_Night;
                                        break;
                                    case "Istanbul_Day":
                                        city = MissionCityType.Istanbul_Day;
                                        break;
                                    case "Istanbul_Night":
                                        city = MissionCityType.Istanbul_Night;
                                        break;
                                }
                                MissionSummary.CityType = city;
                            }
                            break;
                        case "SKYID":
                        case "MOODID":
                            {
                                if (divSpaces.Length - 1 < 1)
                                    //throw new D3M_CompileException($"Not enough parameters for {divSpaces[0]} (expected 2)", sourceScriptName, lineId);
                                    // actually, I think saying the syntax would be better for newbies ...
                                    throw new D3M_CompileException($"Invalid syntax, example syntax: DEFINE_MOODID <positive number: mood ID>", sourceScriptName, lineId);

                                MissionSummary.MoodId = Convert.ToInt16(divSpaces[1]);
                            }
                            break;
                        case "COUNTER":
                            {
                                if (divSpaces.Length - 1 < 3)
                                    //throw new D3M_CompileException($"Not enough parameters for {divSpaces[0]} (expected 2)", sourceScriptName, lineId);
                                    // actually, I think saying the syntax would be better for newbies ...
                                    throw new D3M_CompileException($"Invalid syntax, example syntax: DEFINE_COUNTER <text: name> <number: index> <number: start value>", sourceScriptName, lineId);

                                D3M_Variable vari = ParseTextToVariable(divSpaces[3]);
                                vari.Type = D3M_VariableType.Counter;
                                GlobalVariables.Add(vari);

                                GetNodeWireCollection(LogicStart).Wires.Add(new WireNode()
                                {
                                    NodeId = (short)FindNodeId(CreateLogicNode(ExportedMission, 17, new List<NodeProperty>()
                                    {
                                           new StringProperty((short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew(divSpaces[1])) {
                                             StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pName")
                                           },
                                           new IntegerProperty(Convert.ToInt32(divSpaces[3])) {
                                             StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pValue")
                                           },
                                           new IntegerProperty(Convert.ToInt32(divSpaces[2])) {
                                             StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCounterIndex")
                                           },
                                           // equal operator
                                           new EnumProperty(1) {
                                             StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAction")
                                           },
                                           new FlagsProperty(1) {
                                             StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                                           }
                                    })),
                                    OpCode = 17,
                                    WireType = 1
                                });
                            }
                            break;
                        case "CONSTANT":
                            {
                                if (divSpaces.Length - 1 < 2)
                                    //throw new D3M_CompileException($"Not enough parameters for {divSpaces[0]} (expected 2)", sourceScriptName, lineId);
                                    // actually, I think saying the syntax would be better for newbies ...
                                    throw new D3M_CompileException($"Invalid syntax, example syntax: DEFINE_CONSTANT <text: name> <any: object>", sourceScriptName, lineId);

                                D3M_Variable vari = ParseTextToVariable(divSpaces[2]);
                                vari.Name = divSpaces[1];
                                vari.ReadOnly = true;

                                GlobalVariables.Add(vari);
                            }
                            break;
                        default:
                            throw new D3M_CompileException($"Expected a valid 'DEFINE' expression", sourceScriptName, lineId);
                    }
                    continue;
                }
                // check if it's a math expression 
                // NOTE: Aww man, I was kinda sleepy here, couldn't think any more on how to code this (bruh)
                if (line.Contains("="))
                {
                    // TODO: finish it
                    /*
                    string[] divSpaces = line.Split(' ');
                    string left = divSpaces[1];
                    string right = divSpaces[3];
                    if (left == "=") // in case it's just a '='
                        throw new D3M_CompileException("Invalid expression - left entry is missing or invalid", sourceScriptName, lineId);
                    if (right == null)
                        throw new D3M_CompileException("Invalid expression - right entry is missing or invalid", sourceScriptName, lineId);

                    D3M_Variable counter = null;
                    foreach(D3M_Variable var in GlobalVariables)
                    {
                        if (var.Name == left)
                            counter = var;
                    }

                    if (right.Contains("+"))
                    {
                        CreateLogicNode(ExportedMission, 17, new List<NodeProperty>() {
                            new IntegerProperty(Convert.ToInt32(right)) {
                               StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pValue")
                           },
                           new IntegerProperty(counter.CounterIndex) {
                               StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCounterIndex")
                           },
                           new EnumProperty(2) {
                               StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAction")
                           },
                           new FlagsProperty(0) {
                               StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                           }
                        }, line);
                    }
                    */
                    continue;
                }
                string[] kwdivSpaces = line.Split(' ');
                string keyword = kwdivSpaces[0];
                // check if it's a keyword
                switch (keyword)
                {
                    case "wait":
                        {
                            // flags should be 11 (binary, 3 in decimal) for repeat enabled 
                            float timeToWait = (float)Convert.ToUInt16(kwdivSpaces[1]) / 1000;
                            LastCreatedNode = CreateLogicNode(ExportedMission, 3, new List<NodeProperty>
                            {
                               new FloatProperty(timeToWait) {
                                  StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pInterval")
                               },         // pInterval
                               new FlagsProperty(3) {
                                  StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                               }          // pFlags
                            }, "Wait interval");
                            NodeDefinition addToNode = GetLabelNodeDefinition(currentLabel);

                            if (RootLogicalNodeDefinition != null)
                            {
                                addToNode = RootLogicalNodeDefinition;
                            }

                            GetNodeWireCollection(addToNode).Wires.Add(new WireNode() { WireType = (byte)(addToNode == LogicStart ? 1 : 11), OpCode = LastCreatedNode.TypeId, NodeId = (short)FindNodeId(LastCreatedNode) });
                            LogicalNodeDefinition = LastCreatedNode;
                        }
                        break;

                    case "onsuccess_enable":
                        {
                            NodeDefinition label = GetLabelNodeDefinition(kwdivSpaces[1].Replace("@", ""));
                            if (LogicalNodeDefinition != null && label != null)
                            {
                                GetNodeWireCollection(LogicalNodeDefinition).Wires.Add(new WireNode() { WireType = (byte)(label == LogicStart ? 1 : 11), OpCode = label.TypeId, NodeId = (short)FindNodeId(label) });
                            }
                        }
                        break;
                    case "onsuccess_disable":
                        {
                            NodeDefinition label = GetLabelNodeDefinition(kwdivSpaces[1].Replace("@", ""));
                            if (LogicalNodeDefinition != null && label != null)
                            {
                                GetNodeWireCollection(LogicalNodeDefinition).Wires.Add(new WireNode() { WireType = (byte)(label == LogicStart ? 2 : 12), OpCode = label.TypeId, NodeId = (short)FindNodeId(label) });
                            }
                        }
                        break;
                    case "onfailure_enable":
                        {
                            NodeDefinition label = GetLabelNodeDefinition(kwdivSpaces[1].Replace("@", ""));
                            if (LogicalNodeDefinition != null && label != null)
                            {
                                GetNodeWireCollection(LogicalNodeDefinition).Wires.Add(new WireNode() { WireType = 3, OpCode = label.TypeId, NodeId = (short)FindNodeId(label) });
                            }
                        }
                        break;
                    case "onfailure_disable":
                        {
                            NodeDefinition label = GetLabelNodeDefinition(kwdivSpaces[1].Replace("@", ""));
                            if (LogicalNodeDefinition != null && label != null)
                            {
                                GetNodeWireCollection(LogicalNodeDefinition).Wires.Add(new WireNode() { WireType = 4, OpCode = label.TypeId, NodeId = (short)FindNodeId(label) });
                            }
                        }
                        break;
                    case "onevent":
                    case "on_event":
                        {
                            D3M_Variable secondaryObj = null;
                            if (kwdivSpaces.Length > 3)
                                secondaryObj = ParseTextToVariable(kwdivSpaces[3]);
                            LastCreatedNode = CompileEvent(ParseTextToVariable(kwdivSpaces[1]), secondaryObj, kwdivSpaces[2], currentLabel, lineId, sourceScriptName);
                        }
                        break;
                    case "onglobalevent":
                    case "on_global_event":
                        {
                            D3M_Variable secondaryObj = null;
                            if (kwdivSpaces.Length > 3)
                                secondaryObj = ParseTextToVariable(kwdivSpaces[3]);
                            LastCreatedNode = CompileEvent(ParseTextToVariable(kwdivSpaces[1]), secondaryObj, kwdivSpaces[2], currentLabel, lineId, sourceScriptName, true);
                        }
                        break;
                    case "randomly_do":
                        {
                            LogicalNodeDefinition = CreateLogicNode(ExportedMission, 8, new List<NodeProperty> { }, "Randomly do");

                            NodeDefinition addToNode = GetLabelNodeDefinition(currentLabel);

                            if (RootLogicalNodeDefinition != null)
                            {
                                addToNode = RootLogicalNodeDefinition;
                            }

                            if (addToNode != null)
                            {
                                GetNodeWireCollection(addToNode).Wires.Add(new WireNode() { WireType = (byte)(addToNode == LogicStart ? 2 : 12), OpCode = LogicalNodeDefinition.TypeId, NodeId = (short)FindNodeId(LogicalNodeDefinition) });
                            }
                        }
                        break;
                    case "or_randomly_do":
                        {
                            NodeDefinition actualNodeToDo = null;
                            if (kwdivSpaces.Length > 1)
                            {
                                NodeDefinition targetLabel = GetLabelNodeDefinition(kwdivSpaces[1].Replace("@", ""));

                                if (!kwdivSpaces[1].StartsWith("@"))
                                {
                                    throw new D3M_CompileException("Not a label!", sourceScriptName, lineId);
                                }

                                if (targetLabel != null && kwdivSpaces[1].StartsWith("@"))
                                    actualNodeToDo = targetLabel;
                            }

                            if (actualNodeToDo != null)
                            {
                                actualNodeToDo = CreateLogicNode(ExportedMission, 8, new List<NodeProperty> { });
                            }

                            if (LogicalNodeDefinition != null && actualNodeToDo != null)
                            {
                                GetNodeWireCollection(LogicalNodeDefinition).Wires.Add(new WireNode() { WireType = (byte)(actualNodeToDo == LogicStart ? 1 : 11), OpCode = actualNodeToDo.TypeId, NodeId = (short)FindNodeId(actualNodeToDo) });
                            }

                            if (actualNodeToDo != null)
                            {
                                LastCreatedNode = actualNodeToDo;
                                RootLogicalNodeDefinition = LastCreatedNode;
                            }
                        }
                        break;
                    case "end":
                        {
                            // clear logical node definition
                            LogicalNodeDefinition = null;
                            RootLogicalNodeDefinition = null;
                        }
                        break;
                    default:
                        throw new D3M_CompileException("Expected a expression", sourceScriptName, lineId);
                }
            }
        }

        /// <summary>
        /// Compiles a event from "on_event"/"onevent" keyword
        /// </summary>
        /// <param name="obj">The source object</param>
        /// <param name="eventName">Name of the event</param>
        /// <returns></returns>
        public NodeDefinition CompileEvent(D3M_Variable obj, D3M_Variable secondaryObj, string eventName, string labelName, int lineId, string sourceScriptName = "", bool globalEvent = false)
        {
            NodeDefinition node = null;

            switch(obj.Object.Definition.TypeId)
            {
                // character
                case 2:
                    {
                        int eventType = -1;
                        float value = 0; // usually 0 for events that don't have a secondary object specified
                        int flags = 0;

                        switch(eventName)
                        {
                            case "IsChased":
                                {
                                    eventType = 9;
                                }
                                break;
                            case "IsArrested":
                                {
                                    eventType = 3;
                                }
                                break;
                            case "Death":
                                {
                                    eventType = 1;
                                }
                                break;
                            case "EnterExitVehicle":
                                {
                                    eventType = 4;
                                    flags = 0;
                                }
                                break;
                            default:
                                throw new D3M_CompileException($"Unknown event \"{eventName}\", check for any typos or look for references", sourceScriptName, lineId);
                        }

                        node = CreateLogicNode(ExportedMission, 19, new List<NodeProperty>()
                        {
                           new ActorProperty(FindActorId(obj.Object.Definition)) {
                               StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCharacter")
                           },         // pCharacter
                           new ActorProperty(secondaryObj == null ? -1 : FindActorId(secondaryObj.Object.Definition)) {
                               StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pObject")
                           },         // pObject
                           new EnumProperty(eventType) {
                               StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWatchover")
                           },         // pWatchover
                           new FloatProperty(value) {
                               StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pValue")
                           },         // pValue
                           new FlagsProperty(flags) {
                               StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                           }
                        });
                        NodeDefinition addToNode = GetLabelNodeDefinition(labelName);

                        if (RootLogicalNodeDefinition != null)
                        {
                            addToNode = RootLogicalNodeDefinition;
                        }

                        GetNodeWireCollection(addToNode).Wires.Add(new WireNode() { WireType = (byte)(addToNode == LogicStart ? 1 : 11), OpCode = node.TypeId, NodeId = (short)FindNodeId(node) });
                        LogicalNodeDefinition = node;
                    }
                    break;
                // vehicle
                case 3:
                    {
                        int eventType = -1;
                        float value = 0; // usually 0 for events that don't have a secondary object specified
                        int flags = 5;

                        switch (eventName)
                        {
                            case "Damaged":
                                {
                                    eventType = 1;
                                    value = 0.0001f;
                                }
                                break;
                            case "Wrecked":
                                {
                                    eventType = 1;
                                    value = 1.0f;
                                }
                                break;
                            case "ThrownInWater":
                                {
                                    eventType = 7;
                                }
                                break;
                            default:
                                throw new D3M_CompileException($"Unknown event \"{eventName}\", check for any typos or look for references", sourceScriptName, lineId);
                        }

                        node = CreateLogicNode(ExportedMission, 16, new List<NodeProperty>()
                        {
                           new ActorProperty(FindActorId(obj.Object.Definition)) {
                               StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pVehicle")
                           },         // pVehicle
                           new EnumProperty(eventType) {
                               StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWatchover")
                           },         // pWatchover
                           new FloatProperty(value) {
                               StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pValue")
                           },         // pValue
                           new FlagsProperty(flags) {
                               StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                           }
                        });
                        NodeDefinition label = GetLabelNodeDefinition(labelName);
                        GetNodeWireCollection(label).Wires.Add(new WireNode() { WireType = (byte)(label == LogicStart ? 1 : 11), OpCode = node.TypeId, NodeId = (short)FindNodeId(node) });
                        LogicalNodeDefinition = node;
                    }
                    break;
            }

            return node;
        }

        /// <summary>
        /// Compiles a pure opcode with custom parameter names and types,
        /// 
        /// String types:
        /// INT_(propertyName)
        /// FLOAT_(propertyName)
        /// ACTOR_(propertyName)
        /// STRING_(propertyName)
        /// SOUNDBANK_(propertyName)
        /// PERSONALITY_(propertyName)
        /// </summary>
        /// <param name="type">Type of the opcode</param>
        /// <param name="sourceLine">Source line number</param>
        /// <param name="LabelName">Label name to be created in</param>
        /// <param name="parameterNames">Name for parameters</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public NodeDefinition CompilePureOpcode(int type, int sourceLine, string LabelName, List<string> parameterNames, List<D3M_Variable> parameters)
        {
            // create basic to logic if null
            CreateBasicMissionData();

            NodeDefinition NodeToAddTo = LogicStart;
            byte enable_WireType = 1;
            byte disable_WireType = 2;
            if (LabelName != "MAIN")
            {
                // switch the wire type on disable & enable for group disable & group enable
                enable_WireType += 10;
                disable_WireType += 10;
                // creates GroupBroadcast
                if (GetLabelNodeDefinition(LabelName) == null)
                {
                    NodeToAddTo = CreateLogicNode(ExportedMission, 8, new List<NodeProperty> { }, LabelName);
                    Labels.Add(LabelName, NodeToAddTo);
                }
                else
                    NodeToAddTo = GetLabelNodeDefinition(LabelName);
            }
            if (LogicalNodeDefinition != null)
                NodeToAddTo = LogicalNodeDefinition;

            List<NodeProperty> properties = new List<NodeProperty>();
            for (int id = 0; id < parameters.Count; id++)
            {
                D3M_Variable var = parameters[id];
                string paramName = parameterNames[id];
                string[] splitName = paramName.Split('_');

                string typeName = splitName[0];
                string name = splitName[1];

                NodeProperty prop = null;
                if (typeName == "INT")
                {
                    prop = new IntegerProperty((int)var.Number);
                }
                else if(typeName == "FLOAT")
                {
                    prop = new FloatProperty((float)var.Number);
                }
                else if (typeName == "ACTOR")
                {
                    if (var.Object != null)
                    {
                        int actorId = ExportedMission.LogicData.Actors.Definitions.Count;
                        var.Object.CompileObject(ExportedMission);
                        prop = new ActorProperty(actorId);
                    }
                }

                if (prop != null)
                {
                    properties.Add(prop);
                    prop.StringId = (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew(name);
                }
            }

            NodeDefinition nodeDefinition = CreateLogicNode(ExportedMission, (byte)type, properties);

            if (nodeDefinition != null)
                LastCreatedNode = nodeDefinition;
            return nodeDefinition;
        }

        /// <summary>
        /// Compiles a opcode 
        /// </summary>
        /// <param name="type">Integer that is the type of the opcode to be processed</param>
        /// <param name="sourceLine">Source line from where the opcode is being processed</param>
        /// <param name="LabelName">Source label name from where the opcode is being prcoessed</param>
        /// <param name="parameters">Input parameters</param>
        public NodeDefinition CompileOpcode(int type, int sourceLine, string LabelName, List<D3M_Variable> parameters)
        {
            // create basic to logic if null
            CreateBasicMissionData();

            NodeDefinition NodeToAddTo = LogicStart;
            byte enable_WireType = 1;
            byte disable_WireType = 2;
            if (LabelName != "MAIN")
            {
                // switch the wire type on disable & enable for group disable & group enable
                enable_WireType += 10;
                disable_WireType += 10;
                // creates GroupBroadcast
                if (GetLabelNodeDefinition(LabelName) == null)
                {
                    NodeToAddTo = CreateLogicNode(ExportedMission, 8, new List<NodeProperty> { });
                    Labels.Add(LabelName, NodeToAddTo);
                }
                else
                    NodeToAddTo = GetLabelNodeDefinition(LabelName);
            }
            if (LogicalNodeDefinition != null)
                NodeToAddTo = LogicalNodeDefinition;

            NodeDefinition nodeDefinition = null;
            switch (type)
            {
                case (int)D3M_OpcodeType.DebugText:
                    if (parameters.Count < 1)
                        throw new D3M_CompileException("Not enough parameters!", sourceLine);

                    short stringId_debugtext = (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew(parameters[0].Text);

                    nodeDefinition = CreateLogicNode(ExportedMission, 2, new List<NodeProperty>
                    {
                        new StringProperty(stringId_debugtext) {
                            StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMessage")
                        },         // pInterval
                        new FlagsProperty(1) {
                            StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }          // pFlags
                    });
                    GetNodeWireCollection(NodeToAddTo).Wires.Add(new WireNode() { WireType = enable_WireType, OpCode = nodeDefinition.TypeId, NodeId = (short)FindNodeId(nodeDefinition) });
                    break;
                case (int)D3M_OpcodeType.MissionComplete:
                    bool noDelay = true;

                    if (parameters.Count > 0)
                        noDelay = parameters[0].Boolean;

                    nodeDefinition = CreateLogicNode(ExportedMission, 5, new List<NodeProperty>
                    {
                        new BooleanProperty(noDelay) {
                            StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pNoDelay")
                        }
                    });
                    GetNodeWireCollection(NodeToAddTo).Wires.Add(new WireNode() { WireType = enable_WireType, OpCode = nodeDefinition.TypeId, NodeId = (short)FindNodeId(nodeDefinition) });
                    break;
                case (int)D3M_OpcodeType.MissionFailed:
                    if (parameters.Count < 2)
                        throw new D3M_CompileException("Not enough parameters!", sourceLine);

                    nodeDefinition = CreateLogicNode(ExportedMission, 6, new List<NodeProperty>
                    {
                         new FloatProperty((float)parameters[1].Number) {
                            StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFailDelay")
                        },
                        new LocalisedStringProperty((int)parameters[0].Number) {
                            StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMessage")
                        },
                        new FlagsProperty(1) {
                            StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }
                    });
                    GetNodeWireCollection(NodeToAddTo).Wires.Add(new WireNode() { WireType = enable_WireType, OpCode = nodeDefinition.TypeId, NodeId = (short)FindNodeId(nodeDefinition) });
                    break;
                case (int)D3M_OpcodeType.CreateVehicle: // create character
                    if (parameters.Count < 6)
                        throw new D3M_CompileException("Not enough parameters!", sourceLine);

                    D3M_Variable carpointer = parameters[0];
                    uint model = (uint)parameters[1].Number;
                    uint colorID = (uint)parameters[2].Number;
                    float carx = (float)parameters[3].Number;
                    float cary = (float)parameters[4].Number;
                    float carz = (float)parameters[5].Number;
                    float carangle = 0;
                    float cardamage = 0;
                    float carsoftness = 1;
                    float carweight = 1;
                    float carfragility = 1;

                    float optionalCarDamage1 = 0;
                    float optionalCarDamage2 = 0;
                    float optionalCarDamage3 = 0;
                    float optionalCarDamage4 = 0;
                    float optionalCarDamage5 = 0;

                    // optional parameters
                    if (parameters.Count >= 7)
                        carangle = (float)parameters[6].Number;
                    if (parameters.Count >= 8)
                        cardamage = (float)parameters[7].Number;
                    if (parameters.Count >= 9)
                        carsoftness = (float)parameters[8].Number;
                    if (parameters.Count >= 10)
                        carweight = (int)parameters[9].Number;
                    if (parameters.Count >= 11)
                        carfragility = (float)parameters[10].Number;
                    if (parameters.Count >= 12)
                        optionalCarDamage1 = (float)parameters[11].Number;
                    if (parameters.Count >= 13)
                        optionalCarDamage2 = (float)parameters[12].Number;
                    if (parameters.Count >= 14)
                        optionalCarDamage3 = (float)parameters[13].Number;
                    if (parameters.Count >= 15)
                        optionalCarDamage4 = (float)parameters[14].Number;
                    if (parameters.Count >= 16)
                        optionalCarDamage5 = (float)parameters[15].Number;

                    D3M_Vehicle vehicle = new D3M_Vehicle(model, carx, cary, carz, 0, carangle, 0);
                    // TODO: make ObjectName same as variable name
                    vehicle.ObjectName = carpointer.Name;
                    vehicle.X = carx;
                    vehicle.Y = cary;
                    vehicle.Z = carz;

                    vehicle.RY = carangle;

                    vehicle.PartsDamage1 = optionalCarDamage1;
                    vehicle.PartsDamage2 = optionalCarDamage2;
                    vehicle.PartsDamage3 = optionalCarDamage3;
                    vehicle.PartsDamage4 = optionalCarDamage4;
                    vehicle.PartsDamage5 = optionalCarDamage5;

                    if (NodeToAddTo == LogicStart)
                    {
                        vehicle.StartUninitialised = false;
                    }

                    // compile it
                    vehicle.CompileObject(ExportedMission);

                    // add it to objects list (very important!)
                    Objects.Add(vehicle);

                    // convert the pointer to a object and add it to global variables
                    carpointer.Object = vehicle;
                    carpointer.Type = D3M_VariableType.Object; // set the type to object
                    GlobalVariables.Add(carpointer); // add it to global vars

                    int idx = -1;
                    for (int id = 0; id < ExportedMission.LogicData.Actors.Definitions.Count; id++)
                    {
                        if (ExportedMission.LogicData.Actors.Definitions[id] == vehicle.Definition)
                            idx = id;
                    }

                    if (vehicle.StartUninitialised)
                    {
                        nodeDefinition = CreateLogicNode(ExportedMission, 101, new List<NodeProperty>
                    {
                        new ActorProperty(idx) {
                            StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pActor")
                        },         // pActor
                        new IntegerProperty(1) {
                            StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pActivity")
                        },         // pActivity
                        new FlagsProperty(0) {
                            StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }
                    });
                        GetNodeWireCollection(NodeToAddTo).Wires.Add(new WireNode() { WireType = enable_WireType, OpCode = nodeDefinition.TypeId, NodeId = (short)FindNodeId(nodeDefinition) });
                    }
                    break;
                case (int)D3M_OpcodeType.CreateCharacter: // create character
                    if (parameters.Count < 6)
                        throw new D3M_CompileException("Not enough parameters!", sourceLine);

                    D3M_Variable pointer = parameters[0];
                    int role = (int)parameters[1].Number;
                    uint skin = (uint)parameters[2].Number;
                    float x = (float)parameters[3].Number;
                    float y = (float)parameters[4].Number;
                    float z = (float)parameters[5].Number;
                    float angle = 0;
                    float health = 1.0f;
                    float felony = 0.0f;
                    int weapon = 0;
                    float vulnerability = 1.0f;
                    string personality = "";
                    int personalityIdx = -1;

                    // optional parameters
                    if (parameters.Count >= 7)
                        angle = (float)parameters[6].Number;
                    if (parameters.Count >= 8)
                        health = (float)parameters[7].Number;
                    if (parameters.Count >= 9)
                        felony = (float)parameters[8].Number;
                    if (parameters.Count >= 10)
                        weapon = (int)parameters[9].Number;
                    if (parameters.Count >= 11)
                        vulnerability = (float)parameters[10].Number;
                    if (parameters.Count >= 12)
                        personality = parameters[11].Text;
                    if (parameters.Count >= 13)
                        personalityIdx = (int)parameters[12].Number;

                    D3M_Object character = new D3M_Character(skin, role, health, felony, weapon, vulnerability, personality, personalityIdx);
                    // TODO: make ObjectName same as variable name
                    character.ObjectName = pointer.Name;
                    character.X = x;
                    character.Y = y;
                    character.Z = z;

                    character.RY = angle;

                    if (NodeToAddTo == LogicStart)
                    {
                        character.StartUninitialised = false;
                    }

                    // compile it
                    character.CompileObject(ExportedMission);

                    // add it to objects list (very important!)
                    Objects.Add(character);

                    // convert the pointer to a object and add it to global variables
                    pointer.Object = character;
                    pointer.Type = D3M_VariableType.Object; // set the type to object
                    GlobalVariables.Add(pointer); // add it to global vars

                    int caridx = -1;
                    for (int id = 0; id < ExportedMission.LogicData.Actors.Definitions.Count; id++)
                    {
                        if (ExportedMission.LogicData.Actors.Definitions[id] == character.Definition)
                            caridx = id;
                    }

                    if (character.StartUninitialised)
                    {
                        nodeDefinition = CreateLogicNode(ExportedMission, 101, new List<NodeProperty>
                    {
                        new ActorProperty(caridx) {
                            StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pActor")
                        },         // pActor
                        new IntegerProperty(1) {
                            StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pActivity")
                        },         // pActivity
                        new FlagsProperty(0) {
                            StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }
                    });
                        GetNodeWireCollection(NodeToAddTo).Wires.Add(new WireNode() { WireType = enable_WireType, OpCode = nodeDefinition.TypeId, NodeId = (short)FindNodeId(nodeDefinition) });
                    }
                    break;
                case (int)D3M_OpcodeType.MusicControl:
                    if (parameters.Count < 1)
                        throw new D3M_CompileException("Not enough parameters!", sourceLine);

                    nodeDefinition = CreateLogicNode(ExportedMission, 118, new List<NodeProperty>
                    {
                        new EnumProperty(Convert.ToInt32(parameters[0].Number)) {
                            StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMusicType")
                        },         // pMusicType
                        new FlagsProperty(1) {
                            StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }          // pFlags
                    });
                    GetNodeWireCollection(NodeToAddTo).Wires.Add(new WireNode() { WireType = enable_WireType, OpCode = nodeDefinition.TypeId, NodeId = (short)FindNodeId(nodeDefinition) });
                    break;
                case (int)D3M_OpcodeType.InstantiateActor:
                        if (parameters.Count < 1)
                            throw new D3M_CompileException("Not enough parameters!", sourceLine);

                        int instIdx = -1;
                        for (int id = 0; id < ExportedMission.LogicData.Actors.Definitions.Count; id++)
                        {
                            if (ExportedMission.LogicData.Actors.Definitions[id] == parameters[0].Object.Definition)
                                instIdx = id;
                        }

                        nodeDefinition = CreateLogicNode(ExportedMission, 101, new List<NodeProperty>
                        {
                        new ActorProperty(instIdx) {
                            StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pActor")
                        },         // pActor
                        new IntegerProperty(1) {
                            StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pActivity")
                        },         // pActivity
                        new FlagsProperty(0) {
                            StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }
                    });
                        GetNodeWireCollection(NodeToAddTo).Wires.Add(new WireNode() { WireType = enable_WireType, OpCode = nodeDefinition.TypeId, NodeId = (short)FindNodeId(nodeDefinition) });
                    break;
                case (int)D3M_OpcodeType.PlayFMV:
                        if (parameters.Count < 1)
                            throw new D3M_CompileException("Not enough parameters!", sourceLine);

                        short stringId_fmvName = (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew(parameters[0].Text);

                        nodeDefinition = CreateLogicNode(ExportedMission, 25, new List<NodeProperty>
                        {
                            new StringProperty(stringId_fmvName) {
                                StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFMVFile")
                            }
                        });
                        GetNodeWireCollection(NodeToAddTo).Wires.Add(new WireNode() { WireType = enable_WireType, OpCode = nodeDefinition.TypeId, NodeId = (short)FindNodeId(nodeDefinition) });
                    break;

                // default stuff
                case 0: // (nop)
                    break;
                default:
                    if (sourceLine > -1 && sourceLine != null)
                       throw new InvalidOperationException($"Unknown opcode of the type {type}! at line {sourceLine + 1}");
                    else
                       throw new InvalidOperationException($"Unknown opcode of the type {type}!");
            }
            if (nodeDefinition != null)
                LastCreatedNode = nodeDefinition;
            return nodeDefinition;
        }
    }
}
