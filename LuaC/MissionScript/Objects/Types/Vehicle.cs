using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;

using DSCript;

using Zartex.Converters;

namespace Zartex
{
    public enum VehicleUID : int
    {
        //Camaro = 0,                 // speculation: cut very early in development; Stuntman car?
        //Capri = 1,                  // speculation: cut very early in development; Stuntman car?
        Taxi = 2,
        //Mustang_Blue = 3,           // speculation: used in early alpha before cars had colors
        //Mustang_White = 4,          // speculation: used in early alpha before cars had colors
        //Mustang_Yellow = 5,         // speculation: used in early alpha before cars had colors
        Mustang_Grey = 6,           // speculation: used in early alpha before cars had colors; why this became the final vehicle? who knows..
        //Mustang_Red = 7,            // speculation: used in early alpha before cars had colors
        Dougal = 8,                 // speculation: cut very early in development; what the heck is a Dougal?! something from Stuntman?
        CopCar_Miami = 9,
        Jalpa = 10,
        Vantage = 11,                // speculation: alternative version of AstonMartin?
        //Citroen2CV = 12,             // cut late in development, seen in few screenshots
        Countach = 13,
        CopCar_Istanbul = 14,
        DodgeTruck = 15,
        Bora = 16,
        Panda = 17,
        //HierarchyTest = 18,          // speculation: used for testing 18-wheelers?
        RigTruck = 19,
        RigTrailer = 20,
        IstanbulTaxi = 21,
        Challenger = 22,
        ChevyVan = 23,
        MiamiFordPickup = 24,
        CitroenCX = 25,
        //Starion = 26,                // speculation: cut late in development (it's an unreleased cut car, so we'll never know)
        BMW507 = 27,
        Datsun240Z = 28,
        ChevyBelaire = 29,
        IstanbulFordPickup = 30,
        Ducati = 31,
        ChevyBlazer = 32,
        Ferrari = 33,
        TransAm = 34,
        Bentley = 35,
        Dumpster = 36,
        Corvette = 37,
        Torino = 38,
        Miami_Bus = 39,
        Lincoln = 40,
        AuburnSpeedster = 41,
        Hotrod = 42,
        Gokart = 43,
        VRod = 44,
        Scarab = 45,                 // speculation: Tanner originally had his own unique speedboat; later became the only speed boat
        Miami_Cruiser_Boat = 46,
        Miami_Small_Boat = 47,
        //Miami_Speed_Boat = 48,       // speculation: cut early in development - Scarab used instead?
        MetroMover = 49,
        BMW_Alpina = 50,
        Jaguar = 51,
        RenaultVan = 52,
        RenaultVanPickup = 53,
        Forklift = 54,
        AstonMartin = 55,
        //Citroen_DS = 56,             // cut very early in development, seen in a couple of screenshots (as a cop car!)
        CopCar_Nice = 57,
        Nice_Bus = 58,
        Nice_Van = 59,
        Nice_Truck = 60,
        Nice_Taxi = 61,
        Reanult_5 = 62,
        //Porsche = 63,                // speculation: cut very early in development
        //SwatVan = 64,                // speculation: cut due to time constraints, so replaced with the Sobe truck gimmick xD
        CamperVan = 65,
        Cobra = 66,
        Nice_SportsBike = 67,
        Nice_Moped = 68,
        Nice_SpeedBoat = 69,
        Nice_Cruiser_Boat = 70,
        Nice_Small_Boat = 71,
        Chevy_Impala = 72,
        Istanbul_Bus = 73,
        RenaultAlpine = 74,
        Wagonaire = 75,
        MackTruck = 76,
        Merc = 77,
        Bugatti = 78,
        Racer = 79,
        GT40 = 80,
        Istanbul_StreetBike = 81,
        Istanbul_Moped = 82,
        Fishing_Boat = 83,
        Big_Cruiser_Boat = 84,
        Istanbul_SpeedBoat = 85,
        Tram = 86,
        Train = 87,
        RigCarrierTrailer = 88,
        Miami_Cop_Boat = 89,
        Nice_Cop_Boat = 90,
        Istanbul_Cop_Boat = 91,
        CitroenZX = 92,
        VW_Passat = 93,
        MercSL500 = 94,
        Carriage = 95,
        /*

        // these were never used/defined, so names are placeholders
        Extra1 = 96,
        Extra2 = 97,
        Extra3 = 98,
        Extra4 = 99,
        Extra5 = 100,
        Extra6 = 101,
        Extra7 = 102,

        // end of the list
        MaxVehicles = 103,
        */
    };

    public class VehicleObject : MissionObject
    {
        public override int TypeId
        {
            get { return 1; }
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
        public int UID { get; set; }
        //public VehicleUID VehicleID { get; set; }

        protected override void LoadData(Stream stream)
        {
            Position = stream.Read<Vector3>();
            UID = stream.ReadInt32();
            //UID = stream.Read<VehicleUID>(); // stream.ReadInt32()
        }

        protected override void SaveData(Stream stream)
        {
            stream.Write(Position);
            stream.Write(UID);
        }

        protected override void LoadCreationData(Stream stream)
        {
            CreationData = stream.ReadAllBytes();
        }

        protected override void SaveCreationData(Stream stream)
        {
            byte[] bX = BitConverter.GetBytes(Position.X);
            byte[] bY = BitConverter.GetBytes(Position.Y);
            byte[] bZ = BitConverter.GetBytes(Position.Z);
            // don't worry, this works, it's just more lines of code...
            CreationData[44] = bX[0]; CreationData[44+1] = bX[1]; CreationData[44+2] = bX[2]; CreationData[44+3] = bX[3];
            CreationData[44+4] = bY[0]; CreationData[44 + 5] = bY[1]; CreationData[44 + 6] = bY[2]; CreationData[44 + 7] = bY[3];
            CreationData[44+8] = bZ[0]; CreationData[44 + 9] = bZ[1]; CreationData[44 + 10] = bZ[2]; CreationData[44 + 11] = bZ[3];
            stream.Write(CreationData); // write the creation data...
        }
    }
}
