using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

using DSCript;
using DSCript.Spooling;

namespace Zartex
{
    public enum MissionCityType : int
    {
        [Description("Miami at Day")]
        Miami_Day        = 1,
        [Description("Miami at Night")]
        Miami_Night      = 2,

        [Description("Nice at Day")]
        Nice_Day         = 3,
        [Description("Nice at Night")]
        Nice_Night       = 4,

        [Description("Istanbul at Day")]
        Istanbul_Day     = 5,
        [Description("Istanbul at Night")]
        Istanbul_Night   = 6
    }

    public class MissionSummaryData : SpoolableResource<SpoolableBuffer>
    {
        public bool DPL = false;
        [Category("Misc")]
        private const int DensityDataMagic = 0x55264524;

        [Category("Misc")]
        [ReadOnly(true)]
        [DisplayName("Has Density Data")]
        [Description("If density data is present.")]
        public bool HasDensityData { get; set; }

        //public double[] StartPosition = { 0.0f, 0.0f };
        [Category("Scenario")]
        [DisplayName("Start Position")]
        [Description("Start position of the mission before the game fully loads.")]
        public Vector2 StartPosition { get; set; } //new Vector2(0f, 0f);

        [Category("Misc")]
        [DisplayName("Mission Locale")]
        [Description("Mission localised text ID.")]
        public short MissionLocaleId { get; set; }

        [Category("Scenario")]
        [DisplayName("City")]
        [Description("The city where the mission is located.")]
        public MissionCityType CityType { get; set; }
        [Category("Scenario")]
        [DisplayName("Mission Mood")]
        [Description("Mission mood ID.")]
        public short MissionId { get; set; }

        [Category("Traffic")]
        [DisplayName("Density Override")]
        [Description("No research so far was done on this.")]
        public bool DensityOverride { get; set; }
        [Category("Traffic")]
        [DisplayName("Parked Car Density")]
        [Description("Density of parked cars")]
        public int ParkedCarDensity { get; set; }
        [Category("Traffic")]
        [DisplayName("Attractor Parked Cars Density")]
        [Description("No research so far was done on this.")]
        public int AttractorParkedCarDensity { get; set; }
        [Category("Traffic")]
        [DisplayName("Ping In Radius")]
        [Description("A range of incoming characters, cars, etc.")]
        public double PingInRadius { get; set; }
        [Category("Traffic")]
        [DisplayName("Ping Out Radius")]
        [Description("A range of outcoming characters, cars, etc.")]
        public double PingOutRadius { get; set; }
        [Category("Mission")]
        [DisplayName("Flags")]
        [Description("Extra information stored for the mission. (Driver: Parallel Lines missions only)")]
        [Browsable(true)]
        public int Flags { get; set; }

        public byte[] DPLBuffer = new byte[0x1C];

        [DisplayName("Are Changes Pending")]
        [Description("")]
        [ReadOnly(true)]
        [Browsable(false)]
        public override bool AreChangesPending => base.AreChangesPending;

        public string GetSummaryAsString(bool forceDensityData = false)
        {
            var sb = new StringBuilder();

            sb.AppendLine("-- Mission Summary Data --");
            sb.AppendLine("StartPosition: [{0:F6}, {1:F6}]", StartPosition.X, StartPosition.Y/*StartPosition[0], StartPosition[1]*/);
            sb.AppendLine("CityType: {0}", CityType);
            sb.AppendLine("MissionId: {0}", MissionId);
            sb.AppendLine("MissionLocaleId: {0}", MissionLocaleId);

            if (HasDensityData || forceDensityData)
            {
                sb.AppendLine();

                sb.AppendLine("-- Density Override Data --");
                sb.AppendLine("Overrides enabled: {0}", DensityOverride);
                sb.AppendLine("ParkedCarDensity: {0}", ParkedCarDensity);
                sb.AppendLine("AttractorParkedCarDensity: {0}", AttractorParkedCarDensity);
                sb.AppendLine("PingInRadius: {0:F6}", PingInRadius);
                sb.AppendLine("PingOutRadius: {0:F6}", PingOutRadius);
            }

            return sb.ToString();
        }

        protected override void Load()
        {
            using (var f = Spooler.GetMemoryStream())
            {
                if (DPL)
                    DPLBuffer = f.ReadAllBytes();
                if (!DPL)
                {
                    //StartPosition[0] = f.ReadFloat();
                    //StartPosition[1] = f.ReadFloat();
                    StartPosition = f.Read<Vector2>(8);

                    CityType = (MissionCityType)f.ReadInt32();

                    // NOTE: Inconsistent usage of these variables!
                    MissionId = f.ReadInt16();
                    MissionLocaleId = f.ReadInt16();

                    HasDensityData = (Spooler.Size > 0x10);

                    // new format or old format?
                    if (HasDensityData && (f.ReadInt32() == DensityDataMagic))
                    {
                        // value in first byte, rest is padding
                        DensityOverride = ((f.ReadInt32() & 0xFF) == 1) ? true : false;

                        ParkedCarDensity = f.ReadInt32();
                        AttractorParkedCarDensity = f.ReadInt32();

                        PingInRadius = f.ReadFloat();
                        PingOutRadius = f.ReadFloat();
                    }
                    else
                    {
                        // this is how the game handles the old format
                        // overrides are still disabled, however
                        ParkedCarDensity = 7;
                        AttractorParkedCarDensity = 1;

                        PingInRadius = 130.0;
                        PingOutRadius = 135.0;
                    }
                }
                // Driver PL format
                else
                {
                    
                    /*
                    Console.WriteLine($"DPL MSD POS: {f.Position}");
                    MissionId = (short)f.ReadInt16();
                    MissionLocaleId = (short)f.ReadInt16();

                    StartPosition = f.Read<Vector2>();
                    Flags = f.ReadInt32();
                    */
                }
            }
        }

        private byte[] SaveForDPL()
        {
            int bufSize = 0x1C;

            var mBuffer = new byte[bufSize];

            using (var fM = new MemoryStream(mBuffer))
            {
                fM.Write(DPLBuffer);
                /*
                fM.Write((long)1);
                fM.Write((int)3);
                fM.Write((short)MissionId);
                fM.Write((short)MissionLocaleId);
                */

                //fM.WriteFloat(StartPosition.X/*StartPosition[0]*/);
                //fM.WriteFloat(StartPosition.Y/*StartPosition[1]*/);
                /*
                fM.Write((uint)0xCCCCCCCC);
                fM.Write((uint)0xCCCCCCCC);

                fM.Write(0x0); fM.Write(0x0); fM.Write(0x1); fM.Write(0x1);
                */
            }

            //Spooler.SetBuffer(mBuffer);
            return mBuffer;
        }

        public byte[] SaveForD3()
        {
            int bufSize = (HasDensityData) ? 0x28 : 0x10;

            var mBuffer = new byte[bufSize];

            using (var fM = new MemoryStream(mBuffer))
            {
                fM.WriteFloat(StartPosition.X/*StartPosition[0]*/);
                fM.WriteFloat(StartPosition.Y/*StartPosition[1]*/);

                fM.Write((int)CityType);

                fM.Write(MissionId);
                fM.Write(MissionLocaleId);

                if (HasDensityData)
                {
                    fM.Write(DensityDataMagic);

                    // combine value and padding into one
                    fM.Write((0xCCCCCC << 8) | ((DensityOverride) ? 1 : 0));

                    fM.Write(ParkedCarDensity);
                    fM.Write(AttractorParkedCarDensity);

                    fM.WriteFloat(PingInRadius);
                    fM.WriteFloat(PingOutRadius);
                }
            }

            //Spooler.SetBuffer(mBuffer);
            return mBuffer;
        }

        protected override void Save()
        {
            Spooler.SetBuffer(DPL ? SaveForDPL() : SaveForD3());
            if (Spooler==null)
            {
                Spooler.SetBuffer(DPLBuffer);
            }
        }

        //public MissionSummaryData(bool isDriverPL = false) { _dpl = isDriverPL; }
    }
}
