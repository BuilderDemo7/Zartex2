using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DSCript;
using DSCript.Spooling;

namespace Zartex
{
    public class LookupEntry
    {
        public static int BufferSize = 6;
        public short Entry1; // sub mission ID
        public int Entry2; // mission ID (chunk ID)

        public LookupEntry() { }
        public LookupEntry(short entry1, int entry2) { Entry1 = entry1; Entry2 = entry2; } 
    }

    public class SpoolSystemLookup : SpoolableResource<SpoolableBuffer> 
    {
        public static int HeaderBufferSize = 0x1C;

        public List<LookupEntry> Lookups = new List<LookupEntry>();
        public SpoolableBuffer Spooler
        {
            get { return base.Spooler; }
            set { base.Spooler = value; }
        }
        public bool BigEndianHack = false; // just in case

        public short ShortFlags;
        // entriesCount 
        public int Flags;
        public int SpoolFlags;
        public short Unk1;
        public long Unk2;
        public int Unk3;

        protected override void Load()
        {
            using (var f = Spooler.GetMemoryStream())
            {
                ShortFlags = f.ReadInt16(BigEndianHack);
                int count = f.ReadInt32(BigEndianHack);
                Flags = f.ReadInt32(BigEndianHack);
                SpoolFlags = f.ReadInt32(BigEndianHack);
                Unk1 = f.ReadInt16(BigEndianHack);
                Unk2 = f.ReadInt64(BigEndianHack);
                Unk3 = f.ReadInt32(BigEndianHack);

                Lookups = new List<LookupEntry>();
                for (int id = 0; id<count; id++)
                {
                    Lookups.Add(new LookupEntry(f.ReadInt16(), f.ReadInt32()));
                }
            }
        }
        protected override void Save()
        {
            var bufferSize = HeaderBufferSize + (Lookups.Count * LookupEntry.BufferSize);

            byte[] buffer = new byte[bufferSize];

            using (var f = new MemoryStream(buffer))
            {
                f.Write(ShortFlags);
                f.Write((int)Lookups.Count);
                f.Write(Flags);
                f.Write(SpoolFlags);
                f.Write(Unk1);
                f.Write(Unk2);
                f.Write(Unk3);
                foreach(LookupEntry entry in Lookups)
                {
                    f.Write((short)entry.Entry1);
                    f.Write((int)entry.Entry2);
                }
            }

            Spooler.SetBuffer(buffer);
        }
    }
}
