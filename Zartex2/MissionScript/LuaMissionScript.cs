using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using DSCript;
using DSCript.Spooling;

using MoonSharp;
using MoonSharp.Interpreter;

using Zartex;

namespace Zartex
{
    [MoonSharpUserData]
    public static class VehicleType
    {
        public static readonly int Camaro = 0;                 // speculation: cut very early in development; Stuntman car?
        public static readonly int Capri = 1;                  // speculation: cut very early in development; Stuntman car?
        public static readonly int Taxi = 2;
        public static readonly int Mustang_Blue = 3;           // speculation: used in early alpha before cars had colors
        public static readonly int Mustang_White = 4;         // speculation: used in early alpha before cars had colors
        public static readonly int Mustang_Yellow = 5;         // speculation: used in early alpha before cars had colors
        public static readonly int Mustang_Grey = 6;           // speculation: used in early alpha before cars had colors; why this became the final vehicle? who knows..
        public static readonly int Mustang_Red = 7;            // speculation: used in early alpha before cars had colors
        public static readonly int Dougal = 8;                 // speculation: cut very early in development; what the heck is a Dougal?! something from Stuntman?
        public static readonly int CopCar_Miami = 9;
        public static readonly int Jalpa = 10;
        public static readonly int Vantage = 11;                // speculation: alternative version of AstonMartin?
        public static readonly int Citroen2CV = 12;             // cut late in development, seen in few screenshots
        public static readonly int Countach = 13;
        public static readonly int CopCar_Istanbul = 14;
        public static readonly int DodgeTruck = 15;
        public static readonly int Bora = 16;
        public static readonly int Panda = 17;
        public static readonly int HierarchyTest = 18;          // speculation: used for testing 18-wheelers?
        public static readonly int RigTruck = 19;
        public static readonly int RigTrailer = 20;
        public static readonly int IstanbulTaxi = 21;
        public static readonly int Challenger = 22;
        public static readonly int ChevyVan = 23;
        public static readonly int MiamiFordPickup = 24;
        public static readonly int CitroenCX = 25;
        public static readonly int Starion = 26;                // speculation: cut late in development (it's an unreleased cut car, so we'll never know)
        public static readonly int BMW507 = 27;
        public static readonly int Datsun240Z = 28;
        public static readonly int ChevyBelaire = 29;
        public static readonly int IstanbulFordPickup = 30;
        public static readonly int Ducati = 31;
        public static readonly int ChevyBlazer = 32;
        public static readonly int Ferrari = 33;
        public static readonly int TransAm = 34;
        public static readonly int Bentley = 35;
        public static readonly int Dumpster = 36;
        public static readonly int Corvette = 37;
        public static readonly int Torino = 38;
        public static readonly int Miami_Bus = 39;
        public static readonly int Lincoln = 40;
        public static readonly int AuburnSpeedster = 41;
        public static readonly int Hotrod = 42;
        public static readonly int Gokart = 43;
        public static readonly int VRod = 44;
        public static readonly int Scarab = 45;                 // speculation: Tanner originally had his own unique speedboat; later became the only speed boat
        public static readonly int Miami_Cruiser_Boat = 46;
        public static readonly int Miami_Small_Boat = 47;
        public static readonly int Miami_Speed_Boat = 48;       // speculation: cut early in development - Scarab used instead?
        public static readonly int MetroMover = 49;
        public static readonly int BMW_Alpina = 50;
        public static readonly int Jaguar = 51;
        public static readonly int RenaultVan = 52;
        public static readonly int RenaultVanPickup = 53;
        public static readonly int Forklift = 54;
        public static readonly int AstonMartin = 55;
        public static readonly int Citroen_DS = 56;             // cut very early in development, seen in a couple of screenshots (as a cop car!)
        public static readonly int CopCar_Nice = 57;
        public static readonly int Nice_Bus = 58;
        public static readonly int Nice_Van = 59;
        public static readonly int Nice_Truck = 60;
        public static readonly int Nice_Taxi = 61;
        public static readonly int Reanult_5 = 62;
        public static readonly int Porsche = 63;                // speculation: cut very early in development
        public static readonly int SwatVan = 64;                // speculation: cut due to time constraints, so replaced with the Sobe truck gimmick xD
        public static readonly int CamperVan = 65;
        public static readonly int Cobra = 66;
        public static readonly int Nice_SportsBike = 67;
        public static readonly int Nice_Moped = 68;
        public static readonly int Nice_SpeedBoat = 69;
        public static readonly int Nice_Cruiser_Boat = 70;
        public static readonly int Nice_Small_Boat = 71;
        public static readonly int Chevy_Impala = 72;
        public static readonly int Istanbul_Bus = 73;
        public static readonly int RenaultAlpine = 74;
        public static readonly int Wagonaire = 75;
        public static readonly int MackTruck = 76;
        public static readonly int Merc = 77;
        public static readonly int Bugatti = 78;
        public static readonly int Racer = 79;
        public static readonly int GT40 = 80;
        public static readonly int Istanbul_StreetBike = 81;
        public static readonly int Istanbul_Moped = 82;
        public static readonly int Fishing_Boat = 83;
        public static readonly int Big_Cruiser_Boat = 84;
        public static readonly int Istanbul_SpeedBoat = 85;
        public static readonly int Tram = 86;
        public static readonly int Train = 87;
        public static readonly int RigCarrierTrailer = 88;
        public static readonly int Miami_Cop_Boat = 89;
        public static readonly int Nice_Cop_Boat = 90;
        public static readonly int Istanbul_Cop_Boat = 91;
        public static readonly int CitroenZX = 92;
        public static readonly int VW_Passat = 93;
        public static readonly int MercSL500 = 94;
        public static readonly int Carriage = 95;
    };
    // Logic data to Lua
    [MoonSharpUserData]
    public class Actor
    {
        public ActorDefinition TheActor = new ActorDefinition();
        public int index; // extremely important
        public Actor(ActorDefinition actor, int idx)
        {
            index = idx;
            TheActor = actor;
        }

        public static implicit operator int(Actor actor)
        {
            return actor.index;
        }

        public byte TypeId { get { return TheActor.TypeId; } set { TheActor.TypeId = value; } }

        public short Flags { get { return TheActor.Flags; } set { TheActor.Flags = value; } }

        public void SetColor(byte r, byte g, byte b, byte a = 255)
        {
            TheActor.Color = new NodeColor(r, g, b, a);
        }

        public void SetFloatPropertyValue(int id, float value)
        {
            FloatProperty p = (FloatProperty)(TheActor.Properties[id]);
            p.Value = (float)value;
        }
        public void SetFloatPropertyValue(int id, double value)
        {
            FloatProperty p = (FloatProperty)(TheActor.Properties[id]);
            p.Value = (float)value;
        }
        public void SetIntPropertyValue(int id, int value)
        {
            IntegerProperty p = (IntegerProperty)(TheActor.Properties[id]);
            p.Value = (int)value;
        }
        public void SetFloat3PropertyValue(int id, float x, float y, float z)
        {
            Float3Property p = (Float3Property)(TheActor.Properties[id]);
            p.Value = new Vector3(x, y, z);
        }
        public void SetFloat4PropertyValue(int id, float x, float y, float z, float w)
        {
            Float4Property p = (Float4Property)(TheActor.Properties[id]);
            p.Value = new Vector4(x, y, z, w);
        }
        public void SetTextPropertyValue(int id, short index, short value)
        {
            var prop = (TextFileItemProperty)TheActor.Properties[id];
            prop.Index = index;
            prop.Value = value;
        }
        public void SetAudioPropertyValue(int id, short bank, short sample)
        {
            var prop = (AudioProperty)TheActor.Properties[id];
            prop.Value = new AudioInfo(bank, sample);
        }
        public void SetActorPropertyValue(int id, Actor actor)
        {
            ActorProperty p = (ActorProperty)(TheActor.Properties[id]);
            p.Value = actor == null ? -1 : actor.index;
        }

        public DynValue GetProperyValue(int id)
        {
            var prop = TheActor.Properties[id];
            // property is known
            if (prop is FloatProperty)
            {
                return DynValue.NewNumber(((FloatProperty)(prop)).Value);
            }
            if (prop is IntegerProperty)
            {
                return DynValue.NewNumber(((IntegerProperty)(prop)).Value);
            }
            if (prop is ActorProperty)
            {
                int actorId = ((ActorProperty)(prop)).Value;
                return DynValue.NewNumber(actorId); // only actor ID can be returned
            }
            // otherwise
            return null;
        }
    }
    [MoonSharpUserData]
    public class Node 
    {
        public NodeDefinition TheNode = new ActorDefinition();
        public int index; // extremely important
        public CollectionOfWires WireCollection { get; set; }
        public CollectionOfWires Wires { get { return WireCollection; } set { WireCollection = value; } }

        public Node(NodeDefinition node, int idx)
        {
            index = idx;
            TheNode = node;
        }

        public static implicit operator int(Node node)
        {
            return node.index;
        }

        public void SetWireCollection(CollectionOfWires collectionOfWires, bool appendId = false)
        {
            WireCollection.Wires = collectionOfWires.Wires;
            if (appendId)
            {
                foreach (NodeProperty prop in TheNode.Properties)
                {
                    if (prop is WireCollectionProperty)
                    {
                        WireCollectionProperty wcp = prop as WireCollectionProperty;
                        wcp.Value = -collectionOfWires.index;
                        break;
                    }
                }
            }
        }

        public byte TypeId { get { return TheNode.TypeId; } set { TheNode.TypeId = value; } }

        public short Flags { get { return TheNode.Flags; } set { TheNode.Flags = value; } }

        public void SetColor(byte r, byte g, byte b, byte a = 255)
        {
            TheNode.Color = new NodeColor(r, g, b, a);
        }

        public void SetFloatPropertyValue(int id, float value)
        {
            FloatProperty p = (FloatProperty)(TheNode.Properties[id]);
            p.Value = (float)value;
        }
        public void SetFloatPropertyValue(int id, double value)
        {
            FloatProperty p = (FloatProperty)(TheNode.Properties[id]);
            p.Value = (float)value;
        }
        public void SetIntPropertyValue(int id, int value)
        {
            IntegerProperty p = (IntegerProperty)(TheNode.Properties[id]);
            p.Value = (int)value;
        }
        public void SetFloat3PropertyValue(int id, float x, float y, float z)
        {
            Float3Property p = (Float3Property)(TheNode.Properties[id]);
            p.Value = new Vector3(x, y, z);
        }
        public void SetFloat4PropertyValue(int id, float x, float y, float z, float w)
        {
            Float4Property p = (Float4Property)(TheNode.Properties[id]);
            p.Value = new Vector4(x, y, z, w);
        }
        public void SetTextPropertyValue(int id, short index, short value)
        {
            var prop = (TextFileItemProperty)TheNode.Properties[id];
            prop.Index = index;
            prop.Value = value;
        }
        public void SetAudioPropertyValue(int id, short bank, short sample)
        {
            var prop = (AudioProperty)TheNode.Properties[id];
            prop.Value = new AudioInfo(bank, sample);
        }
        public void SetActorPropertyValue(int id, Actor actor)
        {
            ActorProperty p = (ActorProperty)(TheNode.Properties[id]);
            p.Value = actor == null ? -1 : actor.index;
        }

        public DynValue GetProperyValue(int id)
        {
            var prop = TheNode.Properties[id];
            // property is known
            if (prop is FloatProperty)
            {
                return DynValue.NewNumber( ((FloatProperty)(prop)).Value );
            }
            if (prop is IntegerProperty)
            {
                return DynValue.NewNumber(((IntegerProperty)(prop)).Value);
            }
            if (prop is ActorProperty)
            {
                int actorId = ((ActorProperty)(prop)).Value;
                return DynValue.NewNumber(actorId); // only actor ID can be returned
            }
            // otherwise
            return null;
        }
    }
    // LEWC to Lua
    [MoonSharpUserData]
    public class CollectionOfWires : WireCollection
    {
        //public List<WireNode> Wires = new List<WireNode>();
        public int index; // extremely important

        public void Add(Node node, byte type = 1)
        {
            if (node==null)
            {
                throw new ScriptRuntimeException("Attempt to add to wire collection a nil value");
            }
            Wires.Add(new WireNode()
            {
                NodeId = (short)node.index,
                OpCode = node.TheNode.TypeId,
                WireType = (byte)type
            });
        }
        public void RemoveAt(int id)
        {
            Wires.RemoveAt(id);
        }
        public void RemoveRange(int id, int count)
        {
            Wires.RemoveRange(id, count);
        }
        public void RemoveNode(Node node)
        {
            int wnId = 0;
            foreach(WireNode wn in Wires)
            {
                if (wn.NodeId==(short)node.index)
                {
                    Wires.Remove(wn);
                    return; // stop before a error is returned
                }
                wnId++;
            }
        }
        public CollectionOfWires(WireCollection wireCollection) : base(0)
        {
            Wires = wireCollection.Wires;
        }
        public CollectionOfWires(int nWires = 0) : base(nWires)
        {
        }
        public CollectionOfWires(int nWires, int idx) : base(nWires)
        {
            index = idx;
        }
    }
    // GEBI to Lua
    [MoonSharpUserData]
    public class BuildingInstanceData : MissionInstanceData
    {
        public List<MissionObject> _props = new List<MissionObject>();
        public void CreateInstance(float x, float y, float z, short instanceId, short attachedTo=-1, float bx = 1, float by = 0, float bz = 0, float bw = 101)
        {
            _props.Add(new PropObject() { Id = Instances.Count });
            Instances.Add(new MissionInstance(new Vector3(x, y, z), instanceId, attachedTo, new Vector4(bx, by, bz, bw)));
        }
        public BuildingInstanceData() : base() { Instances = new List<MissionInstance>(); Unk1 = -4; Unk2 = -4; }
    }
    // LEAS to Lua
    [MoonSharpUserData]
    public class ActorSetting : ActorSet
    {
        public int index; // extremely important
    }
    [MoonSharpUserData]
    public class ActorSetup : ActorSetTableData
    {
        public void Add(ActorSetting actorSetting)
        {
            actorSetting.index = Table.Count;
            Table.Add(actorSetting);
        }
        public ActorSetup() { Table = new List<ActorSet>(); }
    }
    [MoonSharpUserData]
    public class ActorCollection
    {
        public List<int> Collection { get; set; }
    }
    [MoonSharpUserData]
    public class MissionSummary : MissionSummaryData
    {
        public short MoodId { get { return this.MissionId; } set { this.MissionId = value; } }
        public short LocaleId { get { return this.MissionLocaleId; } set { this.MissionLocaleId = value; } }

        public float X { get { return this.StartPosition.X; } set { this.StartPosition = new Vector2(value, this.StartPosition.Y); } }
        public float Y { get { return this.StartPosition.Y; } set { this.StartPosition = new Vector2(this.StartPosition.X, value); } }

        public SpoolableBuffer Spooler { get { return base.Spooler; } set { base.Spooler = value; } }

        public static string GetCityNameByType(MissionCityType cityType)
        {
            switch (cityType)
            {
                case MissionCityType.Miami_Day:
                    return "Miami_Day";
                case MissionCityType.Miami_Night:
                    return "Miami_Night";
                case MissionCityType.Nice_Day:
                    return "Nice_Day";
                case MissionCityType.Nice_Night:
                    return "Nice_Night";
                case MissionCityType.Istanbul_Day:
                    return "Istanbul_Day";
                case MissionCityType.Istanbul_Night:
                    return "Istanbul_Night";
                default:
                    return "Unknown";
            }
        }

        public static MissionCityType GetCityTypeByName(string cityType)
        {
            switch (cityType)
            {
                case "Miami_Day":
                    return MissionCityType.Miami_Day;
                case "Miami_Night":
                    return MissionCityType.Miami_Night;
                case "Nice_Day":
                    return MissionCityType.Nice_Day;
                case "Nice_Night":
                    return MissionCityType.Nice_Night;
                case "Istanbul_Day":
                    return MissionCityType.Istanbul_Day;
                case "Istanbul_Night":
                    return MissionCityType.Istanbul_Night;
                default:
                    return 0;
            }
        }

        // Miami_Day
        // Miami_Night
        // Nice_Day
        // Nice_Night
        // Istanbul_Day
        // Istanbul_Night
        public string Level
        {
            get
            {
                return GetCityNameByType(this.CityType);
            }
            set
            {
                this.CityType = GetCityTypeByName(value);
            }
        }
        public MissionSummary() { StartPosition = new Vector2(0,0); }
    }
    // oh god.. here we go again...
    // This is Lua mission script for Driv3r
    [MoonSharpUserData]
    public class LuaMissionScript
    {
        public string SourceScriptFilePath { get; set; }

        public static int toflag(byte arg1, byte arg2, byte arg3, byte arg4)
        {
            byte[] f = new byte[4] { arg1, arg2, arg3, arg4 };
            return BitConverter.ToInt32(f, 0);
        }

        public static LuaMissionScript LoadScriptFromFile(string filepath)
        {
            Script script = new Script();
            UserData.RegisterAssembly();

            LuaMissionScript LMS = new LuaMissionScript();
            LMS.WorkingDirectory = Path.GetDirectoryName(filepath);
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

            // hmm.. I'm not sure if these really help
            script.Globals["PERSONALITY_UNDEFINED"] = -1;
            script.Globals["PERSONALITY_CIVILIANSTUCK"] = 0;
            script.Globals["PERSONALITY_NORMAL"] = 1;
            script.Globals["PERSONALITY_PATHANDVEHICLE"] = 2;
            script.Globals["PERSONALITY_PATHFACE"] = 9;

            // for Vehicle actor
            script.Globals["VEHICLEFLAGS_SPAWNED_AND_UNLOCKED"] = 0x12030001;
            script.Globals["VEHICLEFLAGS_UNSPAWNED_AND_UNLOCKED"] = 0x12030000;

            // for Character actor
            script.Globals["CHARACTERFLAGS_UNSPAWNED"] = 131072;
            script.Globals["CHARACTERFLAGS_SPAWNED"] = 131073;

            // for SpecialEffect actor
            script.Globals["SPECIALEFFECT_FAKE_EXPLOSION"] = 4;
            script.Globals["SPECIALEFFECT_EXPLOSION"] = 1;

            // vehicle types enum for Lua (as a class)
            script.Globals["VehicleType"] = typeof(VehicleType);

            // for CheatControl node
            script.Globals["CHEAT_INFINITEMASS"] = 1;
            script.Globals["CHEAT_18WHEELER_MODE"] = 2;
            script.Globals["CHEAT_FUGITIVE_MODE"] = 3;

            // for ProximityCheck node
            script.Globals["PROXIMITYCHECKTYPE_APROXIMATE"] = 1;
            script.Globals["PROXIMITYCHECKTYPE_DISTANCE"] = 2;

            // for OpenCargoDoorsControl node
            script.Globals["CARGODOORSACTION_OPEN"] = 1;
            script.Globals["CARGODOORSACTION_CLOSE"] = 2;

            // temporary
			// CharacterControl.pProperty: 18 = teleport?

            // for AIPath actor
            script.Globals["PATHFLAGS_STOP_WHEN_DONE"] = 65536;
            script.Globals["PATHFLAGS_CONTINUE_WHEN_DONE"] = 1;

            // for CountdownIntro node
            script.Globals["CLOCKCOMMAND_START_TIMER"] = 1;
            script.Globals["CLOCKCOMMAND_EQUALS"] = 1;
            script.Globals["CLOCKCOMMAND_START_COUNTDOWN"] = 4;
            script.Globals["CLOCKCOMMAND_STOP"] = 5;
            script.Globals["CLOCKCOMMAND_HIDE"] = 6;

            // thanks "Welcome to Nice" mission!
            script.Globals["COUNTERACTION_EQUALS"] = 1;
            script.Globals["COUNTERACTION_SOMATE"] = 2;
            script.Globals["COUNTERACTION_SUBTRACT"] = 3;

            script.Globals["COUNTERCONDITION_SMALLERTHAN"] = 5;
            script.Globals["COUNTERCONDITION_BIGGERTHAN"] = 4;
			
            script.Globals["COLLISIONWATCH_FLAGS_TOUCH"] = 1;

            // Messages display types
            script.Globals["DISPLAYMESSAGE_CENTER_UP"] = 0x010000;
            script.Globals["DISPLAYMESSAGE_LEFT_CORNER_DOWN"] = 0x000002;

            // Character skins
            script.Globals["SKIN_TANNER"] = 0xBA125961;
            script.Globals["SKIN_JONES"] = 0xE52A26EA;

            script.Globals["SKIN_LOMAZ"] = 0x759C10B3;
            script.Globals["SKIN_CALITA"] = 0xDCE0D782;
            script.Globals["SKIN_BADHAND"] = 0xF52DB73B;
            script.Globals["SKIN_JERICHO"] = 0x1517B105;

            script.Globals["SKIN_FABIENNE"] = 0x076EB88C;
            script.Globals["SKIN_FAB_GOON1"] = 0x681351A6;
            script.Globals["SKIN_FAB_GOON2"] = 0xF11A001C;
            script.Globals["SKIN_FAB_GOON3"] = 0x861D308A;

            script.Globals["SKIN_BODYGUARD1"] = 0x1944FDE4;
            script.Globals["SKIN_BODYGUARD2"] = 0x804DAC5E;

            script.Globals["SKIN_BACCUS"] = 0xA8EAD1AE;
            script.Globals["SKIN_TICO"] = 0x11C25CD5;

            // South beach goons
            script.Globals["SKIN_SB_GOON1"] = 0x9E892CAC;
            script.Globals["SKIN_SB_GOON2"] = 0x7807D16;

            script.Globals["SKIN_THE_GATOR"] = 0x613C6BD2;

            // The Gator's goons
            script.Globals["SKIN_GATOR_GOON1"] = 0xA99D8E66;
            script.Globals["SKIN_GATOR_GOON2"] = 0x3094DFDC;
            script.Globals["SKIN_GATOR_GOON3"] = 0x4793EF4A;

            // Jericho's goons (from Istanbul)
            script.Globals["SKIN_JERICHO_GOON1"] = 0x135E6F23;
            script.Globals["SKIN_JERICHO_GOON2"] = 0x8A573E99;
            script.Globals["SKIN_JERICHO_GOON3"] = 0xFD500E0F;

            script.Globals["SKIN_TIMMY"] = 0x10B0D246;

            script.Globals["SKIN_TURKISH_UNDERCOVER"] = 0xAC1CD5A4;
            script.Globals["SKIN_FRENCH_UNDERCOVER"] = 0x9B548DB;

            script.Globals["SKIN_MIAMICOP1"] = 0xEC1491ED; // chief
            script.Globals["SKIN_MIAMICOP2"] = 0x751DC057;
            script.Globals["SKIN_MIAMICOP3"] = 0xEB7955F4;
            script.Globals["SKIN_MIAMICOP4"] = 0xCAD3FF7B; // black cop
            script.Globals["SKIN_NICECOP1"] = 0x8C95F722;
            script.Globals["SKIN_NICECOP2"] = 0x65F65217;

            script.Globals["SKIN_MIAMI_WMWSBP"] = 0x54B76AD8;    // White Male: wearing White Shirt and Black Pants 
            script.Globals["SKIN_MIAMI_WOMYBHJP"] = 0xCDBE3B62;  // White Old Male: Yellow Brown Hair and wearing Jeans Pants
            script.Globals["SKIN_MIAMI_BMWSBP"] = 0xBAB90BF4;    // Black Male: wearing White Shirt and Black Pants
            script.Globals["SKIN_MIAMI_BWMTTBS"] = 0x24DD9E57;   // Bald White Male: wearing Tank Top and Brown Shorts
            script.Globals["SKIN_MIAMI_WMPSWP"] = 0x53DAAEC1;    // White Male: wearing Pink Suit and White Pants
            script.Globals["SKIN_MIAMI_WFWD"] = 0x5A6CE2EA;      // White Female: wearing White Dress
            script.Globals["SKIN_MIAMI_WMBSBS"] = 0xD3C8CE3A;    // White Male: wearing Black Sweater and Black Jeans
            script.Globals["SKIN_MIAMI_BFBSBP"] = 0xA4CFFEAC;    // Black Female: wearing Brown Sweater and Black Pants
            script.Globals["SKIN_MIAMI_WFBSBP"] = 0x3470E33D;    // White Female: wearing Black Sweater and Black Pants

            //script.Globals["SKIN_MIAMI_MARATHONIST_WOMAN"] = DynValue.NewNumber(0x3AAB6B0F);
            //script.Globals["SKIN_MIAMI_BLACK_WOMAN"] = DynValue.NewNumber(0xCDBE3B62);
            //script.Globals["SKIN_MIAMI_BLACK_WOMAN"] = DynValue.NewNumber(0xD3C8CE3A);

            // I fix it later :)
            // TODO: Fix skin IDs (mission57.dam)
            /*
            script.Globals["SKIN_NICE_WHITESHIRT_MAN"] = 0xBF80D1DB;
            script.Globals["SKIN_NICE_BLACKSHIRT_MAN"] = 0x518EB0F7;
            script.Globals["SKIN_NICE_TANKTOP_MAN"] = 0xCFEA2554;
            script.Globals["SKIN_NICE_BUSY_WOMAN"] = 0x21E44478;
            script.Globals["SKIN_NICE_YOUNG_WOMAN"] = 0xA69BE09A; 
            script.Globals["SKIN_NICE_BIKINI_WOMAN"] = 0xC65C697F;
            script.Globals["SKIN_NICE_BUSY_MAN"] = 0x3F92B120;
            script.Globals["SKIN_NICE_SWEATER_WOMAN"] = 0xD6F11415;
            */

            //script.DoString("local logicStart = MISSION.logicStart()"); // logic start global variable

            DynValue res = script.DoFile(filepath);
            LMS.SourceScriptFilePath = filepath;

            //script.DoString("LogicStart()"); // calls the logic start
            return LMS;
        }

        public ExportedMission missionData = new ExportedMission();
        public MissionSummary missionSummary = new MissionSummary();
        public BuildingInstanceData instanceData = new BuildingInstanceData();
        public ActorSetup actorSetup = new ActorSetup();

        public string WorkingDirectory = ""; // "" = relative to the tool 

        public List<WireCollection> wireCollection = new List<WireCollection>();
        //public Table wireCollection;

        public List<int> SpoolVehicles { get; protected set; }

        // table must contain Actor in each
        public ActorSetting CreateActorSetting(Table set)
        {
            ActorSetting actorSetting = new ActorSetting();
            foreach(DynValue actorSetEntry in set.Keys)
            {
                UserData cast = actorSetEntry.UserData;
                int index = (int)(actorSetEntry.Number)-1;
                Actor actorCast = new Actor(missionData.LogicData.Actors.Definitions[index],index);//cast.Object as Actor;
                actorSetting.Sets.Add(actorCast.index);
            }
            return actorSetting;
        }

        public LuaMissionScript(string workingDirectory = "")
        {
            SpoolVehicles = new List<int>();
            SourceScriptFilePath = "";
            WorkingDirectory = workingDirectory;

            // create the logic data
            missionData.LogicData = new LogicExportData();

            missionData.LogicData.Actors = new LogicDataCollection<ActorDefinition>();
            missionData.LogicData.Actors.Definitions = new List<ActorDefinition>();

            // string stuff
            missionData.LogicData.StringCollection = new StringCollectionData();
            missionData.LogicData.StringCollection.Strings = new List<string>();

            // default strings
            missionData.LogicData.StringCollection.AppendString("");
            missionData.LogicData.StringCollection.AppendString("Unknown");

            // logic stuff
            missionData.LogicData.Nodes = new LogicDataCollection<NodeDefinition>();
            missionData.LogicData.Nodes.Definitions = new List<NodeDefinition>();

            missionData.LogicData.WireCollection = new WireCollectionData();
            missionData.LogicData.WireCollection.WireCollections = new List<WireCollection>();

            // objects
            missionData.Objects = new ExportedMissionObjects();
            missionData.Objects.Objects = new List<MissionObject>();

            // sound bank table
            missionData.LogicData.SoundBankTable = new SoundBankTableData();
            missionData.LogicData.SoundBankTable.Table = new List<int>();

            // script counters
            missionData.LogicData.ScriptCounters = new ScriptCountersData();
            missionData.LogicData.ScriptCounters.Counters = new List<int>();

            /*
            // logic start
            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(255, 255, 0, 255),
                TypeId = 1,
                StringId = 1,
                Properties = new List<NodeProperty>()
                {
                    new WireCollectionProperty(0) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },         // pWireCollection
                }
            });
            wireCollection.Add(new CollectionOfWires(0)); // add the logic start wire collection
            */
        }

        // LUA functions: MISSION.functionName(args)
        public CollectionOfWires CreateWireCollection(int nWires = 0)
        {
            CollectionOfWires cow = new CollectionOfWires(nWires);
            wireCollection.Add(cow);
            return cow;
        }

        public void AddSoundBankToLoadList(int bank)
        {
            missionData.LogicData.SoundBankTable.Table.Add(bank);
        }

        // Lua starts from 1 and C# starts indexing from 0
        public Actor GetActorById(int id)
        {
            if ((id - 1) > missionData.LogicData.Actors.Definitions.Count - 1)
                throw new ScriptRuntimeException("Bad argument #1 - Index ID can't be bigger than the size of the array");
            if ((id - 1) < 0)
                throw new ScriptRuntimeException("Bad argument #1 - Index ID can't be negative or zero");
            return new Actor(missionData.LogicData.Actors.Definitions[(id - 1)], (id - 1));
        }

        public void SetActorUID(Actor actor, int uid)
        {
            MissionObject mo = missionData.Objects.Objects[actor.TheActor.ObjectId];
            int actorType = actor.TheActor.TypeId;
            switch (actorType)
            {
                case 2:
                    CharacterObject ch = mo as CharacterObject;
                    ch.UID = uid;
                    break;
                case 3:
                    VehicleObject vh = mo as VehicleObject;
                    vh.UID = uid;
                    break;
            }
        }

        public void SetActorObjectPositionXYZA(Actor actor, float x, float y, float z, float angle)
        {
            MissionObject mo = missionData.Objects.Objects[actor.TheActor.ObjectId];
            int actorType = actor.TheActor.TypeId;
            switch (actorType)
            {
                case 2:
                    CharacterObject ch = mo as CharacterObject;
                    ch.Position = new Vector3(x, y, z);
                    var a = (Math.PI / 180) * angle; // convert degrees to radians
                    Vector3 fwd = new Vector3(
                        (float)(Math.Cos(0) * Math.Cos(a)), // x

                        0, //(float)-Math.Sin(angle), // altitude

                        (float)(Math.Cos(0) * Math.Sin(a)) // z
                    );
                    // 24
                    MemoryStream stream = new MemoryStream(ch.CreationData.Length);
                    using (var f = new BinaryWriter(stream, Encoding.Default))
                    {
                        stream.Write(ch.CreationData);
                        stream.Position = 0x4;
                        stream.Write(ch.Position);
                        stream.Position = 0x10;
                        stream.Write(fwd);
                    }
                    ch.CreationData = stream.GetBuffer();
                    stream.Dispose();
                    return;
                case 3:
                    VehicleObject vh = mo as VehicleObject;
                    vh.Position = new Vector3(x, y, z);
                    return;
                case 4:
                    VolumeObject vl = mo as VolumeObject;
                    vl.Position = new Vector3(x, y, z);
                    return;
                case 5:
                    ObjectiveIconObject obj = mo as ObjectiveIconObject;
                    obj.Position = new Vector3(x, y, z);
                    return;
            }
        }

        public Actor FindPlayerActor()
        {
            int id = 0;
            foreach(ActorDefinition actor in missionData.LogicData.Actors.Definitions)
            {
                if (actor.TypeId == 2)
                {
                    if (actor.Properties!=null)
                    {
                        IntegerProperty prop = actor.Properties[0] as IntegerProperty;
                        if (prop.Value == 1)
                            return new Actor(missionData.LogicData.Actors.Definitions[id], id);
                    }
                }
                id++;
            }
            return null;
        }

        public Node GetLogicStart()
        {
            int id = 0;
            foreach(NodeDefinition node in missionData.LogicData.Nodes.Definitions)
            {
                if (node.TypeId == 1)
                {
                    return GetNodeById(id+1);
                    //return new Node(missionData.LogicData.Nodes.Definitions[id], id);
                }
                id++;
            }
            return null;
        }

        public Node GetNodeById(int id)
        {
            if ((id - 1) > missionData.LogicData.Nodes.Definitions.Count - 1)
                throw new ScriptRuntimeException("Bad argument #1 - Index ID can't be bigger than the size of the array");
            if ((id - 1) < 0)
                throw new ScriptRuntimeException("Bad argument #1 - Index ID can't be negative or zero");

            NodeDefinition nd = missionData.LogicData.Nodes.Definitions[(id - 1)];
            Node node = new Node(nd, (id - 1));
            int wireId = ((IntegerProperty)nd.Properties[0]).Value;
            node.WireCollection = new CollectionOfWires();
            node.WireCollection.Wires = wireCollection[wireId].Wires;
            node.WireCollection.index = wireId;
            return node;
        }

        public virtual DynValue ImportMissionScriptFromFile(string filename, bool appendSpooler = false)
        {
            string fn = $"{WorkingDirectory}\\{filename}";
            if ( appendSpooler == null )
            {
                throw new ScriptRuntimeException("Bad argument #2 - Bool expected got nil");
            }
            if ( !(appendSpooler is bool) )
            {
                throw new ScriptRuntimeException("Bad argument #2 - Bool expected got "+appendSpooler.GetType().ToString());
            }
            if (filename == "")
            {
                throw new ScriptRuntimeException("Bad argument #1 - Literally empty string detected");
            }
            if (!File.Exists(fn))
            {
                throw new ScriptRuntimeException("Bad argument #1 - The file does not exist - " + Path.GetFullPath(fn));
            }
#if DEBUG
#else
            try
            {
#endif
                MissionScriptFile externalMission = new MissionScriptFile(fn);
                if (!appendSpooler)
                {
                    if (externalMission.MissionData.LogicData.ScriptCounters!=null)
                    {
                    missionData.LogicData.ScriptCounters.Counters = externalMission.MissionData.LogicData.ScriptCounters.Counters;
                    }
                    missionData.LogicData.Nodes.Definitions = externalMission.MissionData.LogicData.Nodes.Definitions;
                    missionData.LogicData.Actors.Definitions = externalMission.MissionData.LogicData.Actors.Definitions;
                    missionData.Objects.Objects = externalMission.MissionData.Objects.Objects;
                    missionData.LogicData.StringCollection.Strings = externalMission.MissionData.LogicData.StringCollection.Strings;
                    missionData.LogicData.SoundBankTable.Table = externalMission.MissionData.LogicData.SoundBankTable.Table;
                    missionSummary.CityType = externalMission.MissionSummary.CityType;
                    missionSummary.X = externalMission.MissionSummary.StartPosition.X;
                    missionSummary.Y = externalMission.MissionSummary.StartPosition.Y;
                    missionSummary.MoodId = externalMission.MissionSummary.MissionId;
                    missionSummary.LocaleId = externalMission.MissionSummary.MissionLocaleId;
                    missionSummary.PingInRadius = externalMission.MissionSummary.PingInRadius;
                    missionSummary.PingOutRadius = externalMission.MissionSummary.PingOutRadius;
                    wireCollection = externalMission.MissionData.LogicData.WireCollection.WireCollections;
                    missionData.LogicData.WireCollection.WireCollections = wireCollection;
                    // instanceData.Instances = externalMission.MissionData.MissionInstances.Instances;
            }
            else
                {
                    missionData.Spooler = externalMission.MissionData.Spooler;
                }

                Debug.WriteLine($"Loaded {externalMission.MissionData.LogicData.Actors.Definitions.Count} actors, {externalMission.MissionData.LogicData.Nodes.Definitions.Count} logic nodes and {wireCollection.Count} wire collections");
                externalMission.Dispose();
#if DEBUG
#else
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                var frame = st.GetFrame(0);
                var line = frame.GetFileLineNumber();

                return DynValue.NewString(ex.Message+"\nLine: "+line+"\nFile: "+frame.GetFileName()); // failure, will return the message of the error
            }
#endif
            return DynValue.NewBoolean(true); // success
        }

        public Node LogicStart(string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 1,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node TruckDeliveryControl(Actor truck, Actor car1, Actor car2, Actor car3, Actor compound, int wrongCarTextId = -1, int distanceText = -1, int kilometerText = -1, int mileText = -1, string note = "", int r = 0, int g = 200, int b = 122)
        {
            if (truck.TheActor.TypeId != 3)
                throw new ScriptRuntimeException("Bad Argument #1 - The truck actor type is not a Vehicle actor type");
            if (car1.TheActor.TypeId != 3)
                throw new ScriptRuntimeException("Bad Argument #2 - The car #1 actor type is not a Vehicle actor type");
            if (car2.TheActor.TypeId != 3)
                throw new ScriptRuntimeException("Bad Argument #3 - The car #2 actor type is not a Vehicle actor type");
            if (car3.TheActor.TypeId != 3)
                throw new ScriptRuntimeException("Bad Argument #4 - The car #3 actor type is not a Vehicle actor type");
            if (compound == null)
                throw new ScriptRuntimeException("Bad Argument #5 - A actor to the compound position is required");
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 130,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },

                        new ActorProperty(truck) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pTruck")
                        },

                        new ActorProperty(car1) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCar1")
                        },
                        new ActorProperty(car2) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCar2")
                        },
                        new ActorProperty(car3) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCar3")
                        },

                        new ActorProperty(compound) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCompound")
                        },

                        new LocalisedStringProperty(wrongCarTextId) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWrongCarText")
                        },

                        new LocalisedStringProperty(distanceText) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pDistanceText")
                        },

                        new LocalisedStringProperty(kilometerText) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pKilometerText")
                        },

                        new LocalisedStringProperty(mileText) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMileText")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node CargoVehicleWatch(Actor truck, Actor car, int wrongCarText = -1, bool sideDoorsOnly = false, int wrongCarTextId = -1, int distanceText = -1, int kilometerText = -1, int mileText = -1, string note = "", int r = 0, int g = 200, int b = 122)
        {
            if (truck.TheActor.TypeId != 3)
                throw new ScriptRuntimeException("Bad Argument #1 - The truck actor type is not a Vehicle actor type");
            if (car.TheActor.TypeId != 3)
                throw new ScriptRuntimeException("Bad Argument #2 - The car actor type is not a Vehicle actor type");
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 125,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },

                        new ActorProperty(truck) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pTruck")
                        },

                        new ActorProperty(car) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pJag")
                        },
                        new BooleanProperty(sideDoorsOnly) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pSideDoorsOnly")
                        },
                        
                        new LocalisedStringProperty(wrongCarTextId) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWrongCarText")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node CharacterNameControl(Actor character, string name, bool showName = true, int playerNum = 1, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 107,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new ActorProperty(character == null ? -1 : character.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCharacter")
                        },
                        new UnicodeStringProperty(name) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCharacterName")
                        },
                        new BooleanProperty(showName) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pShowName")
                        },
                        new IntegerProperty(playerNum) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pPlayerNum")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node CargoDoorsControl(Actor truck, int action, int doors = 4, string note = "", int r = 100, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 136,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new ActorProperty(truck.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pTruck")
                        },
                        new WireCollectionProperty(doors) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pDoors")
                        },
                        new WireCollectionProperty(action) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAction")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node WatchLineOfSight(Actor actor1, Actor actor2, float maxDistance, float minDistance, int proximityTextId, string note = "", int r = 50, int g = 255, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 106,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },

                        new ActorProperty(actor1) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pActor1")
                        },
                        new ActorProperty(actor2) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pActor2")
                        },

                        new FloatProperty(maxDistance) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMaxDistance")
                        },
                        new FloatProperty(minDistance) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMinDistance")
                        },

                        new LocalisedStringProperty(proximityTextId) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pProximityText")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        // honestly the node with the most nodes ...
        public Node TrainControl(int numberOfCars, float topSpeed, float acceleration, float startingPos, int trackSegment, int direction, int missionType, bool showIcon, float stopDistance, float failDistance, int message = -1, Actor zone = null, int personalityIndex = 0, float vulnerability = 1f, int numPropModels = 3, int propsPerCar = 0, int weapon1 = 2, int weapon2 = 5, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 123,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },

                        new IntegerProperty(numberOfCars) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pNumberOfCars")
                        },
                        new FloatProperty(topSpeed) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pTopSpeed")
                        },
                        new FloatProperty(acceleration) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAcceleration")
                        },
                        new FloatProperty(startingPos) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pStartingPos")
                        },
                        new EnumProperty(trackSegment) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pTrackSegment")
                        },
                        new EnumProperty(direction) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pDirection")
                        },
                        new EnumProperty(missionType) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMissionType")
                        },
                        new BooleanProperty(showIcon) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pShowIcon")
                        },
                        new EnumProperty(weapon1) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWeapon1")
                        },
                        new EnumProperty(weapon2) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWeapon2")
                        },
                        new FloatProperty(vulnerability) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pVulnerability")
                        },
                        new IntegerProperty(numPropModels) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pNumPropModels")
                        },
                        new IntegerProperty(propsPerCar) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pPropsPerCar")
                        },

                        new FloatProperty(stopDistance) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pStopDistance")
                        },
                        new FloatProperty(failDistance) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFailDistance")
                        },

                        new ActorProperty(zone == null ? -1 : zone.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pZone")
                        },

                        new IntegerProperty(personalityIndex) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pPersonalityIndex")
                        },
                        new LocalisedStringProperty(message) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMessage")
                        },
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node AwardVehicleToPlayer(Actor character, Actor marker, int type, int tintValue, float greenRadius = 125f, float yellowRadius = 95f, float redRadius = 20f, int flags = 2, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 140,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },

                        // char, marker
                        new ActorProperty(character.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCharacter")
                        },
                        new ActorProperty(marker.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMarker")
                        },

                        // type, tint
                        new IntegerProperty(type) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pType")
                        },
                        new IntegerProperty(tintValue) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pTintValue")
                        },

                        // g,y,r
                        new FloatProperty(greenRadius) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pGreenRadius")
                        },
                        new FloatProperty(yellowRadius) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pYellowRadius")
                        },
                        new FloatProperty(redRadius) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pRedRadius")
                        },

                        // flags
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node AwardWeaponToPlayer(int action, int item, int weapon, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 129,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new EnumProperty(action) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAction")
                        },
                        new EnumProperty(item) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pItem")
                        },
                        new EnumProperty(weapon) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWeapon")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node ActionButtonWatch(Actor targetSwitch, int flags = 2, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 13,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new ActorProperty(targetSwitch.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pSwitch")
                        },
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node ObjectControl(Actor obj, int activity, int tyresFlags = 0, int flags = 0, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 23,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new ActorProperty(obj.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pObject")
                        },
                        new EnumProperty(activity) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pActivity")
                        },

                        new FlagsProperty(tyresFlags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pTyres")
                        },
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node CheatControl(int cheat, string note = "", int r = 234, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 134,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new EnumProperty(cheat) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCheat")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node SetChaseVehicle(Actor vehicle, float health, int textId, float minForce = 1700, float maxForce = 14000, float bulletDamage = 0.085f, bool invicibleTyres = true, string note = "", int r = 100, int g = 0, int b = 222)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 137,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },

                        new ActorProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },

                        new FloatProperty(minForce) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMinForce")
                        },
                        new FloatProperty(maxForce) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMaxForce")
                        },
                        new FloatProperty(health) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pHealth")
                        },
                        new FloatProperty(bulletDamage) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pBulletDamage")
                        },
                        new LocalisedStringProperty(textId) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pText")
                        },
                        new BooleanProperty(invicibleTyres) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pInvicibleTyres")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node ProximityCheck(Actor actor1, Actor actor2, float threshold, int checkType, int flags = 1, string note = "", int r = 0, int g = 200, int b = 122)
        {
            if (actor1 == null)
                throw new ScriptRuntimeException("Bad Argument #1 - Actor expected got nil");
            if (actor2 == null)
                throw new ScriptRuntimeException("Bad Argument #2 - Actor expected got nil");
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 30,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new ActorProperty(actor1.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pActor1")
                        },
                        new ActorProperty(actor2.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pActor2")
                        },
                        new FloatProperty(threshold) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pThreshold")
                        },
                        new EnumProperty(checkType) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCheckType")
                        },
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node BombTruckControl(Actor vehicle, int timer, int detonationTimer, float initialVelocity, float proximity = 6.5f, float explosionImpulseScale = 0.8f, float explosionRange = 7.0f, float damageScale = 0.5f, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 114,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },

                        new IntegerProperty(timer) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pTimer")
                        },
                        new IntegerProperty(detonationTimer) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pDetonationTimer")
                        },

                        new FloatProperty(initialVelocity) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pInitialVelocity")
                        },
                        new FloatProperty(proximity) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pProximity")
                        },
                        new FloatProperty(explosionImpulseScale) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pExplosionImpulseScale")
                        },
                        new FloatProperty(explosionRange) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pExplosionRange")
                        },
                        new FloatProperty(damageScale) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pDamageScale")
                        },
                        new ActorProperty(vehicle.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pVehicle")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node RearVehicleShooting(Actor vehicle, Actor player, int weapon1, int weapon2, int vehicleType = 2, string note = "", int r = 255, int g = 190, int b = 122)
        {
            if (vehicle == null)
                throw new ScriptRuntimeException("Bad Argument #1 - Actor of type Vehicle expected, got nil");
            if (player == null)
                throw new ScriptRuntimeException("Bad Argument #2 - Actor of type Character expected, got nil");
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 105,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },

                        new ActorProperty(vehicle.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pVehicle")
                        },
                        new ActorProperty(player.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pPlayer")
                        },

                        new EnumProperty(weapon1) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWeapon1")
                        },
                        new EnumProperty(weapon2) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWeapon2")
                        },

                        new EnumProperty(vehicleType) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pVehicleType")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node CollisionWatch(Actor actor, Actor secondActor = null, int impactForce = 500, int flags = 1, string note = "", int r = 255, int g = 190, int b = 122)
        {
            if (actor == null)
                throw new ScriptRuntimeException("Bad Argument #1 - Actor expected, got nil");
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 22,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },

                        new ActorProperty(actor) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pActor")
                        },
                        new ActorProperty(secondActor == null ? -1 : secondActor.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pSecondActor")
                        },

                        new FloatProperty(impactForce) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pImpactForce")
                        },
                        
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node VehicleGunnerControl(Actor player, Actor passenger, int weapon, int numberOfBullets, bool alwaysOverlay = false, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 109,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },

                        new ActorProperty(player.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pPlayer")
                        },
                        new ActorProperty(passenger.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pPassenger")
                        },

                        new EnumProperty(weapon) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWeapon")
                        },
                        new IntegerProperty(numberOfBullets) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pNumBullets")
                        },
                        new BooleanProperty(alwaysOverlay) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAlwaysOverlay")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node CounterControl(int value, int counterIndex, int action, string name = null, int flags = 0, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 17,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new IntegerProperty(value) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pValue")
                        },
                        new IntegerProperty(counterIndex) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCounterIndex")
                        },
                        new EnumProperty(action) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAction")
                        },
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            if (name!=null)
            {
                missionData.LogicData.Nodes.Definitions[idx].Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new StringProperty((short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(name)) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pName")
                        },
                        new IntegerProperty(value) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pValue")
                        },
                        new IntegerProperty(counterIndex) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCounterIndex")
                        },
                        new EnumProperty(action) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAction")
                        },
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }
                    };
            }
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node CounterWatch(int value, int counterIndex, int condition, string name = null, int flags = 0, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 4,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new IntegerProperty(value) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pValue")
                        },
                        new IntegerProperty(counterIndex) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCounterIndex")
                        },
                        new EnumProperty(condition) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCondition")
                        },
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            if (name != null)
            {
                missionData.LogicData.Nodes.Definitions[idx].Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new StringProperty((short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(name)) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pName")
                        },
                        new IntegerProperty(value) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pValue")
                        },
                        new IntegerProperty(counterIndex) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCounterIndex")
                        },
                        new EnumProperty(condition) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCondition")
                        },
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }
                    };
            }
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node FrameDelay(int frames = 1, int flags = 0, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 11,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new IntegerProperty(frames) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFrameDelay")
                        },
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node AnimationControl(Actor actor, float speed, int activity = 1, int flags = 0, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 20,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new ActorProperty(actor.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pActor")
                        },
                        new FloatProperty(speed) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pSpeed")
                        },
                        new EnumProperty(activity) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pActivity")
                        },
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node Random(int flags = 33, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 10,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node FMVStart(string fmvFile, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 1,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new StringProperty((short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(fmvFile)) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFMVFile")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node CutsceneSkipped(int flags = 0, string note = "", int r = 255, int g = 0, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 131,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node ToggleIngameCutscene(bool disableCivs = true, bool subtitles = true, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 100,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new BooleanProperty(disableCivs) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pDisableCivs")
                        },
                        new BooleanProperty(subtitles) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pSubtitles")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node CharacterControl(Actor character, int property, float value, int weapon = 0, int personalityindex = -1, Actor vehicle = null, Actor target = null, string animation = "", short animAction = -1, string padFileName = "", int numPadRecordings = 0, int flags = 0, string note = "", int r = 0, int g = 200, int b = 122)
        {
            if (character == null)
                throw new ScriptRuntimeException("Bad argument #1 - Attempt to control a nil value");
            if (character.TheActor.TypeId != 2)
                throw new ScriptRuntimeException("Bad argument #1 - The inputed actor is not a Character actor type");
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 18,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new ActorProperty(character.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCharacter")
                        },
                        new ActorProperty(vehicle == null ? -1 : vehicle.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pVehicle")
                        },
                        new ActorProperty(target == null ? -1 : target.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pTarget")
                        },
                        new IntegerProperty(personalityindex) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pPersonalityIndex")
                        },
                        new EnumProperty(property) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pProperty")
                        },
                        new EnumProperty(weapon) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWeapon")
                        },
                        new FloatProperty(value) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pValue")
                        },
                        new TextFileItemProperty((short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(animation),(short)animAction) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAnimation")
                        },
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        },
                        new IntegerProperty(numPadRecordings) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pNumPadRecordings")
                        },
                        new StringProperty((short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(padFileName)) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pPadFileName")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        // -
        public Node SetCharacterTakeHealth(Actor character, float amount, int flags = 0, string note = "Character.TakeHealth()", int r = 200, int g = 255, int b = 100)
        {
            return CharacterControl(character, 1, amount, 0, -1, null, null, "", -1, "", 0, flags, note, r, g, b);
        }

        // +
        public Node SetCharacterAddHealth(Actor character, float amount, int flags = 0, string note = "Character.AddHealth()", int r = 200, int g = 255, int b = 100)
        {
            return CharacterControl(character, 2, amount, 0, -1, null, null, "", -1, "", 0, flags, note, r, g, b);
        }
		
		// =
        public Node SetCharacterHealthTo(Actor character, float newHealth, int flags = 0, string note = "Character.SetHealthTo()", int r = 200, int g = 255, int b = 100)
        {
            return CharacterControl(character, 3, newHealth, 0, -1, null, null, "", -1, "", 0, flags, note, r, g, b);
        }	

        // +
        public Node SetCharacterAddFelony(Actor character, float amount, int flags = 0, string note = "Character.AddFelony()", int r = 200, int g = 255, int b = 100)
        {
            return CharacterControl(character, 4, amount, 0, -1, null, null, "", -1, "", 0, flags, note, r, g, b);
        }		
		
		// -
        public Node SetCharacterTakeFelony(Actor character, float amount, int flags = 0, string note = "Character.TakeFelony()", int r = 200, int g = 255, int b = 100)
        {
            return CharacterControl(character, 5, amount, 0, -1, null, null, "", -1, "", 0, flags, note, r, g, b);
        }			

        // =
        public Node SetCharacterFelonyTo(Actor character, float felony, int flags = 0, string note = "Character.SetFelonyTo()", int r = 200, int g = 255, int b = 100)
        {
            return CharacterControl(character, 6, felony, 0, -1, null, null, "", -1, "", 0, flags, note, r, g, b);
        }

        // =
        public Node SetCharacterVulnerability(Actor character, float vulnerability, int flags = 0, string note = "Character.SetVulnerability()", int r = 200, int g = 255, int b = 100)
        {
            return CharacterControl(character, 7, vulnerability, 0, -1, null, null, "", -1, "", 0, flags, note, r, g, b);
        }
		
        public Node SetCharacterUsePadFile(Actor character, Actor vehicle, string fileName, int numRecordings, int flags = 0, string note = "Character.UsePadFile()", int r = 200, int g = 255, int b = 100)
        {
            return CharacterControl(character, 8, 0, 0, -1, vehicle, null, "", -1, fileName, numRecordings, flags, note, r, g, b);
        }		
		
        public Node GiveCharacterWeapon(Actor character, int weapon, float clips, int flags = 0, string note = "Character.GiveWeapon()", int r = 200, int g = 255, int b = 100)
        {
            return CharacterControl(character, 9, clips, weapon, -1, null, null, "", -1, "", 0, flags, note, r, g, b);
        }		

        public Node SetCharacterUnStuckInVehicle(Actor character, int flags = 0, string note = "Character.SetUnStuckInVehicle()", int r = 200, int g = 255, int b = 100)
        {
            return CharacterControl(character, 11, 1, 0, -1, null, null, "", -1, "", 0, flags, note, r, g, b);
        }

        public Node SetCharacterStuckInVehicle(Actor character, int flags = 0, string note = "Character.SetStuckInVehicle()", int r = 200, int g = 255, int b = 100)
        {
            return CharacterControl(character, 12, 1, 0, -1, null, null, "", -1, "", 0, flags, note, r, g, b);
        }
		
		// Unsure if this actually does it
        public Node SetCharacterStopAnimation(Actor character, int flags = 0, string note = "Character.StopAnimation()", int r = 200, int g = 255, int b = 100)
        {
            return CharacterControl(character, 13, 0, 0, -1, null, null, "", -1, "", 0, flags, note, r, g, b);
        }				
		
        public Node SetCharacterPlayAnimation(Actor character, string animationName, short animationId, int flags = 0, string note = "Character.PlayAnimation()", int r = 200, int g = 255, int b = 100)
        {
            return CharacterControl(character, 14, 0, 0, -1, null, null, animationName, animationId, "", 0, flags, note, r, g, b);
        }		
		
		// very buggy 
        public Node SetCharacterForceGetOutVehicleTo(Actor character, float x, float y, float z, float angle = 0, Actor vehicle = null, int flags = 0, string note = "Character.SetPositionTo(float,float,float,float)", int r = 200, int g = 255, int b = 100)
        {
            var a = (Math.PI / 180) * angle; // convert degrees to radians
            Vector3 fwd = new Vector3(
                 0,
                
                (float)(Math.Cos(0) * Math.Cos(a)), // x

                //1, //(float)-Math.Sin(angle), // altitude

                (float)(Math.Cos(0) * Math.Sin(a)) // z
            );

            Actor marker = Marker(x, y, z, fwd.X, fwd.Y, fwd.Z);
            return CharacterControl(character, 17, 0, 0, -1, vehicle, marker, "", -1, "", 0, flags, note, r, g, b);
        }

        public Node SetCharacterForceGetOutVehicleTo(Actor character, Actor actor, Actor vehicle, int flags = 0, string note = "Character.SetPositionTo(Actor)", int r = 200, int g = 255, int b = 100)
        {
            return CharacterControl(character, 17, 0, 0, -1, vehicle, actor, "", -1, "", 0, flags, note, r, g, b);
        }			

        // this one can be also used to teleport you with your vehicle
        public Node SetCharacterPositionTo(Actor character, float x, float y, float z, float angle = 0, Actor vehicle = null, int flags = 0, string note = "Character.SetPositionTo(float,float,float,float)", int r = 200, int g = 255, int b = 100)
        {
            var a = (Math.PI / 180) * angle; // convert degrees to radians
            Vector3 fwd = new Vector3(
                 0,
                
                (float)(Math.Cos(0) * Math.Cos(a)), // x

                //1, //(float)-Math.Sin(angle), // altitude

                (float)(Math.Cos(0) * Math.Sin(a)) // z
            );

            Actor marker = Marker(x, y, z, fwd.X, fwd.Y, fwd.Z);
            return CharacterControl(character, 18, 0, 0, -1, vehicle, marker, "", -1, "", 0, flags, note, r, g, b);
        }

        public Node SetCharacterPositionTo(Actor character, Actor actor, Actor vehicle = null, int flags = 0, string note = "Character.SetPositionTo(Actor)", int r = 200, int g = 255, int b = 100)
        {
            return CharacterControl(character, 18, 0, 0, -1, vehicle, actor, "", -1, "", 0, flags, note, r, g, b);
        }	

        public Node SetCharacterPersonalityIndexTo(Actor character, int personality, int flags = 0, string note = "Character.SetPersonalityIndexTo()", int r = 200, int g = 255, int b = 100)
        {
            return CharacterControl(character, 19, 0, 0, personality, null, null, "", -1, "", 0, flags, note, r, g, b);
        }	

        public Node ClearCharacterWeapons(Actor character, int flags = 0, string note = "Character.Disarm()", int r = 200, int g = 255, int b = 100)
        {
            return CharacterControl(character, 27, 0, 0, -1, null, null, "", -1, "", 0, flags, note, r, g, b);
        }

        public Node SetCharacterGetOutOfVehicle(Actor character, int flags = 0, string note = "Character.GetOutOfVehicle()", int r = 200, int g = 255, int b = 100)
        {
            return CharacterControl(character, 20, 0, 0, -1, null, null, "", -1, "", 0, flags, note, r, g, b);
        }

        // thanks welcome to nice
        public Node CharacterEquipWeapon(Actor character, int flags = 0, string note = "Character.EquipWeapon()", int r = 200, int g = 255, int b = 100)
        {
            return CharacterControl(character, 22, 0, 0, -1, null, null, "", -1, "", 0, flags, note, r, g, b);
        }

        public Node CharacterReholsterWeapon(Actor character, int flags = 0, string note = "Character.ReholsterWeapon()", int r = 200, int g = 255, int b = 100)
        {
            return CharacterControl(character, 23, 0, 0, -1, null, null, "", -1, "", 0, flags, note, r, g, b);
        }

        public Node FailMission(int message, float failDelay = 2.0f, int flags = 0, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 6,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new FloatProperty(failDelay) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFailDelay")
                        },
                        new LocalisedStringProperty(message) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMessage")
                        },
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node MissionComplete(bool nodelay = true, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 5,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new BooleanProperty(nodelay) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pNoDelay")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node VehicleWatch(Actor vehicle, int watchover, float value, int flags = 5, string note = "", int r = 0, int g = 200, int b = 122)
        {
            if (vehicle == null)
                throw new ScriptRuntimeException("Bad argument #1 - Attempt to watch a nil value");
            if (vehicle.TheActor.TypeId != 3)
                throw new ScriptRuntimeException("Bad argument #1 - Inputed actor is not a Vehicle type actor");
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 16,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },         // pWireCollection
                        new ActorProperty(vehicle.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pVehicle")
                        },         // pVehicle
                        new EnumProperty(watchover)
                        {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWatchover")
                        },         // pWatchover
                        new FloatProperty(value) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pValue")
                        },         // pValue
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node CharacterWatch(Actor character, int watchover, float value, Actor obj = null, int flags = 5, string note = "", int r = 0, int g = 200, int b = 122)
        {
            if (character == null)
                throw new ScriptRuntimeException("Bad argument #1 - Attempt to watch a nil value");
            if (character.TheActor.TypeId != 2)
                throw new ScriptRuntimeException("Bad argument #1 - Inputed actor is not a Character type actor");
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 19,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },         // pWireCollection
                        new ActorProperty(character.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCharacter")
                        },         // pCharacter
                        new ActorProperty(obj == null ? -1 : obj.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pObject")
                        },         // pObject
                        new EnumProperty(watchover) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWatchover")
                        },         // pWatchover
                        new FloatProperty(value) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pValue")
                        },         // pValue
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node VehicleWrecked(Actor vehicle, int flags = 5, string note = "Vehicle.Wrecked == true", int r = 128, int g = 0, int b = 255)
        {
            return VehicleWatch(vehicle, 1, 1.0f, flags, note, r, g, b);
        }

        public Node VehicleFlipped(Actor vehicle, int flags = 5, string note = "Vehicle.Flipped == true", int r = 128, int g = 0, int b = 255)
        {
            return VehicleWatch(vehicle, 2, 1.0f, flags, note, r, g, b);
        }

        public Node VehicleOnWater(Actor vehicle, int flags = 5, string note = "Vehicle.OnWater == true", int r = 128, int g = 0, int b = 255)
        {
            return VehicleWatch(vehicle, 7, 1.0f, flags, note, r, g, b);
        }

        public Node CharacterHasDied(Actor character, int flags = 0, string note = "Character.Dead == true", int r = 0, int g = 20, int b = 255)
        {
            return CharacterWatch(character, 1, 0, null, flags, note, r, g, b);
        }

        public Node CharacterWasArrested(Actor character, int flags = 5, string note = "Character.Arrested == true", int r = 0, int g = 20, int b = 255)
        {
            return CharacterWatch(character, 3, 0, null, flags, note, r, g, b);
        }

        public Node CharacterIsBeingChased(Actor character, int flags = 14, string note = "Character.Chased == true", int r = 0, int g = 20, int b = 255)
        {
            return CharacterWatch(character, 9, 0, null, flags, note, r, g, b);
        }

        public Node CharacterEnterExitedVehicle(Actor character, Actor vehicle, int flags = 4, string note = "Character.EnteredExitedCar ...", int r = 90, int g = 20, int b = 255)
        {
            return CharacterWatch(character, 4, 0, vehicle, flags, note, r, g, b);
        }

        public Node ActorCreation(Actor actor, int activity = 1, int flags = 0, string note = "", int r = 0, int g = 200, int b = 122)
        {
            if (actor == null)
                throw new ScriptRuntimeException("Bad argument #1 - Attempt to give creation to a nil value");
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 101,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },         // pWireCollection
                        new ActorProperty(actor.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pActor")
                        },         // pActor
                        new IntegerProperty(activity) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pActivity")
                        },         // pActivity
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node FadeScreen(float duration, float fadeToColorTime, float fadeFromColorTime = 0, byte R = 0, byte G = 0, byte B = 0, byte A = 0, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 124,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },         // pWireCollection
                        new FloatProperty(fadeToColorTime) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFadeToColourTime")
                        },         // pFadeToColourTime
                        new FloatProperty(fadeFromColorTime) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFadeFromColourTime")
                        },         // pFadeFromColourTime
                        new FloatProperty(duration) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pDuration")
                        },          // pDuration
                        new Float3Property(new Vector4(R,G,B,A)) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pColour")
                        }          // pColour
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node FadeOut(float duration = 0.3f, byte R = 0, byte G = 0, byte B = 0, byte A = 0, string note = "FadeOut()", int r = 255, int g = 200, int b = 128)
        {
            return FadeScreen(duration, 0, duration, R, G, B, A, note, r, g, b);
        }

        public Node FadeIn(float duration = 0.3f, byte R = 0, byte G = 0, byte B = 0, byte A = 0, string note = "FadeIn()", int r = 255, int g = 200, int b = 128)
        {
            return FadeScreen(duration, duration, 0, R, G, B, A, note, r, g, b);
        }

        public Node CopControl(int chasers, int type = 1, int aggresion = 5, int armor = 2, int goonWeapon1 = 1, int goonWeapon2 = 0, int goonWeapon3 = 0, int flags = 524290, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 102,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                         new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                         },      // pWireCollection
                         new EnumProperty(chasers) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pChasers")
                         },          // pChasers
                         new EnumProperty(1) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pType")
                         },                // pType
                         new EnumProperty(5) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAggression")
                         },                // pAggression
                         new EnumProperty(2) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pArmour")
                         },                // pArmour
                         new EnumProperty(1) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pGoonWeapon1")
                         },                // pGoonWeapon1
                         new EnumProperty(0) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pGoonWeapon2")
                         },                // pGoonWeapon2
                         new EnumProperty(0) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pGoonWeapon3")
                         },                // pGoonWeapon3
                         new FlagsProperty(524290) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                         }           // pFlags
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node CivilianTrafficControl(float pingInRadius = 130, float pingOutRadius = 135, int density = 0, int parkedCarDensity = 0, int attractorParkedCarDensity = -3, int flags = 65536, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 104,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                         new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                         },      // pWireCollection
                         new EnumProperty(density) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pDensity")
                         },
                         new EnumProperty(parkedCarDensity) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pParkedCarDensity")
                         },
                         new EnumProperty(attractorParkedCarDensity) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAttractorParkedCarDensity")
                         },
                         new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                         },           
                         new FloatProperty(pingInRadius)
                         {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pPingInRadius")
                         },
                         new FloatProperty(pingOutRadius)
                         {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pPingOutRadius")
                         }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node BombCar(Actor vehicle, float minimumSpeed = 22.36f, int graceTime = 5, int bank = 45, int sample = 0, string note = "", int r = 0, int g = 200, int b = 122)
        {
            if (vehicle == null)
                throw new ScriptRuntimeException("Bad argument #1 - Attempt to set bomb car to a nil value");
            if (vehicle.TheActor.TypeId != 3)
                throw new ScriptRuntimeException("Bad argument #1 - The inputed actor is not a Vehicle type actor");
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 122,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                         new WireCollectionProperty(pWireCollection) {
                             StringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                         },      // pWireCollection
                         new ActorProperty(vehicle.index) {
                             StringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pVehicle")
                         },               // pVehicle
                         new FloatProperty(minimumSpeed) {
                             StringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMinimumSpeed")
                         },          // pMinimumSpeed
                         new IntegerProperty(graceTime) {
                             StringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pGraceTime")
                         },             // pGraceTime
                         new AudioProperty(bank,sample) {
                             StringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWarningAudio")
                         }             // pWarningAudio
                    }
            });
            // make the mission load the inputed audio bank
            missionData.LogicData.SoundBankTable.AppendBank(bank);

            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        // activity = 2 (destroy/deload)
        public Node DestroyActor(Actor actor, int flags = 0, string note = "", int r = 0, int g = 200, int b = 122)
        {
            return ActorCreation(actor, 2, flags, note, r, g, b);
        }

        public Actor CreateAIPath(Actor character, Table pathTable, float speed = 1, float burnoutTime = 0, int desiredTransport = 0, int flags = 1, string note = "", int r = 100, int g = 128, int b = 255)
        {
            if (character == null)
                throw new ScriptRuntimeException("Bad argument #1 - Attempt to create AI path for a nil value");
            if (character.TheActor.TypeId != 2)
                throw new ScriptRuntimeException("Bad argument #1 - The inputed actor is not a Character actor type");
            if (pathTable.Length == 0)
                throw new ScriptRuntimeException("Bad argument #2 - Path Vector3 table can't be empty!");
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            missionData.LogicData.Actors.Definitions.Add(new ActorDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 6,
                StringId = stringId,
                ObjectId = missionData.Objects.Objects.Count,
                Flags = 0x72D5,
                Properties = new List<NodeProperty>
                    {
                        new ActorProperty(character.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCharacter")
                        },           // pCharacter
                        new FloatProperty(speed) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pSpeed")
                        },           // pSpeed
                        new IntegerProperty(desiredTransport) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pDesiredTransport")
                        },           // pDesiredTransport
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }             // pFlags 
                    }
            });
            Vector4[] path = new Vector4[pathTable.Length];

            for (int pathId = 0; pathId < pathTable.Length + 1; pathId++)
            {
                DynValue vector = pathTable.Get(pathId);
                if (vector.IsNotNil())
                {
                    if (vector.Table==null)
                    {
                        throw new ScriptRuntimeException($"Bad argument #2 - Path table must be a table containing tables in a Vector3 format");
                    }

                    if (vector.Table.Get(1).IsNotNil() & vector.Table.Get(2).IsNotNil() & vector.Table.Get(3).IsNotNil())
                    {
                        Vector4 vec;
                        vec = new Vector4((float)vector.Table.Get(1).Number, (float)vector.Table.Get(2).Number, (float)vector.Table.Get(3).Number, 1.0f);
                        path[pathId - 1] = vec;
                    }
                    else
                    {
                        throw new ScriptRuntimeException($"Bad argument #2 - Path table index {pathId} has some invalid Vector3 values (nil)");
                    }
                }
            }

            missionData.Objects.Objects.Add(new PathObject()
            {
                CreationData = new byte[]
                    {
                        1, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                        0,0,0,0,      0,0,0,0,     0,0,0,0,     0,0,0x3F,0x80
                    },
                Path = path
            });

            int idx = missionData.LogicData.Actors.Definitions.Count - 1;
            return new Actor(missionData.LogicData.Actors.Definitions[idx], idx);
        }

        public Node SetChaseVehicle(Actor vehicle, float health, int text, bool invicibleTyres = true, float bulletDamage = 0.0875f, float minForce = 3000, float maxForce = 25000, string note = "", int r = 155, int g = 0, int b = 255)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 137,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        // I'm not sure if it was also supposed to remove
                        new ActorProperty(vehicle == null ? -1 : vehicle.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pVehicle")
                        },
                        new FloatProperty(minForce) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMinForce")
                        },
                        new FloatProperty(maxForce) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMaxForce")
                        },
                        new FloatProperty(health) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pHealth")
                        },
                        new LocalisedStringProperty(text)
                        {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pText")
                        },
                        new BooleanProperty(invicibleTyres)
                        {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pText")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node FollowPath(Actor aiPath, string note = "", int r = 155, int g = 55, int b = 0)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 138,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },         // pWireCollection
                        new ActorProperty(aiPath.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pPath")
                        }
                    }
            });

            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Actor TestVolume(float x, float y, float z, Actor attachedTo = null, float radius = 0, int flags = 1, string note = "", int r = 100, int g = 128, int b = 255)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            missionData.LogicData.Actors.Definitions.Add(new ActorDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 4,
                StringId = stringId,
                ObjectId = missionData.Objects.Objects.Count,
                Flags = 0x72D5,
                Properties = new List<NodeProperty>
                    {
                        new ActorProperty(attachedTo == null ? -1 : attachedTo.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAttachTo")
                        },           // pAttachTo
                        new FloatProperty(radius) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pRadius")
                        },           // pRadius
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }             // pFlags 
                    }
            });
            missionData.Objects.Objects.Add(new VolumeObject() { Position = new Vector3((float)x, (float)y, (float)z) });

            int idx = missionData.LogicData.Actors.Definitions.Count - 1;
            return new Actor(missionData.LogicData.Actors.Definitions[idx], idx);
        }

        public Actor Marker(float x1, float y1, float z1, float x2 = 1, float y2 = 0, float z2 = 0, string note = "", int r = 100, int g = 128, int b = 255)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            missionData.LogicData.Actors.Definitions.Add(new ActorDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 105,
                StringId = stringId,
                ObjectId = missionData.Objects.Objects.Count,
                Flags = 0x72D5,
                Properties = new List<NodeProperty>
                    {
                       // yes, nothing :)
                    }
            });
            missionData.Objects.Objects.Add(new MarkerObject() { Position = new Vector3((float)x1, (float)y1, (float)z1), V1 = new Vector3((float)x2, (float)y2, (float)z2) });

            int idx = missionData.LogicData.Actors.Definitions.Count - 1;
            return new Actor(missionData.LogicData.Actors.Definitions[idx], idx);
        }

        public Actor CreateArea(float x, float y, float z,  float areaX, float areaY, float areaZ, float areaW = 1.0f,   float w = 1.0f, Actor attachedTo = null, float radius = 0, int flags = 1, string note = "", int r = 100, int g = 128, int b = 255)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            missionData.LogicData.Actors.Definitions.Add(new ActorDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 4,
                StringId = stringId,
                ObjectId = missionData.Objects.Objects.Count,
                Flags = 0x72D5,
                Properties = new List<NodeProperty>
                    {
                        new ActorProperty(attachedTo == null ? -1 : attachedTo.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAttachTo")
                        },           // pAttachTo
                        new FloatProperty(radius) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pRadius")
                        },           // pRadius
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }             // pFlags 
                    }
            });
            MemoryStream stream = new MemoryStream(0x40);
            using (var f = new BinaryWriter(stream, Encoding.UTF8))
            {
                f.Write((long)1);
                f.Write((long)0);

                f.Write(areaX);
                f.Write(areaY);
                f.Write(areaZ);
                f.Write(areaW);
                f.Write(x);
                f.Write(y);
                f.Write(z);
                f.Write(w);

                f.Write((int)1);
                f.Write((int)524292);
                f.Write((long)0xA161BD0010101B8);
            }
            missionData.Objects.Objects.Add(new AreaObject() { CreationData = stream.GetBuffer() });
            stream.Dispose();

            int idx = missionData.LogicData.Actors.Definitions.Count - 1;
            return new Actor(missionData.LogicData.Actors.Definitions[idx], idx);
        }

        // AnimProp, animated prop
        public Actor GetAnimatedObjectAt(float x, float y, float z, int type = 0, string note = "", int r = 200, int g = 128, int b = 128)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            missionData.LogicData.Actors.Definitions.Add(new ActorDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 104,
                StringId = stringId,
                ObjectId = missionData.Objects.Objects.Count,
                Flags = 0x72D5,
                Properties = new List<NodeProperty>
                    {
                        new EnumProperty(type) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pType")
                        }           // pType
                    }
            });
            missionData.Objects.Objects.Add(new AnimPropObject() { Position = new Vector3((float)x, (float)y, (float)z) });

            int idx = missionData.LogicData.Actors.Definitions.Count - 1;
            return new Actor(missionData.LogicData.Actors.Definitions[idx], idx);
        }

        public Actor CreateSwitch(float x, float y, float z, float angle, int type = 10, string note = "", int r = 200, int g = 128, int b = 128)
        {
            var a = (Math.PI / 180) * angle; // convert degrees to radians
            Vector4 fwd = new Vector4(
                (float)(Math.Cos(0) * Math.Cos(a)), // x

                0, //(float)-Math.Sin(angle), // altitude

                (float)(Math.Cos(0) * Math.Sin(a)), // z
                
                0.0333f // scale?
            );

            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            missionData.LogicData.Actors.Definitions.Add(new ActorDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 101,
                StringId = stringId,
                ObjectId = missionData.Objects.Objects.Count,
                Flags = 0x72D5,
                Properties = new List<NodeProperty>
                    {
                        new EnumProperty(type) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pType")
                        }           // pType
                    }
            });
            missionData.Objects.Objects.Add(new SwitchObject() { Position = new Vector3((float)x, (float)y, (float)z), Direction = fwd });

            int idx = missionData.LogicData.Actors.Definitions.Count - 1;
            return new Actor(missionData.LogicData.Actors.Definitions[idx], idx);
        }

        public Node ActorHasReachedPoint(Actor actor, float x, float y, float z, float radius, int flags = 1, string note = "ActorHasReachedPoint", int r = 100, int g = 128, int b = 255)
        {
            if (actor == null)
                throw new ScriptRuntimeException("Bad argument #1 - Attempt to check point from a nil value");
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            int testVolumeidx = missionData.LogicData.Actors.Definitions.Count;

            // test volume
            missionData.LogicData.Actors.Definitions.Add(new ActorDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 4,
                StringId = stringId,
                ObjectId = missionData.Objects.Objects.Count,
                Flags = 0x72D5,
                Properties = new List<NodeProperty>
                    {
                        new ActorProperty(-1) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAttachTo")
                        },           // pAttachTo
                        new FloatProperty(radius) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pRadius")
                        },           // pRadius
                        new FlagsProperty(1) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }             // pFlags 
                    }
            });
            // the format is different from camera Look At target, sort of....
            missionData.Objects.Objects.Add(new VolumeObject() { Position = new Vector3((float)x, (float)z, (float)radius) });

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 12,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },         // pWireCollection
                        new ActorProperty(testVolumeidx) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pArea")
                        },         // pArea
                        new ActorProperty(actor.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pActor")
                        },         // pActor
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }          // pFlags
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node AreaWatch(Actor actor, Actor area, int flags = 1, string note = "ActorHasReachedPoint", int r = 100, int g = 128, int b = 255)
        {
            if (area == null)
                throw new ScriptRuntimeException("Bad argument #1 - Attempt to watch a nil value");
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 12,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },         // pWireCollection
                        new ActorProperty(area.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pArea")
                        },         // pArea
                        new ActorProperty(actor.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pActor")
                        },         // pActor
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }          // pFlags
                    }
            });

            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node Timer(float interval, int flags = 1, string note = "", int r = 155, int g = 55, int b = 0)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 3,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },         // pWireCollection
                        new FloatProperty(interval) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pInterval")
                        },         // pInterval
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }          // pFlags
                    }
            });

            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node DebugText(string message, int flags = 1, string note = "", int r = 155, int g = 55, int b = 0)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 2,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },         // pWireCollection
                        new StringProperty((short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(message)) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMessage")
                        },         // pInterval
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }          // pFlags
                    }
            });

            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node PlayAudio(int bank, int sample, Actor actor = null, int flags = 1, string note = "", int r = 100, int g = 0, int b = 255)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 27,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },         // pWireCollection
                        new AudioProperty(bank,sample) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAudio")
                        },         // pAudio
                        new ActorProperty(actor == null ? -1 : actor.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pActor")
                        },
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }          // pFlags
                    }
            });
            // make the mission load the inputed audio bank
            missionData.LogicData.SoundBankTable.AppendBank(bank);

            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node MusicControl(int type, int flags = 1, string note = "", int r = 55, int g = 100, int b = 255)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 118,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },         // pWireCollection
                        new EnumProperty(type) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMusicType")
                        },         // pMusicType
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }          // pFlags
                    }
            });

            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node CharacterDriveTo(Actor character, float x, float y, float z, float driveSpeed, int desiredTransport = 2, Actor attachTo = null, int flags = 0, string note = "CharacterDriveTo", int r = 128, int g = 0, int b = 255)
        {
            if (character.TheActor.TypeId != 2)
                throw new ScriptRuntimeException("Bad argument #1 - The inputed actor is not a Character type actor");
            if (character == null)
                throw new ScriptRuntimeException("Bad argument #1 - Actor expected got nil");
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            // ai target
            missionData.LogicData.Actors.Definitions.Add(new ActorDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 7,
                StringId = stringId,
                ObjectId = missionData.Objects.Objects.Count,
                Properties = new List<NodeProperty>
                    {
                        new ActorProperty(character.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCharacter")
                        },            // pCharacter
                        new ActorProperty(attachTo == null ? -1 : character.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAttachTo")
                        },           // pAttachTo
                        new FloatProperty(1f) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWalkSpeed")
                        },           // pWalkSpeed
                        new FloatProperty(driveSpeed) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pVehicleSpeed")
                        },           // pVehicleSpeed
                        new EnumProperty(desiredTransport) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pDesiredTransport")
                        },             // pDesiredTransport
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }             // pFlags 
                    }
            });
            missionData.Objects.Objects.Add(new VolumeObject() { Position = new Vector3((float)x, (float)y, (float)z) });


            int idx = missionData.LogicData.Actors.Definitions.Count - 1;
            return ActorCreation(new Actor(missionData.LogicData.Actors.Definitions[idx], idx));
        }
		
        public Node CharacterDriveAroundArea(Actor character, Actor area, float driveSpeed, int desiredTransport = 2, int flags = 0, string note = "CharacterDriveTo", int r = 128, int g = 0, int b = 255)
        {
            if (character.TheActor.TypeId != 2)
                throw new ScriptRuntimeException("Bad argument #1 - The inputed actor is not a Character type actor");
            if (character == null)
                throw new ScriptRuntimeException("Bad argument #1 - Actor expected got nil");
            if (area.TheActor.TypeId != 4)
                throw new ScriptRuntimeException("Bad argument #2 - The inputed actor is not a Area type actor");
            if (area == null)
                throw new ScriptRuntimeException("Bad argument #2 - Actor expected got nil");
            if (driveSpeed == null)
                throw new ScriptRuntimeException("Bad argument #3 - Number expected got nil");	
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            // ai target
            missionData.LogicData.Actors.Definitions.Add(new ActorDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 7,
                StringId = stringId,
                ObjectId = area.TheActor.ObjectId,
                Properties = new List<NodeProperty>
                    {
                        new ActorProperty(character.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCharacter")
                        },            // pCharacter
                        new ActorProperty(-1) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAttachTo")
                        },           // pAttachTo
                        new FloatProperty(1f) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWalkSpeed")
                        },           // pWalkSpeed
                        new FloatProperty(driveSpeed) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pVehicleSpeed")
                        },           // pVehicleSpeed
                        new EnumProperty(desiredTransport) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pDesiredTransport")
                        },             // pDesiredTransport
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }             // pFlags 
                    }
            });
            
			int idx = missionData.LogicData.Actors.Definitions.Count - 1;
            return ActorCreation(new Actor(missionData.LogicData.Actors.Definitions[idx], idx));
        }		

        // chaserType
        // 0 (default) = chases the actor but uses patrolling cop icon
        // 1 = chases the actor but uses red alerted cop icon
        // 2 = chases the actor but uses red alerted cop icon and goes faster and ram the target
        public Node CharacterChaseActor(Actor character, Actor actor, float driveSpeed, int chaserType = 0, string note = "CharacterDriveToActor", int r = 128, int g = 0, int b = 255)
        {
            int flags = 25690112; 
            switch (chaserType)
            {
                case 0:
                default:
                    flags = 25690112;
                    break;
                case 1:
                    flags = 8912896;
                    break;
                case 2:
                    flags = 9961472;
                    break;
            }
            return CharacterDriveFollowingActor(character, actor, driveSpeed, 2, flags, note, r,g,b);
        }

        public Actor AITarget(Actor character, float x, float y, float z, float walkSpeed, float driveSpeed, int desiredTransport, Actor attachTo = null, int flags = 0, string note = "", int r = 128, int g = 0, int b = 255)
        {
            if (character.TheActor.TypeId != 2)
                throw new ScriptRuntimeException("Inputed actor is not a Character type actor");
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            missionData.LogicData.Actors.Definitions.Add(new ActorDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 7,
                StringId = stringId,
                ObjectId = missionData.Objects.Objects.Count,
                Properties = new List<NodeProperty>
                    {
                        new ActorProperty(character.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCharacter")
                        },            // pCharacter
                        new ActorProperty(attachTo == null ? -1 : attachTo.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAttachTo")
                        },           // pAttachTo
                        new FloatProperty(walkSpeed) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWalkSpeed")
                        },           // pWalkSpeed
                        new FloatProperty(driveSpeed) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pVehicleSpeed")
                        },           // pVehicleSpeed
                        new EnumProperty(desiredTransport) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pDesiredTransport")
                        },             // pDesiredTransport
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }             // pFlags 
                    }
            });
			/*
            MemoryStream creationData = new MemoryStream();
            creationData.SetLength(0x40);

            using (var f = new BinaryWriter(creationData, Encoding.UTF8))
            {
                f.Write((int)1);
                f.Write((int)1);
                f.Write((long)0);

                f.Write((float)2);
                f.Write((float)2);
                f.Write((float)2);
                f.Write((float)0);

                f.Write(x);
                f.Write(y);
                f.Write(z);
                f.Write((float)1);

                f.Write((float)0);
                f.Write((float)0);
                f.Write((float)0);
                f.Write(2.952072f);
            }
            byte[] data = new byte[0x40];
            Array.Copy(creationData.GetBuffer(), data, data.Length);

            missionData.Objects.Objects.Add(new AreaObject() { CreationData = data });
            */
			missionData.Objects.Objects.Add(new VolumeObject() { Position = new Vector3((float)x, (float)y, (float)z) });

            int idx = missionData.LogicData.Actors.Definitions.Count - 1;
            return new Actor(missionData.LogicData.Actors.Definitions[idx], idx);
        }

        public Actor SpecialEffect(float x, float y, float z, int type, float force, float range, int time, Actor attachTo = null, int flags = 0, string note = "", int r = 128, int g = 255, int b = 100)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            missionData.LogicData.Actors.Definitions.Add(new ActorDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 8,
                StringId = stringId,
                ObjectId = missionData.Objects.Objects.Count,
                Properties = new List<NodeProperty>
                    {
                        new EnumProperty(type) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pType")
                        }, 
                        new ActorProperty(attachTo == null ? -1 : attachTo.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAttachTo")
                        },          
                        new FloatProperty(force) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pForce")
                        },           
                        new FloatProperty(range) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pRange")
                        },           
                        new IntegerProperty(time) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pTime")
                        },         
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }            
                    }
            });
            MemoryStream creationData = new MemoryStream();
            creationData.SetLength(0x40);

            using (var f = new BinaryWriter(creationData, Encoding.UTF8))
            {
                f.Write((int)1);
                f.Write((int)1);
                f.Write((long)0);

                f.Write((float)range);
                f.Write((float)range);
                f.Write((float)range);
                f.Write((float)0);

                f.Write(x);
                f.Write(y);
                f.Write(z);
                f.Write((float)1);

                f.Write((float)0);
                f.Write((float)0);
                f.Write((float)0);
                f.Write(2.952072f);
            }
            byte[] data = new byte[0x40];
            Array.Copy(creationData.GetBuffer(), data, data.Length);

            missionData.Objects.Objects.Add(new AreaObject() { CreationData = data });

            int idx = missionData.LogicData.Actors.Definitions.Count - 1;
            return new Actor(missionData.LogicData.Actors.Definitions[idx], idx);
        }

        // follows the actor
        public Node CharacterDriveFollowingActor(Actor character, Actor actor, float driveSpeed, int desiredTransport = 2, int flags = 0, string note = "CharacterDriveFollowingActor", int r = 128, int g = 0, int b = 255)
        {
            Node driveTo = CharacterDriveTo(character, 0, 0, 0, driveSpeed, desiredTransport, actor, flags, note, r, g, b);
            //var idx = driveTo.TheNode.Properties[1].Value;
            //Actor aiTarget = new Actor(missionData.LogicData.Actors.Definitions[(int)idx], (int)idx);
            //aiTarget.TheActor.Properties[1].Value = actor.index;

            return driveTo;
        }

        public Node CameraSelect(Actor target, int cameraType, float duration = 0, float blendTime = 0, float thrillCamZoom = 1, float thrillCamSpeed = 1, float thrillCamBlur = 0, int flags = 0, string note = "CameraSelect()", int r = 128, int g = 0, int b = 255)
        {
            if (target == null)
                throw new ScriptRuntimeException("Bad argument #1 - Camera target can't be a nil value");
            if (cameraType == null)
                throw new ScriptRuntimeException("Bad argument #2 - Camera type can't be a nil value");
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 14,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                         new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        }, // pWireCollection
                         new ActorProperty(target.index){
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pTarget")
                        },    // pTarget        
                         new EnumProperty(cameraType){
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCameraType")
                        },     // pCameraType
                         new FloatProperty(duration){
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pDuration")
                        }, // pDuration
                         new FloatProperty(blendTime){
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pBlendTime")
                        },   // pBlendTime
                         new FloatProperty(thrillCamZoom){
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pThrillCamZoom")
                        },   // pThrillCamZoom
                         new FloatProperty(thrillCamSpeed){
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pThrillCamSpeed")
                        }, // pThrillCamSpeed
                         new FloatProperty(thrillCamBlur){
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pThrillCamBlur")
                        },   // pThrillCamBlur
                         new FlagsProperty(flags){
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }     // pFlags
                    }
            });

            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node DisplayMessage(int localisedStringId, int duration, int priority = 100, int flags = 65536, string note = "DisplayMessage()", int r = 55, int g = 255, int b = 0)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 24,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },         // pWireCollection
                        new LocalisedStringProperty(localisedStringId) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMessage")
                        },         // pMessage
                        new IntegerProperty(duration) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pTimer")
                        },         // pTimer
                        new EnumProperty(priority) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pPriority")
                        },         // pPriority
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }          // pFlags
                    }
            });

            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node OverlayClockWatch(int value, int flags = 1, string note = "", int r = 255, int g = 255, int b = 0)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 29,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },         // pWireCollection
                        new IntegerProperty(value) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pValue")
                        },         // pValue
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }          // pFlags
                    }
            });

            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node CountdownIntro(int value, int command = 2, int flags = 1, string note = "", int r = 255, int g = 255, int b = 0)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 28,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },         // pWireCollection
                        new IntegerProperty(value) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pValue")
                        },         // pValue
                        new EnumProperty(command) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCommand")
                        },         // pCommand
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }          // pFlags
                    }
            });

            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node ShowClock(int defaultTime, int flags = 1, string note = "", int r = 255, int g = 255, int b = 0)
        {
            return CountdownIntro(defaultTime, 2, flags = 1, note, r, g, b);
        }

        // direction
        // 0 = countdown
        // 1 = timer
        public Node StartClock(uint direction = 0, int flags = 0, string note = "", int r = 255, int g = 101, int b = 200)
        {
            return CountdownIntro(0, direction == 0 ? 4 : 1, flags, note, r, g, b);
        }

        public Node StopClock(int flags = 0, string note = "", int r = 255, int g = 100, int b = 255)
        {
            return CountdownIntro(0, 5, flags, note, r, g, b);
        }

        public Node HasClockRunnedOut(string note = "", int r = 255, int g = 244, int b = 129)
        {
            return OverlayClockWatch(0, 1, note, r, g, b);
        }

        public Actor CreateObjectiveIcon(float x, float y, float z, int height = 1, int maxAdaptiveHeight = 0, int type = 0, bool canGoUp = true, string note = "", int r = 255, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }
            missionData.LogicData.Actors.Definitions.Add(new ActorDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 5,
                StringId = stringId,
                ObjectId = missionData.Objects.Objects.Count,
                Properties = new List<NodeProperty>
                    {
                        new ActorProperty(-1) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAttachTo")
                        },           // pAttachTo
                        new IntegerProperty(height) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pHeight")
                        },          // pHeight
                        new IntegerProperty(maxAdaptiveHeight) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMaxAdpativeHeight")
                        },          // pMaxAdpativeHeight
                        new FlagsProperty(canGoUp ? 131072 : 1) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }        // pFlags 
                    }
            });
            missionData.Objects.Objects.Add(new ObjectiveIconObject()
            {
                Type = type,
                Position = new Vector3((float)x, (float)y, (float)z)
            });
            int idx = missionData.LogicData.Actors.Definitions.Count - 1;
            return new Actor(missionData.LogicData.Actors.Definitions[idx], idx); // returns the actor
        }

        public Actor CreateObjectiveIconAttachedTo(Actor actor, int height = 1, int maxAdaptiveHeight = 0, int type = 0, bool canGoUp = true, string note = "", int r = 255, int g = 200, int b = 122)
        {
            Actor objectiveIcon = CreateObjectiveIcon(0, height, 0, height, maxAdaptiveHeight, type, canGoUp, note, r, g, b);
            objectiveIcon.TheActor.Properties[0].Value = (int)actor.index;
            return objectiveIcon;
        }

        public Actor CreateObjectiveIconAttachedTo(Actor actor, float x = 0, int height = 1, float z = 0, int maxAdaptiveHeight = 0, int type = 0, bool canGoUp = true, string note = "", int r = 255, int g = 200, int b = 122)
        {
            Actor objectiveIcon = CreateObjectiveIcon(x, height, z, height, maxAdaptiveHeight, type, canGoUp, note, r, g, b);
            objectiveIcon.TheActor.Properties[0].Value = (int)actor.index;
            return objectiveIcon;
        }

        public Actor CreateVehicle(float x, float y, float z, float angle, int vehicleId, int colorId, string note = "", int flags = 302186497, float damage = 0, float softness = 1, float weight = 1f, float fragility = 1, int r = 0, int g = 200, int b = 122)
        {
            var a = (Math.PI / 180) * (angle); // convert degrees to radians
            // convert player/character angle to vehicle angle
            a = (a / 2) + 45; // TODO: subtract this with 26
            a -= 26;
            Vector3 fwd = new Vector3(
                (float)(Math.Cos(0) * Math.Cos(a)), // x

                0, //(float)-Math.Sin(angle), // altitude

                (float)(Math.Cos(0) * Math.Sin(a)) // z
            );
            /*
            Vector3 rt = new Vector3(
                (float)Math.Cos(a),0, (float)-Math.Sin(a)
            );
            */
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }
            missionData.LogicData.Actors.Definitions.Add(new ActorDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 3,
                StringId = stringId,
                ObjectId = missionData.Objects.Objects.Count,
                Properties = new List<NodeProperty>
                    {
                        new FloatProperty((float)weight) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWeight")
                        },         // pWeight
                        new FloatProperty((float)softness) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pSoftness")
                        },         // pSoftness
                        new FloatProperty((float)fragility) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pIFragility")
                        },         // pIFragility
                        new FloatProperty(1.0f) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pDemoOnlySoftness")
                        },         // pDemoOnlySoftness
                        new IntegerProperty((int)colorId) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pTintValue")
                        },          // pTintValue
                        new FlagsProperty((int)flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }     // pFlags 
                    }
            });
            byte[] bX = BitConverter.GetBytes((float)x);
            byte[] bY = BitConverter.GetBytes((float)y);
            byte[] bZ = BitConverter.GetBytes((float)z);
            byte[] bFX = BitConverter.GetBytes((float)fwd.X);
            byte[] bFY = BitConverter.GetBytes((float)fwd.Y);
            byte[] bFZ = BitConverter.GetBytes((float)fwd.Z);
            byte[] bDMG = BitConverter.GetBytes((float)damage);
            //byte[] bRX = BitConverter.GetBytes((float)rt.X);
            //byte[] bRY = BitConverter.GetBytes((float)rt.Y);
            //byte[] bRZ = BitConverter.GetBytes((float)rt.Z);
            missionData.Objects.Objects.Add(new VehicleObject()
            {
                UID = (int)vehicleId, // vehicle Id
                CreationData = new byte[]
                    {
                        4,0x0C,0x28,0x0, // UID maybe?
                        0x0C,0x0,0x4C, // magic
                        0x0,0x0,0x0,0x0,0x0, // zeros......
                        0x01,0x1C,0,0, // part 2 magic
                        // direction of which the vehicle is facing to (i guess)
                        0,0,0x80,0x3F,   0,0,0,0,   0,0,0,0,   0,0,0x80,0x3F,
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
                        0,0,0,0,    0,0,0,0,     0,0,0,0,   0,0,0x0,0x0,
                        // AND..... the end!
                        0,0,0x0,0x0,  bDMG[0],bDMG[1],bDMG[2],bDMG[3],    0,0,0,0
                },
                Position = new DSCript.Vector4((float)x, (float)y, (float)z, angle)
            });

            // tell the mission script the vehicle ID to spool up
            if (SpoolVehicles.IndexOf(vehicleId) == -1)
            {
                SpoolVehicles.Add(vehicleId);
            }

            int idx = missionData.LogicData.Actors.Definitions.Count - 1;
            return new Actor(missionData.LogicData.Actors.Definitions[idx], idx); // returns the actor
        }

        public Actor CreateCollectable(float x, float y, float z, int type, int clips, float rotation = 0, string note = "", int flags = 1, byte r = 200, byte g = 0, int b = 255)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }
            missionData.LogicData.Actors.Definitions.Add(new ActorDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 103,
                StringId = stringId,
                ObjectId = missionData.Objects.Objects.Count,
                Flags = 0x2FD7,
                Properties = new List<NodeProperty>()
                {
                        new IntegerProperty((int)clips) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pClips")
                        },          // pClips
                        new FlagsProperty((int)flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }           // pFlags 
                }
            });
            missionData.Objects.Objects.Add(new CollectableObject()
            {
                Type = (CollectableType)type,
                Rotation = rotation,
                Position = new DSCript.Vector4((float)x, (float)y, (float)z, 1)
            });
            int idx = missionData.LogicData.Actors.Definitions.Count - 1;
            return new Actor(missionData.LogicData.Actors.Definitions[idx], idx); // returns the actor
        }

        public Actor CreateCharacter(float x, float y, float z, float angle, uint skin, int role, string personality, int personalityId, int personalityIndex, float health, float felony, int weapon, float vulnerability, string note = "", int flags = 131073, byte r = 0, byte g = 155, int b = 200)
        {
            var a = (Math.PI / 180) * angle; // convert degrees to radians
            Vector3 fwd = new Vector3(
                (float)(Math.Cos(0) * Math.Cos(a)), // x

                0, //(float)-Math.Sin(angle), // altitude

                (float)(Math.Cos(0) * Math.Sin(a)) // z
            );
            // automatic: if role == 1 (player) then set start position to this character position
            if (role == 1)
                missionSummary.StartPosition = new Vector2(x, z);
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }
            missionData.LogicData.Actors.Definitions.Add(new ActorDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 2,
                StringId = stringId,
                ObjectId = missionData.Objects.Objects.Count,
                Flags = 0x2FD7,
                Properties = new List<NodeProperty>()
                {
                        new IntegerProperty((int)role) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pRole")
                        },          // pRole
                        new TextFileItemProperty((short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(personality),(short)personalityId) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pPersonality")
                        },          // pPersonality
                        new IntegerProperty(personalityIndex) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pPersonalityIndex")
                        },          // pPersonalityIndex
                        new FloatProperty((float)health) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pHealth")
                        },          // pHealth
                        new FloatProperty((float)felony) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFelony")
                        },          // pFelony
                        new EnumProperty((int)weapon) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWeapon")
                        },          // pWeapon
                        new FloatProperty((float)vulnerability) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pVulnerability")
                        },          // pVulnerability
                        new FlagsProperty((int)flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }           // pFlags 
                }
            });
            byte[] bX = BitConverter.GetBytes((float)x);
            byte[] bY = BitConverter.GetBytes((float)y);
            byte[] bZ = BitConverter.GetBytes((float)z);
            byte[] bFX = BitConverter.GetBytes((float)fwd.X);
            byte[] bFY = BitConverter.GetBytes((float)fwd.Y);
            byte[] bFZ = BitConverter.GetBytes((float)fwd.Z);
            missionData.Objects.Objects.Add(new CharacterObject()
            {
                UID = (int)-1073623027, // 0xC001D00D
                SkinId = skin,
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
                Position = new DSCript.Vector4((float)x, (float)y, (float)z, angle)
            });
            int idx = missionData.LogicData.Actors.Definitions.Count - 1;
            return new Actor(missionData.LogicData.Actors.Definitions[idx], idx); // returns the actor
        }
        /*
        // uint support
        public Actor CreateCharacter(float x, float y, float z, float angle, uint skin, int role, string personality, int personalityId, int personalityIndex, float health, float felony, int weapon, float vulnerability, string note = "", int flags = 131073, byte r = 0, byte g = 155, int b = 200)
        {
            return CreateCharacter(x, y, z, angle, (int)skin, role, personality, personalityId, personalityIndex, health, felony, weapon, vulnerability, note, flags, r, g, b);
        }
        // decimal support
        public Actor CreateCharacter(float x, float y, float z, float angle, decimal skin, int role, string personality, int personalityId, int personalityIndex, float health, float felony, int weapon, float vulnerability, string note = "", int flags = 131073, byte r = 0, byte g = 155, int b = 200)
        {
            return CreateCharacter(x, y, z, angle, (int)skin, role, personality, personalityId, personalityIndex, health, felony, weapon, vulnerability, note, flags, r, g, b);
        }
        */

        public Actor CreateCharacterInVehicle(Actor vehicle, uint skin, int role, string personality, int personalityId, int personalityIndex, float health, float felony, int weapon, float vulnerability, float sx = 0, float sy = 0, float sz = 0, byte seatType = 2, string note = "", int flags = 131073, byte r = 0, byte g = 155, int b = 200)
        {
            if (vehicle.TheActor.TypeId != 3)
                throw new ScriptRuntimeException("The inputed actor is not a Vehicle actor type");
            VehicleObject veh = (VehicleObject)missionData.Objects.Objects[vehicle.TheActor.ObjectId];

            float x = veh.Position.X; float y = veh.Position.Y; float z = veh.Position.Z;
            /*
            byte[] bX = BitConverter.GetBytes((float)x);
            byte[] bY = BitConverter.GetBytes((float)y);
            byte[] bZ = BitConverter.GetBytes((float)z);
            */
            byte[] bSX = BitConverter.GetBytes((float)sx);
            byte[] bSY = BitConverter.GetBytes((float)sy);
            byte[] bSZ = BitConverter.GetBytes((float)sz);
            Actor achar = CreateCharacter(x, y, z, 1, skin, role, personality, personalityId, personalityIndex, health, felony, weapon, vulnerability, note, flags, r, g, b);
            CharacterObject ochar = (CharacterObject)missionData.Objects.Objects[achar.TheActor.ObjectId];
            ochar.CreationData = new byte[]
            {
                1,0,1,13, // speculation: the first digit of the first int4 = in car? (bool)
                bSX[0],bSX[1],bSX[2],bSX[3],   bSY[0],bSY[1],bSY[2],bSY[3],    bSZ[0],bSZ[1],bSZ[2],bSZ[3], // seat position offset, why didn't they made it automatic?
                seatType,0,0,0
            };
            ochar.UID = vehicle.TheActor.ObjectId;
            return achar;
        }
        /*
        // uint support
        public Actor CreateCharacterInVehicle(Actor vehicle, uint skin, int role, string personality, int personalityId, int personalityIndex, float health, float felony, int weapon, float vulnerability, float sx = 0, float sy = 0, float sz = 0, byte seatType = 2, string note = "", int flags = 131073, byte r = 0, byte g = 155, int b = 200)
        {
            return CreateCharacterInVehicle(vehicle, (int)skin, role, personality, personalityId, personalityIndex, health, felony, weapon, vulnerability, sx, sy, sz, seatType, note, flags, r, g, b);
        }
        // decimal support
        public Actor CreateCharacterInVehicle(Actor vehicle, decimal skin, int role, string personality, int personalityId, int personalityIndex, float health, float felony, int weapon, float vulnerability, float sx = 0, float sy = 0, float sz = 0, byte seatType = 2, string note = "", int flags = 131073, byte r = 0, byte g = 155, int b = 200)
        {
            return CreateCharacterInVehicle(vehicle, (int)skin, role, personality, personalityId, personalityIndex, health, felony, weapon, vulnerability, sx, sy, sz, seatType, note, flags, r, g, b);
        }
        */

        public Actor CreateCamera(float x, float y, float z, double pitch, double yaw, double roll, float cameraZoom = 1, float speed = 1, float blur = 0, Actor lookAt = null, Actor attachTo = null, string note = "", int flags = 0, byte r = 0, byte g = 155, int b = 200)
        {
            // camera direction generation
            double pi = (Math.PI / 180) * pitch; // convert degrees to radians
            double ya = (Math.PI / 180) * (yaw+90); // convert degrees to radians
            double ro = (Math.PI / 180) * roll; // convert degrees to radians
            Vector3 fwd = new Vector3(
                (float)-(Math.Cos(pi) * Math.Cos(ya)), // x

                (float)-Math.Sin(pi), // altitude

                (float)-(Math.Cos(pi) * Math.Sin(ya)) // z
            );
            Vector3 right = new Vector3(
                (float)Math.Cos(pi), // x

                (float)0, 

                (float)Math.Sin(ya) // z
            );
            Vector4 up = new Vector3(
                (float)0, // x

                (float)Math.Cos(roll), // altitude

                (float)Math.Sin(roll) // z
            );
            /*
            Vector4 v2 = new Vector4(
                (float)(Math.Cos(p) * Math.Cos(y)), // x

                (float)-Math.Sin(p), // altitude

                (float)(Math.Cos(p) * Math.Sin(y)), // z
                (float)Math.Sin(r)
            );
            */

            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }
            missionData.LogicData.Actors.Definitions.Add(new ActorDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 9,
                StringId = stringId,
                ObjectId = missionData.Objects.Objects.Count,
                Flags = 0x2FD7,
                Properties = new List<NodeProperty>()
                {
                        new ActorProperty(lookAt == null ? -1 : lookAt.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pLookAt")
                        },          // pLookAt
                        new ActorProperty(attachTo == null ? -1 : attachTo.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pAttachTo")
                        },          // pAttachTo
                        new FloatProperty(cameraZoom) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pCameraZoom")
                        },          // pCameraZoom
                        new FloatProperty(speed) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pSpeed")
                        },          // pSpeed
                        new FloatProperty(blur) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pBlur")
                        }          // pBlur 
                }
            });
            missionData.Objects.Objects.Add(new CameraObject()
            {
                Forward = fwd,
                Up = up,
                Right = right,

                Position = new Vector3(x, y, z)
            });
            int idx = missionData.LogicData.Actors.Definitions.Count - 1;
            return new Actor(missionData.LogicData.Actors.Definitions[idx], idx); // returns the actor
        }
        public Actor CreateCameraLookingAtPosition(float x1, float y1, float z1, float x2, float y2, float z2, float cameraZoom = 1, float speed = 1, float blur = 0, Actor attachTo = null, string note = "", int flags = 0, byte r = 0, byte g = 155, int b = 200)
        {
            Actor testVolume = TestVolume(x2, y2, z2, null, 0, 1, "Camera Target", r, g, b);
            return CreateCamera(x1, y1, z1, 0, 0, 0, cameraZoom, speed, blur, testVolume, attachTo, note, flags, r, g, b);
        }
    }
    // Mission package class for Lua for Driver: PL
    // perhaps a good time to say the f word out loud...
    [MoonSharpUserData]
    public class LuaMissionPackage : FileChunker
    {
        public LuaMissionScriptDPL InitMission = new LuaMissionScriptDPL();
        public List<LuaMissionScriptDPL> Missions = new List<LuaMissionScriptDPL>();
        public List<SpoolablePackage> MissionSpoolerPackages = new List<SpoolablePackage>();

        public SpoolablePackage InitSpooler { get; set; }

        public static SpoolerAlignment MissionsSpoolerAlignment = SpoolerAlignment.Align2048;
        public static SpoolerAlignment ExportedMissionRootSpoolerAlignment = SpoolerAlignment.Align256;

        public SpoolSystemLookup MissionsLookup = new SpoolSystemLookup()
        {
            ShortFlags = 1,
            Flags = -2147483643, // 05 00 00 80
            SpoolFlags = 1,
            Unk3 = 2,
            Spooler =  new SpoolableBuffer()
        {
            Context = (int)ChunkType.SpoolSystemLookup,
            Description = "Chunk containing missions sub IDs and chunk IDs"
        } };

        public void LoadFromFile(string filepath)
        {
            throw new NotImplementedException("Loading from file is not supported yet!");
        }

        public LuaMissionScriptDPL CreateMission(string packageName = null, short id = 5, float x = 0, float y = 0, int flags = 0x01010000, bool defineRoot = true)
        {
            LuaMissionScriptDPL mission = new LuaMissionScriptDPL();
            // EPMR
            if (defineRoot) {
                mission.Spooler = new SpoolablePackage()
                {
                    Context = (int)ChunkType.ExportedMissionRoot,
                    Alignment = ExportedMissionRootSpoolerAlignment
                };
            if (packageName != null)
                mission.Spooler.Description = packageName;
            }
            // EM__
            mission.missionData.Spooler = new SpoolablePackage()
            {
                Context = (int)ChunkType.ExportedMissionChunk,
                Description = "Exported Mission",
                Alignment = MissionsSpoolerAlignment
            };
            // EMMS
            mission.missionSummary.Spooler = new SpoolableBuffer()
            {
                Context = (int)ChunkType.MissionSummary,
                Description = "Mission Summary",
                Alignment = MissionsSpoolerAlignment
            };
            
			mission.missionSummary.Spooler.GetMemoryStream().Position = 0;
            mission.missionSummary.Spooler.SetBuffer(new byte[0x1C]);

                using (var stream = new MemoryStream(new byte[0x1C]))
                {
                    stream.Write((byte)0x01);
                    stream.Write(new byte[7]);
                    stream.Write((byte)0x01);
                    stream.Write(new byte[3]);
                    stream.Write(id);
                    stream.Write((ushort)0xFFFF);
                    stream.Write(x);
                    stream.Write(y);
                    stream.Write((int)flags);
                    mission.missionSummary.Spooler.SetBuffer(stream.ToArray());
                }

            // Another one because the game for some reason needs it
            var emms = new SpoolableBuffer()
            {
                Context = (int)ChunkType.MissionSummary,
                Description = "Mission Summary",
                Alignment = MissionsSpoolerAlignment
            };

            emms.GetMemoryStream().Position = 0;
            emms.SetBuffer(new byte[0x1C]);

            using (var stream = new MemoryStream(new byte[0x1C]))
            {
                stream.Write((byte)0x01);
                stream.Write(new byte[7]);
                stream.Write((byte)0x01);
                stream.Write(new byte[3]);
                stream.Write(id);
                stream.Write((ushort)0xFFFF);
                stream.Write(x);
                stream.Write(y);
                stream.Write((int)flags);
                emms.SetBuffer(stream.ToArray());
            }

            SpoolablePackage missionPackageSp = (SpoolablePackage)mission.Spooler;
            try
            {
                missionPackageSp.Children.Add(mission.missionSummary.Spooler);
            }
            catch (Exception e) { }
            SpoolablePackage missionDataSp = (SpoolablePackage)mission.missionData.Spooler;
            try
            {
                missionDataSp.Children.Add(emms);
            }
            catch (Exception e) { }

            mission.missionData.Objects = new ExportedMissionObjects();
            mission.missionData.Objects.Spooler = new DSCript.Spooling.SpoolableBuffer()
            {
                Context = (int)ChunkType.ExportedMissionObjects,
                Description = "Exported Mission Objects",
                Alignment = MissionsSpoolerAlignment
            };
            mission.missionData.Spooler.Children.Add(mission.missionData.Objects.Spooler);
            mission.missionData.Objects.Objects = new List<MissionObject>();

            mission.missionData.PropHandles = new DSCript.Spooling.SpoolableBuffer()
            {
                Context = (int)ChunkType.ExportedMissionPropHandles,
                Description = "Exported Mission Prop Handles",
                Alignment = MissionsSpoolerAlignment
            };
            mission.missionData.PropHandles.SetBuffer(new byte[24]);
            mission.missionData.Spooler.Children.Add(mission.missionData.PropHandles);

            mission.missionData.LogicData.Spooler = new DSCript.Spooling.SpoolablePackage()
            {
                Context = (int)ChunkType.LogicExportData,
                Description = "Logic Export Data",
                Alignment = MissionsSpoolerAlignment
            };
            mission.missionData.Spooler.Children.Add(mission.missionData.LogicData.Spooler);

            mission.missionData.LogicData.WireCollection = new WireCollectionData();
            mission.missionData.LogicData.WireCollection.WireCollections = new List<WireCollection>();
            mission.missionData.LogicData.WireCollection.Spooler = new DSCript.Spooling.SpoolableBuffer()
            {
                Context = (int)ChunkType.LogicExportWireCollections,
                Description = "Exported Wire Collections",
                Alignment = MissionsSpoolerAlignment
            };

            mission.missionData.LogicData.StringCollection.Spooler = new DSCript.Spooling.SpoolableBuffer()
            {
                Context = (int)ChunkType.LogicExportStringCollection,
                Description = "String Collection",
                Alignment = MissionsSpoolerAlignment
            };
            mission.missionData.LogicData.StringCollection.Spooler.SetBuffer(new byte[4]);

            mission.missionData.LogicData.SoundBankTable = new SoundBankTableData();
            mission.missionData.LogicData.SoundBankTable.Table = new List<int>();
            mission.missionData.LogicData.SoundBankTable.Spooler = new DSCript.Spooling.SpoolableBuffer()
            {
                Context = (int)ChunkType.LogicExportSoundBank,
                Description = "Sound Bank Table",
                Alignment = MissionsSpoolerAlignment
            };
            mission.missionData.LogicData.SoundBankTable.Spooler.SetBuffer(new byte[4]);

            // unknown logic exports; required to each mission maybe?
            var LEPS = new SpoolableBuffer()
            {
                Context = (int)ChunkType.UnknownLogicExportLEPS,
                Description = "Unknown (LEPS)",
                Alignment = MissionsSpoolerAlignment
            };
            LEPS.SetBuffer(new byte[4]);
            mission.missionData.LogicData.ScriptCounters.Spooler = new SpoolableBuffer()
            {
                Context = (int)ChunkType.LogicExportScriptCounters,
                Description = "Logic Script Counters Export",
                Alignment = MissionsSpoolerAlignment
            };
            mission.missionData.LogicData.ScriptCounters.Spooler.SetBuffer(new byte[4]);
            var LENE = new SpoolableBuffer()
            {
                Context = (int)ChunkType.UnknownLogicExportLENE,
                Description = "Unknown (LENE)",
                Alignment = MissionsSpoolerAlignment
            };
            LENE.SetBuffer(new byte[4]);
            var FING = new SpoolableBuffer()
            {
                Context = (int)ChunkType.UnknownLogicExportFING,
                Description = "Unknown (FING)",
                Alignment = MissionsSpoolerAlignment
            };
            FING.SetBuffer(new byte[0x32]);
            var LEAT = new SpoolableBuffer()
            {
                Context = (int)ChunkType.UnknownLogicExportLEAT,
                Description = "Unknown (LEAT)",
                Alignment = MissionsSpoolerAlignment
            };
            if (id!=5) 
                LEAT.SetBuffer(new byte[6]);
            else
                LEAT.SetBuffer(new byte[4]);

            mission.missionData.LogicData.Actors = new LogicDataCollection<ActorDefinition>();
            mission.missionData.LogicData.Actors.Definitions = new List<ActorDefinition>();
            mission.missionData.LogicData.Actors.Spooler = new DSCript.Spooling.SpoolablePackage()
            {
                Context = (int)ChunkType.LogicExportActorsChunk,
                Description = "Logic Export Actors Chunk",
                Alignment = MissionsSpoolerAlignment
            };
            mission.missionData.LogicData.Actors.DefinitionsTable = new DSCript.Spooling.SpoolableBuffer()
            {
                Context = (int)ChunkType.LogicExportActorDefinitions,
                Description = "Logic Actors Definitions Table",
                Alignment = MissionsSpoolerAlignment
            };
            mission.missionData.LogicData.Actors.Spooler.Children.Add(mission.missionData.LogicData.Actors.DefinitionsTable);

            mission.missionData.LogicData.Actors.PropertiesTable = new DSCript.Spooling.SpoolableBuffer()
            {
                Context = (int)ChunkType.LogicExportPropertiesTable,
                Description = "Logic Actors Properties Table",
                Alignment = MissionsSpoolerAlignment
            };
            mission.missionData.LogicData.Actors.Spooler.Children.Add(mission.missionData.LogicData.Actors.PropertiesTable);

            mission.missionData.LogicData.Nodes = new LogicDataCollection<NodeDefinition>();
            mission.missionData.LogicData.Nodes.Definitions = new List<NodeDefinition>();
            mission.missionData.LogicData.Nodes.Spooler = new DSCript.Spooling.SpoolablePackage()
            {
                Context = (int)ChunkType.LogicExportNodesChunk,
                Description = "Logic Export Nodes Chunk",
                Alignment = MissionsSpoolerAlignment
            };
            mission.missionData.LogicData.Nodes.DefinitionsTable = new DSCript.Spooling.SpoolableBuffer()
            {
                Context = (int)ChunkType.LogicExportNodeDefinitionsTable,
                Description = "Logic Nodes Definitions Table",
                Alignment = MissionsSpoolerAlignment
            };
            mission.missionData.LogicData.Nodes.Spooler.Children.Add(mission.missionData.LogicData.Nodes.DefinitionsTable);

            mission.missionData.LogicData.Nodes.PropertiesTable = new DSCript.Spooling.SpoolableBuffer()
            {
                Context = (int)ChunkType.LogicExportPropertiesTable,
                Description = "Logic Nodes Properties Table",
                Alignment = MissionsSpoolerAlignment
            };
            mission.missionData.LogicData.Nodes.Spooler.Children.Add(mission.missionData.LogicData.Nodes.PropertiesTable);


            mission.missionData.LogicData.ActorSetTable = new ActorSetTableData();
            mission.missionData.LogicData.ActorSetTable.Table = new List<ActorSet>();
            mission.missionData.LogicData.ActorSetTable.Spooler = new DSCript.Spooling.SpoolableBuffer()
            {
                Context = (int)ChunkType.LogicExportActorSetTable,
                Description = "Actors Set Table",
                Alignment = MissionsSpoolerAlignment
            };

            // add other stuff too
            mission.missionData.LogicData.Spooler.Children.Add(mission.missionData.LogicData.StringCollection.Spooler);
            mission.missionData.LogicData.Spooler.Children.Add(mission.missionData.LogicData.SoundBankTable.Spooler);

            mission.missionData.LogicData.Spooler.Children.Add(LEPS);

            mission.missionData.LogicData.Spooler.Children.Add(mission.missionData.LogicData.Actors.Spooler);
            mission.missionData.LogicData.Spooler.Children.Add(mission.missionData.LogicData.Nodes.Spooler);

            mission.missionData.LogicData.Spooler.Children.Add(mission.missionData.LogicData.ActorSetTable.Spooler);
            mission.missionData.LogicData.Spooler.Children.Add(mission.missionData.LogicData.WireCollection.Spooler);
            mission.missionData.LogicData.Spooler.Children.Add(mission.missionData.LogicData.ScriptCounters.Spooler);
            // unknown ones
            mission.missionData.LogicData.Spooler.Children.Add(LENE);
            mission.missionData.LogicData.Spooler.Children.Add(FING);
            mission.missionData.LogicData.Spooler.Children.Add(LEAT);

            // finger print stuff, probably important too...
            FING.Write((short)id);
            Random random = new Random();
            byte[] randomHash = new byte[0x30];
            random.NextBytes(randomHash);
            FING.Write(randomHash);

            if (id != 5)
            { 
                LEAT.Write((int)1);
                LEAT.Write((short)4005);
            }

            if (defineRoot)
              ((SpoolablePackage)mission.Spooler).Children.Add(mission.missionData.Spooler as SpoolablePackage);

            
            //Children.Add(mission.Spooler as SpoolablePackage);

            Missions.Add(mission);

            return mission;
        }

        public void AddMission(LuaMissionScriptDPL mission,short subMissionId,string name = null)
        {
            // SSLP
            MissionsLookup.Lookups.Add(new LookupEntry((short)subMissionId, MissionSpoolerPackages.Count+1));
            MissionSpoolerPackages.Add(mission.Spooler as SpoolablePackage);

            Children.Add(mission.Spooler);
        }

        public LuaMissionPackage() : base() {
            InitMission = CreateMission(null, 5, 0, 0, 0, false);

            InitSpooler = new SpoolablePackage()
            {
                Context = (int)ChunkType.SpoolSystemInitChunker,
                Description = "Auto-exec Mission Container",
                Alignment = MissionsSpoolerAlignment
            };
            Children.Add(InitSpooler);

            MissionsLookup.Spooler = new SpoolableBuffer()
            {
                Context = (int)ChunkType.SpoolSystemLookup,
                Alignment = MissionsSpoolerAlignment,
                Description = "Missions System Lookup"
            };
            InitSpooler.Children.Add(MissionsLookup.Spooler);

            SpoolableBuffer MRRD = new SpoolableBuffer()
            {
                Context = 0x4452524D,
                Description = "Placeholder MRRD",
                Alignment = MissionsSpoolerAlignment
            };
            MRRD.SetBuffer(new byte[368]);

            using (var mstream = new MemoryStream(new byte[368]))
            {
                mstream.Write((byte)91);
                mstream.Write(new byte[365]);
                mstream.Write((byte)1);
                mstream.Write((byte)1);
                MRRD.SetBuffer(mstream.ToArray());
            }
            InitSpooler.Children.Add(MRRD);

            //SpoolablePackage initMissionPackage = InitMission.Spooler as SpoolablePackage;
            //initMissionPackage.Children.Remove(InitMission.missionData.Spooler);

            InitSpooler.Children.Add(InitMission.missionSummary.Spooler);
            InitSpooler.Children.Add(InitMission.missionData.Spooler);
            IsLoaded = true;
        }

        protected override void OnFileSaveBegin()
        {
            int missionId = 0;
            //SpoolableResourceFactory.Save(InitMission.missionData);
            foreach (LuaMissionScriptDPL mission in Missions)
            {
                Debug.WriteLine($"(LUA) Exporting Mission {missionId}");
                mission.missionData.LogicData.WireCollection.WireCollections = mission.wireCollection;
                mission.missionData.LogicData.ActorSetTable.Table = mission.actorSetup.Table;
                mission.missionData.LogicData.ScriptCounters.Counters = mission._scriptCountersData.Counters;
                SpoolableResourceFactory.Save(mission.missionData);
                
                Debug.WriteLine($"(LUA EXPORT) {mission.missionData.LogicData.Actors.Definitions.Count} actors exported, {mission.missionData.LogicData.Nodes.Definitions.Count} logic nodes exported and {mission.missionData.LogicData.WireCollection.WireCollections.Count} wire collections exported, {mission._scriptCountersData.Counters.Count} counters exported");
                missionId++;
            }
            SpoolableResourceFactory.Save(MissionsLookup);
        }
    }
    // Now this is the Lua mission script class for Driver: PL
    [MoonSharpUserData]
    public class LuaMissionScriptDPL : LuaMissionScript
    {
        public LuaMissionScriptDPL() : base()
        {
            _scriptCountersData = new ScriptCountersData();
            _scriptCountersData.Counters = new List<int>();
        }

        public ScriptCountersData _scriptCountersData { get; set; }
        public Spooler Spooler { get; set; }

        public void CountActor(Actor actor)
        {
            if (_scriptCountersData == null)
                _scriptCountersData = new ScriptCountersData();

            _scriptCountersData.Counters.Add(actor.index);
        }

        public Node LogicStart(int flags = 0, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 1,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Flags")
                        }
                }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }
		
        public Node JobComplete(int flags = 0, string note = "", int r = 255, int g = 0, int b = 0)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 5,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Flags")
                        }
                }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }		

        public Node DensityControl(float activeTrafficDensity, float parkedTrafficDensity, Table assetDensities, float litterDensity = 0.8f, int flags = 2, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            var ad = new AssetDensities() { StringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Asset Densities"), Assets = new List<int>(), Densities = new List<float>() };
            for (int assetDensityId = 0; assetDensityId < assetDensities.Length + 1; assetDensityId++)
            {
                DynValue assetDensity = assetDensities.Get(assetDensityId);
                if (assetDensity.IsNotNil())
                {
                    if (assetDensity.Table == null)
                    {
                        throw new ScriptRuntimeException($"Bad argument #3 - The table must be a table containing tables in (asset, density) format");
                    }

                    if (assetDensity.Table.Get(1).IsNotNil() & assetDensity.Table.Get(2).IsNotNil())
                    {
                        ad.Assets.Add((int)assetDensity.Table.Get(1).Number);
                        ad.Densities.Add((float)assetDensity.Table.Get(2).Number);
                    }
                    else
                    {
                        throw new ScriptRuntimeException($"Bad argument #3 - Asset Densities table index {assetDensityId} has some invalid asset densities values (nil)");
                    }
                }
            }

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 100,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new FloatProperty(activeTrafficDensity) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Active Traffic Density")
                        },
                        new FloatProperty(parkedTrafficDensity) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Parked Traffic Density")
                        },
                        new FloatProperty(litterDensity) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Litter Density")
                        },
                        ad,
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Flags")
                        }
                }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node PedestrianDensityControl(float pedDensity, Table assetDensities, int flags = 2, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            var ad = new AssetDensities() { StringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Asset Densities"), Assets = new List<int>(), Densities = new List<float>() };
            for (int assetDensityId = 0; assetDensityId < assetDensities.Length + 1; assetDensityId++)
            {
                DynValue assetDensity = assetDensities.Get(assetDensityId);
                if (assetDensity.IsNotNil())
                {
                    if (assetDensity.Table == null)
                    {
                        throw new ScriptRuntimeException($"Bad argument #3 - The table must be a table containing tables in (asset, density) format");
                    }

                    if (assetDensity.Table.Get(1).IsNotNil() & assetDensity.Table.Get(2).IsNotNil())
                    {
                        ad.Assets.Add((int)assetDensity.Table.Get(1).Number);
                        ad.Densities.Add((float)assetDensity.Table.Get(2).Number);
                    }
                    else
                    {
                        throw new ScriptRuntimeException($"Bad argument #3 - Asset Densities table index {assetDensityId} has some invalid asset densities values (nil)");
                    }
                }
            }

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 105,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new FloatProperty(pedDensity) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Pedestrian Density")
                        },
                        ad,
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Flags")
                        }
                }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node MarkerControl(Actor actor, int activity, int displayType, int minimapDisplayType, float R = 1, float G = 0.10f, float B = 0, float A = 0, int visibility = 1, int description = -1, int flags = 0, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 186,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new ActorProperty(actor) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Actor")
                        },
                        new EnumProperty(activity) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Activity")
                        },
                        new EnumProperty(displayType) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Display Type")
                        },
                        new EnumProperty(minimapDisplayType) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Minimap Display Type")
                        },
                        new Float3Property(new Vector4(R,G,B,A)) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Minimap Icon Colour")
                        },
                        new EnumProperty(visibility) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Visibility")
                        },
                        new LocalisedStringProperty(description) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Description")
                        },
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Flags")
                        }
                }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node ShowMarkerOnActor(Actor actor, int displayType, int minimapDisplayType, int description = -1, float R = 1, float G = 0.1f, float B = 0, float A = 0, int flags = 0, string note = "", int r = 0, int g = 200, int b = 122)
        {
            return MarkerControl(actor, 1, displayType, minimapDisplayType, R, G, B, A, 1, description, flags);
        }

        public Node HideMarkerOnActor(Actor actor, int flags = 0, string note = "", int r = 0, int g = 200, int b = 122)
        {
            return MarkerControl(actor, 2, 0, 2, 0, 0, 0, 0, 0, -1, 0);
        }

        public Node Wanderer(Actor wanderer, float speed, float acceleration, float traction = 1, float handOfTom = 1, int flags = 0, string note = "", int r = 255, int g = 255, int b = 0)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 166,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new ActorProperty(wanderer) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Wanderer")
                        },
                        new FloatProperty(speed) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Speed")
                        },
                        new FloatProperty(acceleration) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Acceleration")
                        },
                        new FloatProperty(traction) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Traction")
                        },
                        new FloatProperty(traction) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Hand of Tom")
                        },
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Flags")
                        }
                }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node StartMission(int subMissionID, int status = 1, int flags = 0, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 148,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new IntegerProperty(subMissionID) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Sub Mission ID")
                        },
                        new EnumProperty(status) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Status")
                        },

                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Flags")
                        }
                }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node MissionWatch(int subMissionID, int condition = 1, int flags = 0, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 147,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new IntegerProperty(subMissionID) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Sub Mission ID")
                        },
                        new EnumProperty(condition) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Condition")
                        },

                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Flags")
                        }
                }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node GarageControl(int activity, int garage, int vehicleType = -1, int veditFlags = 16777216, int flags = 0, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 107,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new EnumProperty(activity) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Activity")
                        },
                        new EnumProperty(garage) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Garage")
                        },
                        new EnumProperty(vehicleType) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Vehicle Type")
                        },
                        new FlagsProperty(veditFlags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("V-Edit Flags")
                        },
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Flags")
                        }
                }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node GiveActorCreation(Actor actor, int activity = 1, int flags = 1, string note = "", int r = 0, int g = 200, int b = 122)
        {
            if (actor == null)
                throw new ScriptRuntimeException("Bad argument #1 - Attempt to give creation to a nil value");
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 101,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new FloatProperty(actor.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Interval")
                        },
                        new EnumProperty(activity) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Activity")
                        },
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Flags")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }
		
		public Node MusicTrackControl(int track, int activity = 3, int flags = 1, string note = "", int r = 0, int g = 200, int b = 122)
        {
            if (track == null)
                throw new ScriptRuntimeException("Bad argument #1 - Attempt to control a nil value track!");
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 216,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new EnumProperty(activity) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Activity")
                        },
						new IntegerProperty(track) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Track")
                        },
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Flags")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node CreationGroup(int actorSetID, int activity = 1, int flags = 0, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 37,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new IntegerProperty(actorSetID) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Actor Set ID")
                        },
                        new EnumProperty(activity) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Activity")
                        },

                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Flags")
                        }
                }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node TimeControl(int activity, int hours, int minutes, int flags = 1,string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 113,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
						new EnumProperty(activity) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Activity")
                        },
						new IntegerProperty(hours) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Hours")
                        },
                        new IntegerProperty(minutes) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Minutes")
                        },
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Flags")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        // LUA functions: MISSION.functionName(args)
        public Node SetTimer(float interval, int flags = 1,string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 3,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new FloatProperty(interval) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Interval")
                        },
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Flags")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node FadeControl(int direction,float duration,int flags = 0, byte R = 0, byte G = 0, byte B = 0, byte A = 0, string note = "", int r = 0, int g = 200, int b = 122)
        {
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 112,
                StringId = stringId,
                Flags = 0x1862,
                Reserved = 0,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new EnumProperty(direction) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Direction")
                        },
                        new FloatProperty(duration) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Duration")
                        },
                        new Float3Property(new Vector4(R,G,B,A)) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Colour")
                        },
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Flags")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }

        public Node SetFadeOut(float duration = 0.5f, int flags = 0, byte R = 0, byte G = 0, byte B = 0, byte A = 0, string note = "FadeOut()", int r = 0, int g = 200, int b = 122)
        {
            return this.FadeControl(2, duration, flags, R, G, B, A, note, r, g, b);
        }

        public Node SetFadeIn(float duration = 0.5f, int flags = 0, byte R = 0, byte G = 0, byte B = 0, byte A = 0, string note = "FadeIn()", int r = 0, int g = 200, int b = 122)
        {
            return this.FadeControl(1, duration, flags, R, G, B, A, note, r, g, b);
        }
		
        public Node ProximityCheck(Actor actor1, Actor actor2, float threshold, int checkType, int flags = 5, string note = "", int r = 0, int g = 200, int b = 122)
        {
            if (actor1 == null)
                throw new ScriptRuntimeException("Bad Argument #1 - Actor expected got nil");
            if (actor2 == null)
                throw new ScriptRuntimeException("Bad Argument #2 - Actor expected got nil");
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }

            int pWireCollection = wireCollection.Count;
            CollectionOfWires cow = new CollectionOfWires(0, pWireCollection);
            wireCollection.Add(cow);

            missionData.LogicData.Nodes.Definitions.Add(new NodeDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 30,
                StringId = stringId,
                Properties = new List<NodeProperty>
                    {
                        new WireCollectionProperty(pWireCollection) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pWireCollection")
                        },
                        new ActorProperty(actor1.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("First Actor")
                        },
                        new ActorProperty(actor2.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Second Actor")
                        },
                        new EnumProperty(checkType) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Check Type")
                        },						
                        new FloatProperty(threshold) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Threshold")
                        },
                        new FlagsProperty(flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }
                    }
            });
            int idx = missionData.LogicData.Nodes.Definitions.Count - 1;
            return new Node(missionData.LogicData.Nodes.Definitions[idx], idx) { WireCollection = cow };
        }		

        public Actor CreateMissionActor(float x, float y, float z, int subMissionID, int eventType, int potNumber = 0, int iconType = 12, string tempMissionName = "", string tempMissionDescription = "", string fileName = "", string FMVfileName = "", int flags = 1, string note = "", byte r = 0, byte g = 155, int b = 200)
        {
            var a = 0; // convert degrees to radians (yaw for forward)
            var a2 = 0; // convert degrees to radians (yaw - 90 deg. for left)
            var a3 = 0; // convert degrees to radians (pitch for up)
            // forward
            Vector4 fwd = new Vector4(
                (float)(Math.Cos(0) * Math.Cos(a)), // x

                0, //(float)-Math.Sin(angle), // altitude

                (float)(Math.Cos(0) * Math.Sin(a)), 0 // z
            );
            // left
            Vector4 l = new Vector4(
                (float)(Math.Cos(0) * Math.Cos(a2)), // x

                0, //(float)-Math.Sin(angle), // altitude

                (float)(Math.Cos(0) * Math.Sin(a2)), 0 // z
            );
            // up
            Vector4 up = new Vector4(
                (float)(Math.Cos(a3) * Math.Cos(a)), // x

                (float)-Math.Sin(a3), // altitude

                (float)(Math.Cos(a3) * Math.Sin(a)), 0 // z
            );
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }
            missionData.LogicData.Actors.Definitions.Add(new ActorDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 106,
                StringId = stringId,
                ObjectId = -1,
                Flags = 0x14B9,
                Reserved = 0,
                Properties = new List<NodeProperty>()
                {
                        new MatrixProperty(new Vector4(x,y,z,1),l,up,fwd) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Matrix")
                        },
                        new EnumProperty(subMissionID) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Sub Mission ID")
                        },
                        new StringProperty((short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(tempMissionName)) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Temp Mission Name")
                        },
                        new StringProperty((short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(tempMissionDescription)) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Temp Mission Description")
                        },
                        new StringProperty((short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(fileName)) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("FileName")
                        },
                        new EnumProperty(eventType) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Event Type")
                        },
                        new IntegerProperty(potNumber) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Pot Number")
                        },
                        new StringProperty((short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(FMVfileName)) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("FMV FileName")
                        },
                        new EnumProperty(iconType) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Icon Type")
                        },
                        new FlagsProperty((int)flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Flags")
                        }           // pFlags
                }
            });

            int idx = missionData.LogicData.Actors.Definitions.Count - 1;
            return new Actor(missionData.LogicData.Actors.Definitions[idx], idx); // returns the actor
        }

        // NOTE: use flags 65537 to make this the player
        public Actor CreateCharacter(float x, float y, float z, float angle, int skin, float health, float felony, Actor vehicle=null, int vehicleSeat=0, int weapon = 0, bool player = false, int flags = 1, string note = "", byte r = 0, byte g = 155, int b = 200)
        {
            // auto
            if (player)
                flags = 65537;
            var rad = (float)(Math.PI / 180);
            var a = rad  * angle; // convert degrees to radians (yaw for forward)
            var a2 = rad * (angle+90); // convert degrees to radians (yaw - 90 deg. for left)
            var a3 = rad * -90; // convert degrees to radians (pitch for up)
            // forward
            Vector4 fwd = new Vector4(
                (float)(Math.Cos(0) * Math.Cos(a)), // x

                0, //(float)-Math.Sin(angle), // altitude

                (float)(Math.Cos(0) * Math.Sin(a)), 0 // z
            );
            // left
            Vector4 l = new Vector4(
                (float)(Math.Cos(0) * Math.Cos(a2)), // x

                0, //(float)-Math.Sin(angle), // altitude

                (float)(Math.Cos(0) * Math.Sin(a2)), 0 // z
            );
            // up
            Vector4 up = new Vector4(
                (float)(Math.Cos(a3) * Math.Cos(a)), // x

                (float)-Math.Sin(a3), // altitude

                (float)(Math.Cos(a3) * Math.Sin(a)), 0 // z
            );
            // automatic: if it's player then set start position to this
            if (player)
                missionSummary.StartPosition = new Vector2(x, y);
            short stringId = 0;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }
            missionData.LogicData.Actors.Definitions.Add(new ActorDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 2,
                StringId = stringId,
                ObjectId = -1,
                Flags = 0x14B9,
                Reserved = 2974,
                Properties = new List<NodeProperty>()
                {
                        new MatrixProperty(new Vector4(x,y,z,1),l,up,fwd) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Matrix")
                        },         
                        new ObjectTypeProperty(skin) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Character Skin Type")
                        },        
                        new FloatProperty(health) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Initial Health")
                        },          
                        new FloatProperty(felony) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Initial Felony")
                        },
                        new EnumProperty(weapon) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Weapon")
                        },
                        new ActorProperty(vehicle == null ? -1 : vehicle.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Initial Vehicle")
                        },
                        new EnumProperty(vehicleSeat) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Initial Vehicle Seat")
                        },
                        new FlagsProperty((int)flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Flags")
                        }           // pFlags
                }
            });

            int idx = missionData.LogicData.Actors.Definitions.Count - 1;
            return new Actor(missionData.LogicData.Actors.Definitions[idx], idx); // returns the actor
        }

        public Actor CreateVehicle(float x, float y, float z, float angle, int vehicleId, int colorId, float initialSpeed = 0, float initialFelony = 0, float bulletSoftness = 0.3333f, float explosionSoftness = 0.5f, float impactSoftness = 0.6667f, float impactFragility = 0.6667f, Actor attachedVehicle = null, string note = "", int flags = 302186497, float damage = 0, float softness = 1, float weight = 1f, float fragility = 1, int r = 0, int g = 200, int b = 122)
        {
            var rad = (float)(Math.PI / 180);
            var a = rad * angle; // convert degrees to radians (yaw for forward)
            var a2 = rad * (angle + 90); // convert degrees to radians (yaw - 90 deg. for left)
            var a3 = rad * -90; // convert degrees to radians (pitch for up)
            // forward
            Vector4 fwd = new Vector4(
                (float)(Math.Cos(0) * Math.Cos(a)), // x

                0, //(float)-Math.Sin(angle), // altitude

                (float)(Math.Cos(0) * Math.Sin(a)), 0 // z
            );
            // left
            Vector4 l = new Vector4(
                (float)(Math.Cos(0) * Math.Cos(a2)), // x

                0, //(float)-Math.Sin(angle), // altitude

                (float)(Math.Cos(0) * Math.Sin(a2)), 0 // z
            );
            // up
            Vector4 up = new Vector4(
                (float)(Math.Cos(a3) * Math.Cos(a)), // x

                (float)-Math.Sin(a3), // altitude

                (float)(Math.Cos(a3) * Math.Sin(a)), 0 // z
            );
            short stringId;
            if (note == "" | note == null) { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown"); }
            else { stringId = (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew(note); }
            missionData.LogicData.Actors.Definitions.Add(new ActorDefinition()
            {
                Color = new NodeColor(r, g, b, 255),
                TypeId = 3,
                StringId = stringId,
                ObjectId = -1,
                Properties = new List<NodeProperty>
                    {
                        new MatrixProperty(new Vector4(x,y,z,1),l,up,fwd) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Matrix")
                        },         // pWeight
                        new ObjectTypeProperty(vehicleId) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Vehicle Type")
                        },         // pSoftness
                        new FloatProperty(initialSpeed) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Initial Speed")
                        },         // pIFragility
                        new FloatProperty(initialFelony) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Initial Felony")
                        },         // pIFragility
                        new FloatProperty(impactSoftness) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Impact Softness")
                        },         // pDemoOnlySoftness
                        new FloatProperty(explosionSoftness) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Explosion Softness")
                        },         // pDemoOnlySoftness
                        new FloatProperty(explosionSoftness) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Bullet Softness")
                        },         // pDemoOnlySoftness
                        new FloatProperty(impactFragility) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Impact Fragility")
                        },         // pDemoOnlySoftness
                        new VehicleTintProperty((int)colorId) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Vehicle Tint")
                        },          // pTintValue
                        new IntegerProperty((int)colorId) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Tint Value")
                        },          // pTintValue
                        new ActorProperty(attachedVehicle == null ? -1 : attachedVehicle.index) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("Attached Vehicle")
                        },          // pTintValue
                        new FlagsProperty((int)flags) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }     // pFlags 
                    }
            });
            
            int idx = missionData.LogicData.Actors.Definitions.Count - 1;
            return new Actor(missionData.LogicData.Actors.Definitions[idx], idx); // returns the actor
        }
    }
}