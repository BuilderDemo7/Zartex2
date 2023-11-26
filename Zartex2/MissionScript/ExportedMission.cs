using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using DSCript;
using DSCript.Spooling;

namespace Zartex
{
    public class ExportedMission : SpoolableResource<SpoolablePackage>
    {
        public ExportedMissionObjects Objects { get; set; }
        public SpoolableBuffer PropHandles { get; set; } // old class: SpoolableBuffer
        public MissionInstanceData MissionInstances { get; set; }

        public LogicExportData LogicData { get; set; }

        // custom spoolable buffer
        public SpoolablePackage Spooler
        {
            get { return base.Spooler; }
            set { base.Spooler = value; }
        }

        protected override void Load()
        {
            Objects = Spooler.GetFirstChild(ChunkType.ExportedMissionObjects).AsResource<ExportedMissionObjects>(true);
            PropHandles = Spooler.GetFirstChild(ChunkType.ExportedMissionPropHandles) as SpoolableBuffer;
            var chnk = Spooler.GetFirstChild(ChunkType.BuildingInstanceData);
            if (chnk!=null)
               MissionInstances = chnk.AsResource<MissionInstanceData>(true);

            LogicData = Spooler.GetFirstChild(ChunkType.LogicExportData).AsResource<LogicExportData>(true);
        }

        protected override void Save()
        {
            SpoolableResourceFactory.Save(Objects);
            // optional chunk if not present
            if (MissionInstances != null)
            {
                if (MissionInstances.Spooler!=null)
                  SpoolableResourceFactory.Save(MissionInstances);
            }
            SpoolableResourceFactory.Save(LogicData);
        }
    }
}
