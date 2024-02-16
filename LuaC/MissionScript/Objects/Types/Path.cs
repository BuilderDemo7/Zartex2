using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;

using DSCript;

namespace Zartex
{
    public class PathObject : MissionObject
    {
        public override int TypeId
        {
            get { return 5; }
        }
        
        public override bool HasCreationData
        {
            get { return true; }
        }

        public byte[] CreationData { get; set; }

        [ReadOnly(true)]
        public long Count { get; set; }

        public Vector4[] Path { get; set; }

        protected override void LoadData(Stream stream)
        {
            Count = BitConverter.ToInt64(CreationData, 0);
            Path = new Vector4[Count];
            for (int id=0;id<Count;id++)
            {
                int baseOff = 16 * (id + 1);
                Path[id] = new Vector4(BitConverter.ToSingle(CreationData, baseOff), BitConverter.ToSingle(CreationData, baseOff + 4 ), BitConverter.ToSingle(CreationData, baseOff + 8), BitConverter.ToSingle(CreationData, baseOff + 12));
            }
        }

        protected override void SaveData(Stream stream)
        {
            return; // nothing to do here
        }

        protected override void LoadCreationData(Stream stream)
        {
            CreationData = stream.ReadAllBytes();
        }

        protected override void SaveCreationData(Stream stream)
        {
            Count = Path.Length; // update count
            byte[] bCount = BitConverter.GetBytes(Count);
            byte[] bData = new byte[16 * (Count + 1)];
            // the WORST way to write in a byte array
            bData[0] = bCount[0]; bData[1] = bCount[1]; bData[2] = bCount[2]; bData[3] = bCount[3]; bData[4] = bCount[4]; bData[5] = bCount[5]; bData[6] = bCount[6];
            bData[7] = bCount[7];
            for (int id = 0; id < Count; id++)
            {
                //Debug.Write($"Rewriting Path Vector4 {id}/{Count}");
                int baseOff = 16 * (id + 1);
                byte[] X = BitConverter.GetBytes(Path[id].X);
                byte[] Y = BitConverter.GetBytes(Path[id].Y);
                byte[] Z = BitConverter.GetBytes(Path[id].Z);
                byte[] W = BitConverter.GetBytes(Path[id].W);
                // again... the WORST way to write in a byte array
                bData[baseOff] = X[0]; bData[baseOff + 1] = X[1]; bData[baseOff + 2] = X[2]; bData[baseOff + 3] = X[3];
                bData[baseOff + 4] = Y[0]; bData[baseOff + 5] = Y[1]; bData[baseOff + 6] = Y[2]; bData[baseOff + 7] = Y[3];
                bData[baseOff + 8] = Z[0]; bData[baseOff + 9] = Z[1]; bData[baseOff + 10] = Z[2]; bData[baseOff + 11] = Z[3];
                bData[baseOff + 12] = W[0]; bData[baseOff + 13] = W[1]; bData[baseOff + 14] = W[2]; bData[baseOff + 15] = W[3];
            }
            CreationData = bData; // send new creation data
            stream.Write(CreationData);
        }
    }
}
