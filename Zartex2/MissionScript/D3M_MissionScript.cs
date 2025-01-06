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
    public enum D3M_VariableType : int
    {
        Null = -1,

        Number = 0,
        Text = 1,
        Object = 2
    }
    public enum D3M_OpcodeType : int
    {
        Nop = 0,
        DebugText = 1,
        MissionComplete = 2,
        MissionFailed = 3
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
    public class D3M_Vehicle : D3M_Object
    {
        public uint VehicleType;

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
            var a = (Math.PI / 180) * (RY); // convert degrees to radians
            // convert player/character angle to vehicle angle
            a = (a / 2) + 45; // TODO: subtract this with 26
            a -= 25.5f;
            Vector3 fwd = new Vector3(
                (float)(Math.Cos(0) * Math.Cos(a)), // x

                0, //(float)-Math.Sin(angle), // altitude

                (float)(Math.Cos(0) * Math.Sin(a)) // z
            );
            int flags = 302186497 - (StartUninitialised ? 1 : 0); // TODO: extend this feature
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
                        0,0,0,0,    0,0,0,0,     0,0,0,0,   0,0,0x0,0x0,
                        // AND..... the end!
                        0,0,0x0,0x0,  bDMG[0],bDMG[1],bDMG[2],bDMG[3],    0,0,0,0
                },
                Position = new DSCript.Vector4((float)X, (float)Y, (float)Z, RY)
            });

            int idx = exportedMission.LogicData.Actors.Definitions.Count - 1;
            Definition = exportedMission.LogicData.Actors.Definitions[idx];
            MissionObject = exportedMission.Objects.Objects[Definition.ObjectId];
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
    }
    public class D3M_MissionScript
    {
        protected const byte r = 14;
        protected const byte g = 201;
        protected const byte b = 255;

        public List<D3M_Variable> GlobalVariables;
        public List<D3M_Object> Objects;

        public ExportedMission ExportedMission;
        public NodeDefinition LogicStart;

        /// <summary>
        /// Creates a logic node inside a exported mission
        /// </summary>
        /// <param name="exportedMission">The exported mission to add the node definition to</param>
        /// <param name="Type">The node type</param>
        /// <param name="nodeProperties">The node's properties</param>
        /// <returns>The created node definition</returns>
        NodeDefinition CreateLogicNode(ExportedMission exportedMission, byte Type, List<NodeProperty> nodeProperties)
        {
            short stringId = (short)exportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("Unknown");

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

        /// <summary>
        /// Processes a opcode 
        /// </summary>
        /// <param name="type">Integer that is the type of the opcode to be processed</param>
        /// <param name="sourceLine">Source line from where the opcode is being processed</param>
        /// <param name="LabelName">Source label name from where the opcode is being prcoessed</param>
        /// <param name="parameters">Input parameters</param>
        void ProcessOpcode(int type, int sourceLine, string LabelName, List<D3M_Variable> parameters)
        {
            if (LogicStart == null)
                LogicStart = CreateLogicNode(ExportedMission, 1, new List<NodeProperty> { });

            NodeDefinition NodeToAddTo = LogicStart;
            byte enable_WireType = 1;
            byte disable_WireType = 2;
            if (LabelName != "MAIN")
            {
                // switch the wire type on disable & enable for group disable & group enable
                enable_WireType += 10;
                disable_WireType += 10;
                NodeToAddTo = CreateLogicNode(ExportedMission, 8, new List<NodeProperty> { });
            }

            switch (type)
            {
                case 1:
                    var node = CreateLogicNode(ExportedMission, 2, new List<NodeProperty>
                    {
                        new StringProperty((short)parameters[0].Number) {
                            StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pMessage")
                        },         // pInterval
                        new FlagsProperty(0) {
                            StringId =  (short)ExportedMission.LogicData.StringCollection.findStringIdByValueOrCreateNew("pFlags")
                        }          // pFlags
                    });
                    GetNodeWireCollection(NodeToAddTo).Wires.Add(new WireNode() { WireType = enable_WireType, OpCode = node.TypeId, NodeId = (short)FindNodeId(node) });
                    break;
                // opcode caves
                case 0:
                    break;
                default:
                    throw new InvalidOperationException($"Unknown opcode of the type {type}! at line {sourceLine}");
            }
        }
    }
}
