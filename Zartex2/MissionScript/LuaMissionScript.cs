using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DSCript;

using MoonSharp;
using MoonSharp.Interpreter;

namespace Zartex
{
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
        public void SetPropertyValue(int id, float value)
        {
            TheActor.Properties[id].Value = (float)value;
        }
        public void SetPropertyValue(int id, int value)
        {
            TheActor.Properties[id].Value = (int)value;
        }
        public void SetPropertyValue(int id, float x, float y, float z)
        {
            TheActor.Properties[id].Value = new Vector3(x, y, z);
        }
        public void SetPropertyValue(int id, float x, float y, float z, float w)
        {
            TheActor.Properties[id].Value = new Vector4(x, y, z, w);
        }
        public void SetPropertyValue(int id, short index, short value)
        {
            var prop = (TextFileItemProperty)TheActor.Properties[id];
            prop.Index = index;
            prop.Value = value;
        }
        public void SetPropertyValue(int id, Actor actor)
        {
            TheActor.Properties[id].Value = (int)actor.index;
        }
    }
    [MoonSharpUserData]
    public class Node
    {
        public NodeDefinition TheNode = new ActorDefinition();
        public int index; // extremely important
        public CollectionOfWires WireCollection { get; set; }

        public Node(NodeDefinition node, int idx)
        {
            index = idx;
            TheNode = node;
        }
        public void SetPropertyValue(int id, float value)
        {
            TheNode.Properties[id].Value = (float)value;
        }
        public void SetPropertyValue(int id, int value)
        {
            TheNode.Properties[id].Value = (int)value;
        }
        public void SetPropertyValue(int id, float x, float y, float z)
        {
            TheNode.Properties[id].Value = new Vector3(x, y, z);
        }
        public void SetPropertyValue(int id, float x, float y, float z, float w)
        {
            TheNode.Properties[id].Value = new Vector4(x, y, z, w);
        }
        public void SetPropertyValue(int id, short index, short value)
        {
            var prop = (TextFileItemProperty)TheNode.Properties[id];
            prop.Index = index;
            prop.Value = value;
        }
        public void SetPropertyValue(int id, Actor actor)
        {
            TheNode.Properties[id].Value = (int)actor.index;
        }
    }
    [MoonSharpUserData]
    public class CollectionOfWires : WireCollection
    {
        //public List<WireNode> Wires = new List<WireNode>();
        public int index; // extremely important

        public void Add(Node node, byte type = 1)
        {
            Wires.Add(new WireNode()
            {
                NodeId = (short)node.index,
                OpCode = node.TheNode.TypeId,
                WireType = (byte)type
            });
        }
        public CollectionOfWires(int nWires) : base(nWires)
        {
        }
        public CollectionOfWires(int nWires, int idx) : base(nWires)
        {
            index = idx;
        }
    }
    [MoonSharpUserData]
    public class MissionSummary : MissionSummaryData
    {
        public short MoodId { get { return this.MissionId; } set { this.MissionId = value; } }
        public short LocaleId { get { return this.MissionLocaleId; } set { this.MissionLocaleId = value; } }

        public float X { get { return this.StartPosition.X; } set { this.StartPosition = new Vector2(value, this.StartPosition.Y); } }
        public float Y { get { return this.StartPosition.Y; } set { this.StartPosition = new Vector2(this.StartPosition.X, value); } }

        public string GetCityNameByType(MissionCityType cityType)
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

        public MissionCityType GetCityTypeByName(string cityType)
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
        public ExportedMission missionData = new ExportedMission();
        public MissionSummary missionSummary = new MissionSummary();

        public List<WireCollection> wireCollection = new List<WireCollection>();
        //public Table wireCollection;



        public LuaMissionScript()
        {
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

        // Lua starts from 1 and C# starts indexing from 0
        public Actor GetActorById(int id)
        {
            if ((id - 1) > missionData.LogicData.Actors.Definitions.Count - 1)
                throw new ScriptRuntimeException("Bad argument #1 - Index ID can't be bigger than the size of the array");
            if ((id - 1) < 0)
                throw new ScriptRuntimeException("Bad argument #1 - Index ID can't be negative");
            return new Actor(missionData.LogicData.Actors.Definitions[(id - 1)], (id - 1));
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

        public Node SetCharacterFelonyTo(Actor character, float felony, int flags = 0, string note = "", int r = 200, int g = 255, int b = 100)
        {
            return CharacterControl(character, 6, felony, 0, -1, null, null, "", -1, "", 0, flags, note, r, g, b);
        }

        public Node SetCharacterStuckInVehicle(Actor character, int flags = 0, string note = "SetCharacterStuckInVehicle()", int r = 200, int g = 255, int b = 100)
        {
            return CharacterControl(character, 12, 1, 0, -1, null, null, "", -1, "", 0, flags, note, r, g, b);
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

        public Actor GetAnimatedObjectAt(float x, float y, float z, int type = 0, string note = "", int r = 200, int g = 128, int b = 128)
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
                        new EnumProperty(type) {
                            StringId =  (short)missionData.LogicData.StringCollection.findStringIdByValueOrCreateNew("pType")
                        }           // pType
                    }
            });
            missionData.Objects.Objects.Add(new AnimPropObject() { Position = new Vector3((float)x, (float)y, (float)z) });

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

            // ai target
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

        public Node CharacterDriveTo(Actor character, float x, float y, float z, float driveSpeed, int desiredTransport = 2, int flags = 0, string note = "CharacterDriveTo", int r = 128, int g = 0, int b = 255)
        {
            if (character.TheActor.TypeId != 2)
                throw new ScriptRuntimeException("Inputed actor is not a Character type actor");
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
            missionData.Objects.Objects.Add(new VolumeObject() { Position = new Vector3((float)x, (float)y, (float)z) });


            int idx = missionData.LogicData.Actors.Definitions.Count - 1;
            return ActorCreation(new Actor(missionData.LogicData.Actors.Definitions[idx], idx));
        }

        public Node CharacterDriveFollowingActor(Actor character, Actor actor, float driveSpeed, int desiredTransport = 2, int flags = 0, string note = "CharacterDriveFollowingActor", int r = 128, int g = 0, int b = 255)
        {
            Node driveTo = CharacterDriveTo(character, 0, 0, 0, driveSpeed, desiredTransport, flags, note, r, g, b);
            var idx = driveTo.TheNode.Properties[1].Value;
            Actor aiTarget = new Actor(missionData.LogicData.Actors.Definitions[(int)idx], (int)idx);
            aiTarget.TheActor.Properties[1].Value = actor.index;

            return driveTo;
        }

        public Node CameraSelect(Actor target, int cameraType, float duration = 0, float blendTime = 1, float thrillCamZoom = 1, float thrillCamSpeed = 1, float thrillCamBlur = 0, int flags = 0, string note = "CameraSelect()", int r = 128, int g = 0, int b = 255)
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

        public Node StartClock(int flags = 0, string note = "", int r = 255, int g = 255, int b = 0)
        {
            return CountdownIntro(0, 4, flags, note, r, g, b);
        }

        public Node HasClockRunnedOut(string note = "", int r = 255, int g = 255, int b = 0)
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
            Actor objectiveIcon = CreateObjectiveIcon(0, 0, 0, height, maxAdaptiveHeight, type, canGoUp, note, r, g, b);
            objectiveIcon.TheActor.Properties[0].Value = (int)actor.index;
            return objectiveIcon;
        }

        public Actor CreateVehicle(float x, float y, float z, float angle, int vehicleId, int colorId, string note = "", int flags = 302186497, float softness = 1, float weight = 1f, float fragility = 1, int r = 0, int g = 200, int b = 122)
        {
            var a = (Math.PI / 180) * angle; // convert degrees to radians
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
                        bX[0],bX[1],bX[2],bX[3],      bY[0],bY[1],bY[2],bY[3],     bZ[0],bZ[1],bZ[2],bZ[3],
                        // direction...
                        bFX[0],bFX[1],bFX[2],bFX[3],    bFY[0],bFY[1],bFY[2],bFY[3],     bFZ[0],bFZ[1],bFZ[2],bFZ[3],
                        // more unknown floats stuff, guess what? ZEROS!
                        0,0,0,0, 0,0,0x80,0x3F,
                        // unknown byte gang
                        5,0x20,0,0,
                        // more zeros.... and 1.0f again.
                        0,0,0,0,    0,0,0,0,     0,0,0,0,   0,0,0x80,0x3F,
                        // AND..... the end!
                        0,0,0x80,0x3F,  0,0,0,0,    0,0,0,0
                },
                Position = new DSCript.Vector4((float)x, (float)y, (float)z, angle)
            });
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

        public Actor CreateCharacter(float x, float y, float z, float angle, int role, string personality, int personalityId, int personalityIndex, float health, float felony, int weapon, float vulnerability, string note = "", int flags = 131073, byte r = 0, byte g = 155, int b = 200)
        {
            var a = (Math.PI / 180) * angle; // convert degrees to radians
            Vector3 fwd = new Vector3(
                (float)(Math.Cos(0) * Math.Cos(a)), // x

                0, //(float)-Math.Sin(angle), // altitude

                (float)(Math.Cos(0) * Math.Sin(a)) // z
            );
            // automatic: if role == 1 (player) then set start position to this character position
            if (role == 1)
                missionSummary.StartPosition = new Vector2(x, y);
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
        public Actor CreateCharacterInVehicle(Actor vehicle, int role, string personality, int personalityId, int personalityIndex, float health, float felony, int weapon, float vulnerability, float sx = 0, float sy = 0, float sz = 0, byte seatType = 2, string note = "", int flags = 131073, byte r = 0, byte g = 155, int b = 200)
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
            Actor achar = CreateCharacter(x, y, z, 1, role, personality, personalityId, personalityIndex, health, felony, weapon, vulnerability, note, flags, r, g, b);
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
        public Actor CreateCamera(float x, float y, float z, double pitch, double yaw, double roll, float cameraZoom = 1, float speed = 1, float blur = 0, Actor lookAt = null, Actor attachTo = null, string note = "", int flags = 0, byte r = 0, byte g = 155, int b = 200)
        {
            // camera direction generation
            Vector4 v2 = new Vector4(
                (float)(Math.Cos(pitch) * Math.Cos(yaw)), // x

                (float)-Math.Sin(pitch), // altitude

                (float)(Math.Cos(pitch) * Math.Sin(yaw)), // z
                (float)Math.Sin(roll)
            );
            Vector4 v1 = new Vector4(
                (float)-(Math.Cos(pitch) * Math.Cos(yaw)), // x

                (float)Math.Sin(pitch), // altitude

                (float)-(Math.Cos(pitch) * Math.Sin(yaw)), // z
                (float)-Math.Sin(roll)
            );
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
                V1 = v1,
                V2 = v2,
                V3 = new Vector4(-1, x, y, z)
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
    // Now this is the Lua mission script class for Driver: PL
    [MoonSharpUserData]
    public class LuaMissionScriptDPL
    {
        public ExportedMission missionData = new ExportedMission();
        public MissionSummary missionSummary = new MissionSummary();

        public List<WireCollection> wireCollection = new List<WireCollection>();
        //public Table wireCollection;



        public LuaMissionScriptDPL()
        {
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

        // Lua starts from 1 and C# starts indexing from 0
        public Actor GetActorById(int id)
        {
            if ((id - 1) > missionData.LogicData.Actors.Definitions.Count - 1)
                throw new ScriptRuntimeException("Bad argument #1 - Index ID can't be bigger than the size of the array");
            if ((id - 1) < 0)
                throw new ScriptRuntimeException("Bad argument #1 - Index ID can't be negative");
            return new Actor(missionData.LogicData.Actors.Definitions[(id - 1)], (id - 1));
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

        // NOTE: use flags 65537 to make this the player
        public Actor CreateCharacter(float x, float y, float z, float angle, int skin, float health, float felony, Actor vehicle=null, int vehicleSeat=0, int weapon = 0, bool player = false, int flags = 1, string note = "", byte r = 0, byte g = 155, int b = 200)
        {
            // auto
            if (player)
                flags = 65537;
            var rad = (float)(Math.PI / 180);
            var a = rad  * angle; // convert degrees to radians
            var a2 = rad * (angle-90); // convert degrees to radians
            var a3 = rad * -90; // convert degrees to radians
            // forward
            Vector3 fwd = new Vector3(
                (float)(Math.Cos(0) * Math.Cos(a)), // x

                0, //(float)-Math.Sin(angle), // altitude

                (float)(Math.Cos(0) * Math.Sin(a)) // z
            );
            // left
            Vector3 l = new Vector3(
                (float)(Math.Cos(0) * Math.Cos(a2)), // x

                0, //(float)-Math.Sin(angle), // altitude

                (float)(Math.Cos(0) * Math.Sin(a2)) // z
            );
            // up
            Vector3 up = new Vector3(
                (float)(Math.Cos(a3) * Math.Cos(a)), // x

                (float)-Math.Sin(a3), // altitude

                (float)(Math.Cos(a3) * Math.Sin(a)) // z
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
                Flags = 0x2FD7,
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
    }
}