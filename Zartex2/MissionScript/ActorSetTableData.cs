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
    public class ActorSet
    {
        public List<int> Sets { get; set; }

        public int this[int index]
        {
            get { return Sets[index]; }
            set { Sets[index] = value; }
        }

        public ActorSet() { Sets = new List<int>();  }
        public ActorSet(int capacity) { Sets = new List<int>(capacity); }
    }
    public class ActorSetTableData : SpoolableResource<SpoolableBuffer>
    {
        // custom spoolable buffer
        public SpoolableBuffer Spooler
        {
            get { return base.Spooler; }
            set { base.Spooler = value; }
        }

        public List<ActorSet> Table;
        protected override void Load()
        {
            using (var f = Spooler.GetMemoryStream())
            {
                var count = f.ReadInt32();
                Debug.WriteLine(count);
                Table = new List<ActorSet>();
                for (int id = 0; id<count; id++)
                {
                    var countOfSets = f.ReadInt32();
                    ActorSet set = new ActorSet();
                    for (int sid = 0; sid<countOfSets; sid++)
                    {
                        set.Sets.Add(f.ReadInt32());
                    }
                    Table.Add(set);
                }
            }
        }

        protected override void Save()
        {
            var bufferSize = 4;
            foreach (ActorSet set in Table)
            {
                bufferSize += 4; // for the count 
                bufferSize += set.Sets.Count * 4; // for the sets
            }

            var buffer = new byte[bufferSize];

            using (var f = new MemoryStream(buffer))
            {
                f.Write(Table.Count);
                foreach (ActorSet set in Table)
                {
                    f.Write((int)set.Sets.Count);
                    foreach (int setId in set.Sets)
                    {
                        f.Write((int)setId);
                    }
                }
            }

            Spooler.SetBuffer(buffer);
        }
    }
}
