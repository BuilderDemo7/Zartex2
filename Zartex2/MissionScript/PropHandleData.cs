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

using DSCript;
using DSCript.Spooling;

namespace Zartex
{
    public class PropHandle
    {
        public static int DataBufferSize = 32;

        [Category("Instance"), Description("Position of this prop")]
        [PropertyOrder(10)]
        public Vector4 Position { get; set; }
        [Category("Instance"), Description("The ID to the instance this prop handle is using.")]
        [PropertyOrder(20)]
        public short InstanceId { get; set; }
        [Category("Misc"), Description("The ID to the prop handle this prop handle is attached to.")]
        [PropertyOrder(30)]
        public short AttachedTo { get; set; }
        [Category("Misc"), Description("XYZ coordinates representing a bounding box.")]
        [PropertyOrder(40)]
        public Vector4 BoundingBox { get; set; }

        public PropHandle() { }
        public PropHandle(Vector4 pos,short instanceId,short attachedTo,Vector4 boundingBox)
        {
            Position = pos; InstanceId = instanceId; AttachedTo = attachedTo; BoundingBox = boundingBox;
        } 
    }
    public class PropHandleData : SpoolableResource<SpoolableBuffer>
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

        public PropHandle[] PropHandles { get; set; }

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

                PropHandles = new PropHandle[count];
                for (int id = 0; id < count; id++)
                {
                    PropHandles[id] = new PropHandle(f.Read<Vector4>(), f.ReadInt16(), f.ReadInt16(), f.Read<Vector4>());
                }
            }
        }
        protected override void Save()
        {
            var bufferSize = HeaderBufferSize+(PropHandle.DataBufferSize*PropHandles.Length);

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

                PropHandles = new PropHandle[count];
                foreach (PropHandle proph in PropHandles)
                {
                    f.Write<Vector4>(proph.Position); f.Write(proph.InstanceId); f.Write(proph.AttachedTo); f.Write<Vector4>(proph.BoundingBox);
                }
            }

            Spooler.SetBuffer(propBuffer);
        }
    }
}
