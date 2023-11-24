using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Diagnostics;

using DSCript;
using DSCript.Spooling;

namespace Zartex
{
    public class MissionInstance
    {
        public static int DataBufferSize = 32;

        [Category("Instance"), Description("Position of this prop")]
        [PropertyOrder(10)]
        public Vector4 Position { get; set; }
        [Category("Instance"), Description("The ID to the world instance this instance is using.")]
        [PropertyOrder(20)]
        public short InstanceId { get; set; }
        [Category("Misc"), Description("The ID to the instance this instance is attached to.")]
        [PropertyOrder(30)]
        public short AttachedTo { get; set; }
        [Category("Misc"), Description("XYZ coordinates representing a bounding box.")]
        [PropertyOrder(40)]
        public Vector4 BoundingBox { get; set; }

        public MissionInstance() { }
        public MissionInstance(Vector4 pos,short instanceId,short attachedTo,Vector4 boundingBox)
        {
            Position = pos; InstanceId = instanceId; AttachedTo = attachedTo; BoundingBox = boundingBox;
        } 
    }
    public class MissionInstanceData : SpoolableResource<SpoolableBuffer>
    {
        public static int HeaderBufferSize = 64;

        public int Unk1 { get; set; }
        public int Unk2 { get; set; }
        public int Unk3 { get; set; }
        public int Unk4 { get; set; }
        public int Unk5 { get; set; }

        public int InstanceCount { get; set; }
        public int Unk6 { get; set; }
        public int Unk7 { get; set; }

        public Vector3 LoadPosition { get; set; }
        public Vector4 StartPosition { get; set; }

        public List<MissionInstance> Instances { get; set; }

        public MissionInstance this[int index]
        {
            get { return Instances[index]; }
            set { Instances[index] = value; }
        }

        protected override void Load()
        {
            Debug.WriteLine("Load from prop handles called");
            using (var f = Spooler.GetMemoryStream())
            {
                Debug.WriteLine("Loading");
                Debug.WriteLine($"Stream position: {f.Position}, Size {f.Length}");
                int count = f.ReadInt32();
                Unk1 = f.ReadInt32();
                Unk2 = f.ReadInt32();
                Unk3 = f.ReadInt32();
                Unk4 = f.ReadInt32();
                Unk5 = f.ReadInt32();
                InstanceCount = f.ReadInt32();
                Unk6 = f.ReadInt32();
                Unk7 = f.ReadInt32();

                LoadPosition = f.Read<Vector3>();
                StartPosition = f.Read<Vector4>();

                Instances = new List<MissionInstance>(count);
                Debug.WriteLine($"Count of PHs -> {count}");
                for (int id = 0; id < count; id++)
                {
                    Debug.WriteLine($"Processing PH ID: {id}");
                    Instances[id] = new MissionInstance(f.Read<Vector4>(), f.ReadInt16(), f.ReadInt16(), f.Read<Vector4>());
                }
            }
        }
        protected override void Save()
        {
            var bufferSize = HeaderBufferSize+(MissionInstance.DataBufferSize* Instances.Count);

            var propBuffer = new byte[bufferSize];

            using (var f = new MemoryStream(propBuffer))
            {
                int count = f.ReadInt32();
                f.Write(Unk1);
                f.Write(Unk2);
                f.Write(Unk3);
                f.Write(Unk4);
                f.Write(Unk5);
                f.Write(InstanceCount);
                f.Write(Unk6);
                f.Write(Unk7);

                f.Write<Vector3>(LoadPosition);
                f.Write<Vector4>(StartPosition);

                foreach (MissionInstance inst in Instances)
                {
                    f.Write<Vector4>(inst.Position); f.Write(inst.InstanceId); f.Write(inst.AttachedTo); f.Write<Vector4>(inst.BoundingBox);
                }
            }

            Spooler.SetBuffer(propBuffer);
        }
    }
}
