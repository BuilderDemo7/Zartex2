using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;

using DSCript;
using DSCript.Spooling;

namespace Zartex
{
    public class SoundBankTableData : SpoolableResource<SpoolableBuffer>
    {
        // custom public spoolable buffer
        public SpoolableBuffer Spooler
        {
            get { return base.Spooler; }
            set { base.Spooler = value; }
        }

        public List<int> Table { get; set; }

        public int Count
        {
            get { return Table.Count; }
        }

        public int this[int index]
        {
            get { return Table[index]; }
            set { Table[index] = value; }
        }

        public int AppendBank(int bank)
        {
            if (!Table.Contains(bank))
            {
                var idx = Table.Count;
                Table.Add(bank);
                return idx;
            }
            return bank;
        }

        protected override void Load()
        {
            using (var f = Spooler.GetMemoryStream())
            {
                var baseOffset = f.Position;
                var nBanks = f.ReadInt32();
                Debug.WriteLine($"Count -> {nBanks}");

                Table = new List<int>(nBanks);

                for (int i = 0; i < nBanks; i++)
                {
                    var bank = f.ReadInt32();
                    Debug.WriteLine($"Load => Bank No. {i} : {bank}");
                    Table.Add(bank);
                }
            }
        }

        protected override void Save()
        {
            var nBanks = Table.Count;

            var bufferSize = 4 + (nBanks * 4);

            var bnkBuffer = new byte[bufferSize];

            using (var fBnk = new MemoryStream(bnkBuffer))
            {
                fBnk.Write(nBanks);

                int id = 0;
                foreach (var bank in Table)
                {
                    //Debug.WriteLine($"Save => Bank No. {id} : {bank}");
                    fBnk.Write(bank);

                    id++;
                }
            }

            Spooler.SetBuffer(bnkBuffer);
        }
    }
}
