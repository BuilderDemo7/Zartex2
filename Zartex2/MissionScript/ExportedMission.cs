using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DSCript;
using DSCript.Spooling;

namespace Zartex
{
    public class ExportedMission : SpoolableResource<SpoolablePackage>
    {
        public ExportedMissionObjects Objects { get; set; }
        public PropHandleData PropHandles { get; set; } // old class: SpoolableBuffer

        public LogicExportData LogicData { get; set; }

        protected override void Load()
        {
            Objects = Spooler.GetFirstChild(ChunkType.ExportedMissionObjects).AsResource<ExportedMissionObjects>(true);
            //PropHandles = Spooler.GetFirstChild(ChunkType.ExportedMissionPropHandles) as SpoolableBuffer;
            PropHandles = Spooler.GetFirstChild(ChunkType.ExportedMissionPropHandles).AsResource<PropHandleData>(true);

            LogicData = Spooler.GetFirstChild(ChunkType.LogicExportData).AsResource<LogicExportData>(true);
        }

        protected override void Save()
        {
            SpoolableResourceFactory.Save(Objects);
            SpoolableResourceFactory.Save(PropHandles);
            SpoolableResourceFactory.Save(LogicData);
        }
    }
}
