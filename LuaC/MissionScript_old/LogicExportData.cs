using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using DSCript;
using DSCript.Spooling;

namespace Zartex
{
    public class LogicExportData : SpoolableResource<SpoolablePackage>
    {
        // custom public spoolable buffer
        public SpoolablePackage Spooler
        {
            get { return base.Spooler; }
            set { base.Spooler = value; }
        }

        public StringCollectionData StringCollection { get; set; }
        public SoundBankTableData SoundBankTable { get; set; }

        public LogicDataCollection<ActorDefinition> Actors { get; set; }
        public LogicDataCollection<NodeDefinition> Nodes { get; set; }

        public ActorSetTableData ActorSetTable { get; set; } // old class: SpoolableBuffer
        public WireCollectionData WireCollection { get; set; }
        public ScriptCountersData ScriptCounters { get; set; }
        
        protected override void Load()
        {
            StringCollection = Spooler.GetFirstChild(ChunkType.LogicExportStringCollection).AsResource<StringCollectionData>(true);
            //SoundBankTable = Spooler.GetFirstChild(ChunkType.LogicExportSoundBank) as SpoolableBuffer;
            SoundBankTable = Spooler.GetFirstChild(ChunkType.LogicExportSoundBank).AsResource<SoundBankTableData>(true);

            Actors = Spooler.GetFirstChild(ChunkType.LogicExportActorsChunk).AsResource<LogicDataCollection<ActorDefinition>>(true);
            Spooler logicNodes = Spooler.GetFirstChild(ChunkType.LogicExportNodesChunk);
            //Debug.WriteLine($"[ZARTEX] Logic Nodes: Offset = {logicNodes.BaseOffset}, Size = {logicNodes.Size}, Context = {logicNodes.Context:C4}");
            Nodes = logicNodes.AsResource<LogicDataCollection<NodeDefinition>>(true);

            //ActorSetTable = Spooler.GetFirstChild(ChunkType.LogicExportActorSetTable) as SpoolableBuffer;
            ActorSetTable = Spooler.GetFirstChild(ChunkType.LogicExportActorSetTable).AsResource<ActorSetTableData>(true);

            WireCollection = Spooler.GetFirstChild(ChunkType.LogicExportWireCollections).AsResource<WireCollectionData>(true);
            ScriptCounters = Spooler.GetFirstChild(ChunkType.LogicExportScriptCounters).AsResource<ScriptCountersData>(true);
        }

        protected override void Save()
        {
            if (Spooler.Parent == null)
                //Debug.WriteLine("What have you done to LEND!?");
                throw new InvalidOperationException("What have you done!");

            SpoolableResourceFactory.Save(StringCollection);
            SpoolableResourceFactory.Save(SoundBankTable);
            SpoolableResourceFactory.Save(Actors);
            SpoolableResourceFactory.Save(Nodes);
            SpoolableResourceFactory.Save(ActorSetTable);
            SpoolableResourceFactory.Save(WireCollection);
            SpoolableResourceFactory.Save(ScriptCounters);
        }
    }
}
