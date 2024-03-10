using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;

using DSCript;

namespace Zartex
{
    public class CameraObject : MissionObject
    {
        public override int TypeId
        {
            get { return 7; }
        }
        
        public Vector3 Right { get; set; }
        public Vector3 Up { get; set; }
        public Vector3 Forward { get; set; }
        public Vector3 Position { get; set; }

        public int Reserved { get; set; }

        protected override void LoadData(Stream stream)
        {
            Right = stream.Read<Vector3>();
            Up = stream.Read<Vector3>();
            Forward = stream.Read<Vector3>();
            Position = stream.Read<Vector3>();

            Reserved = stream.ReadInt32();
        }

        protected override void SaveData(Stream stream)
        {
            stream.Write(Right);
            stream.Write(Up);
            stream.Write(Forward);
            stream.Write(Position);

            stream.Write(Reserved);
        }
    }
}
