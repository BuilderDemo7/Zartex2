﻿using System;
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
    public class CharacterObject : MissionObject
    {
        public override int TypeId
        {
            get { return 2; }
        }

        public override bool HasCreationData
        {
            get { return true; }
        }

        protected override int Alignment
        {
            get { return 4; }
        }

        public byte[] CreationData { get; set; }

        public Vector3 Position { get; set; }

        [TypeConverter(typeof(HexStringConverter))]
        public uint SkinId { get; set; }

        [TypeConverter(typeof(HexStringConverter))]
        public int UID { get; set; }

        protected override void LoadData(Stream stream)
        {
            Position = stream.Read<Vector3>();
            SkinId = stream.Read<uint>();
            UID = stream.ReadInt32();
        }

        protected override void SaveData(Stream stream)
        {
            stream.Write(Position);
            stream.Write(SkinId);
            stream.Write(UID);
        }

        protected override void LoadCreationData(Stream stream)
        {
            CreationData = stream.ReadAllBytes();
        }

        protected override void SaveCreationData(Stream stream)
        {
            stream.Write(CreationData);
        }
    }
}
