using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;

using DSCript;

using Zartex.Converters;

namespace Zartex
{
    public class AreaObject : MissionObject
    {
        public override int TypeId
        {
            get { return 4; }
        }

        public override bool HasCreationData
        {
            get { return true; }
        }

        //public byte[] CreationData { get; set; }

        public int Data1 { get; set; }
        public int Data2 { get; set; }
        public int Data3 { get; set; }
        public int Data4 { get; set; }

        public Vector4 AreaScale { get; set; }
        public Vector4 AreaPosition { get; set; }

        public int Data5 { get; set; }
        public Vector3 V1 { get; set; }

        protected override void LoadData(Stream stream)
        {
        }

        protected override void SaveData(Stream stream)
        {
        }

        protected override void LoadCreationData(Stream stream)
        {
            //CreationData = stream.ReadAllBytes();
            Data1 = stream.ReadInt32();
            Data2 = stream.ReadInt32();
            Data3 = stream.ReadInt32();
            Data4 = stream.ReadInt32();
            AreaScale = stream.Read<Vector4>();
            AreaPosition = stream.Read<Vector4>();
            Data5 = stream.ReadInt32();
            V1 = stream.Read<Vector3>();
        }

        protected override void SaveCreationData(Stream stream)
        {
            //stream.Write(CreationData);
            stream.Write(Data1);
            stream.Write(Data2);
            stream.Write(Data3);
            stream.Write(Data4);
            stream.Write<Vector4>(AreaScale);
            stream.Write<Vector4>(AreaPosition);
            stream.Write(Data5);
            stream.Write<Vector3>(V1);
        }
    }
}
