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
    public enum CollectableType : int
    {
        Pistol = 0, // Tanner's main weapon!
        Beretta = 1,
        Silenced = 2,
        MachineGun = 3, 
        Shotgun = 4, // reserved
        Uzi = 5, // reserved
        SubMachineGun = 6, // reserved
        M4A1 = 7, // reserved
        GrenadeLauncher = 8, // normal grenade launcher
        Medkit = 20,

        // these aren't really important to document at all

        //Grenade = 9, // does not recharge Grenade Launcher

        // debug gunshot models (yes they're animated)
        //PistolGunShot = 10, // nothing happens (not even the sound plays when collected)
        /*
        Unk1 = 11,
        Unk2 = 12,
        Unk3 = 13,
        Unk4 = 14,
        Unk5 = 15,
        Unk6 = 16,
        Unk7 = 17,
        Unk8 = 18,
        Unk9 = 19,
        // (medkit was supposed to be here...)
        Unk11 = 21,
        Unk12 = 22
        */
    }
    public class CollectableObject : MissionObject
    {
        public override int TypeId
        {
            get { return 10; }
        }

        //public int Type { get; set; }
        public CollectableType Type { get; set; }

        public float Rotation { get; set; }
        public Vector3 Position { get; set; }

        protected override void LoadData(Stream stream)
        {
            Type = (CollectableType)stream.ReadInt32();

            Rotation = stream.ReadSingle();
            Position = stream.Read<Vector3>();
        }

        protected override void SaveData(Stream stream)
        {
            stream.Write((int)Type);
            stream.Write(Rotation);
            stream.Write(Position);
        }
    }
}
