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
        public Vector3 Position { get; set; }
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
        public MissionInstance(Vector3 pos,short instanceId,short attachedTo,Vector4 boundingBox)
        {
            Position = pos; InstanceId = instanceId; AttachedTo = attachedTo; BoundingBox = boundingBox;
        } 
    }
    public class MissionInstanceData : SpoolableResource<SpoolableBuffer>
    {
        public static int HeaderBufferSize = 64;

        // custom spoolable buffer
        public SpoolableBuffer Spooler {
            get { return base.Spooler; }
            set { base.Spooler = value; }
        }

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
            using (var f = Spooler.GetMemoryStream())
            {
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

                Instances = new List<MissionInstance>();
                for (int id = 0; id < count; id++)
                {
                    Instances.Add( new MissionInstance(f.Read<Vector3>(), f.ReadInt16(), f.ReadInt16(), f.Read<Vector4>()) );
                }
            }
        }
        protected override void Save()
        {
            var bufferSize = HeaderBufferSize+(MissionInstance.DataBufferSize* Instances.Count);

            var propBuffer = new byte[bufferSize];

            using (var f = new MemoryStream(propBuffer))
            {
                int count = Instances.Count;
                f.Write(count);
                f.Write(Unk1);
                f.Write(Unk2);
                f.Write(Unk3);
                f.Write(Unk4);
                f.Write(Unk5);
                f.Write(count); // InstanceCount
                f.Write(count);
                f.Write(count);

                f.Write<Vector3>(LoadPosition);
                f.Write<Vector4>(StartPosition);

                foreach (MissionInstance inst in Instances)
                {
                    f.Write<Vector3>(inst.Position); f.Write(inst.InstanceId); f.Write(inst.AttachedTo); f.Write<Vector4>(inst.BoundingBox);
                }
            }

            Spooler.SetBuffer(propBuffer);
        }
    }
}
