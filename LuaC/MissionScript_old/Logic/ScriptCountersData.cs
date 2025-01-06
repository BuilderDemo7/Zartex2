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
    public class ScriptCountersData : SpoolableResource<SpoolableBuffer>
    {
        // custom public spoolable buffer
        public SpoolableBuffer Spooler
        {
            get { return base.Spooler; }
            set { base.Spooler = value; }
        }

        public List<int> Counters { get; set; }

        public int Count
        {
            get { return Counters.Count; }
        }

        public int this[int index]
        {
            get { return Counters[index]; }
            set { Counters[index] = value; }
        }

        public int AppendActor(int Actor)
        {
            if (!Counters.Contains(Actor))
            {
                var idx = Counters.Count;
                Counters.Add(Actor);
                return idx;
            }
            return Actor;
        }

        protected override void Load()
        {
            using (var f = Spooler.GetMemoryStream())
            {
                var baseOffset = f.Position;
                var nActors = f.ReadInt32();

                Counters = new List<int>(nActors);

                for (int i = 0; i < nActors; i++)
                {
                    var Actor = f.ReadInt32();
                    Counters.Add(Actor);
                }
            }
        }

        protected override void Save()
        {
            var nActors = Counters.Count;

            var bufferSize = 4 + (nActors * 4);

            var bnkBuffer = new byte[bufferSize];

            using (var fBnk = new MemoryStream(bnkBuffer))
            {
                fBnk.Write(nActors);

                foreach (int Actor in Counters)
                {
                    //Debug.WriteLine($"Save => Actor No. {id} : {Actor}");
                    fBnk.Write(Actor);
                }
            }

            Spooler.SetBuffer(bnkBuffer);
        }
    }
}
