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

        protected override void Load()
        {
            Objects = Spooler.GetFirstChild(ChunkType.ExportedMissionObjects).AsResource<ExportedMissionObjects>(true);
            PropHandles = Spooler.GetFirstChild(ChunkType.ExportedMissionPropHandles) as SpoolableBuffer;
            MissionInstances = Spooler.GetFirstChild(ChunkType.BuildingInstanceData).AsResource<MissionInstanceData>(true);

            LogicData = Spooler.GetFirstChild(ChunkType.LogicExportData).AsResource<LogicExportData>(true);
        }

        protected override void Save()
        {
            SpoolableResourceFactory.Save(Objects);
            SpoolableResourceFactory.Save(MissionInstances);
            SpoolableResourceFactory.Save(LogicData);
        }
    }
}
