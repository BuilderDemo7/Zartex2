using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

using DSCript;
using DSCript.Spooling;

namespace Zartex
{
    public class MissionScriptFile : FileChunker
    {
        // DPL vars
        public bool isDriverPLMission = false;
        public int MissionIndex = 0;
        private int _mid = 0;

        public ExportedMission MissionData { get; set; }
        public MissionSummaryData MissionSummary { get; set; } // old class: SpoolableBuffer
        public ActorDefinition[] ActorSetTable { get; set; }

        public bool hasBuildInfo { get { return BuildInfo != null; } }
        public string BuildInfo = null;

        private SpoolableBuffer _buildinfo = null;
        public List<ExportedMission> Missions = new List<ExportedMission>();
        public List<string> SpooledLocaleTexts = new List<string>();

        public string Era = "then";

        public bool IsLoaded { get; set; }

        public string FileName { get; set; }

        public bool HasLocale { get; private set; }

        public Spooler Spooler { get; set; }

        public SpoolerCollection Children { get; set; }

        public Dictionary<int, string> LocaleStrings { get; set; }

        public bool HasLocaleString(int id)
        {
            return (LocaleStrings != null) ? LocaleStrings.ContainsKey(id) : false;
        }

        public string GetLocaleString(int id)
        {
            if (HasLocaleString(id))
            {
                var str = LocaleStrings[id];

                return (!String.IsNullOrEmpty(str)) ? str : "<NULL>";
            }
            return "<???>";
        }

        public void LoadLocaleFile(int missionId)
        {
            LoadLocaleFile(MPCFile.GetMissionLocaleFilepath(missionId));
        }

        public void LoadLocaleFile(string filename)
        {
            string text = String.Empty;

            if (!File.Exists(filename))
                return;

            using (var fs = File.Open(filename, FileMode.Open, FileAccess.Read))
            {
                // DPL temporary workaround
                var encoding = (((fs.ReadInt32() >> 16) & 0xFFFF) == 0xFEFF) ? Encoding.Unicode : Encoding.UTF8;

                if (encoding != Encoding.Unicode)
                    fs.Seek(0, SeekOrigin.Begin);

                using (StreamReader f = new StreamReader(fs, encoding, true))
                {
                    text = f.ReadToEnd();
                }
            }

            LocaleStrings = new Dictionary<int, string>();

            var e_ENTRIES = @"(<ID\b[^>]*>.*?<\/TEXT>)";
            var e_ID = @"<ID\b[^>]*>(.*?)<\/ID>";
            var e_TEXT = @"<TEXT\b[^>]*>(.*?)<\/TEXT>";

            foreach (Match m in Regex.Matches(text, e_ENTRIES))
            {
                var val = m.Value;

                var idStr = Regex.Match(val, e_ID).Groups[1].Value;
                var str = Regex.Match(val, e_TEXT).Groups[1].Value;

                var id = int.Parse(idStr);

                if (!LocaleStrings.ContainsKey(id))
                    LocaleStrings.Add(id, str);
            }

            HasLocale = true;
        }

        protected override void OnSpoolerLoaded(Spooler sender, EventArgs e)
        {
            switch ((ChunkType)sender.Context)
            {
            case ChunkType.ExportedMissionChunk:
                    {
                        Missions.Add(sender.AsResource<ExportedMission>(true));
                        if (_mid == MissionIndex) {
                            MissionData = Missions[_mid]; //sender.AsResource<ExportedMission>(true);
                        }
                        if (isDriverPLMission)
                            _mid++;
                    } break;
                case ChunkType.BuildInfo:
                    {
                        if (_mid == MissionIndex)
                        {
                            _buildinfo = sender as SpoolableBuffer;
                            BuildInfo = _buildinfo.GetMemoryStream().Read<string>(_buildinfo.Size);
                        }
                    }
                    break;
            case ChunkType.MissionSummary:
                    {
                        if (_mid == MissionIndex)
                        {
                            MissionSummary = sender.AsResource<MissionSummaryData>(true);
                            MissionSummary.DPL = isDriverPLMission;
                            //MissionSummary = sender as SpoolableBuffer;

                            if (MissionSummary.MissionLocaleId > -1)
                                LoadLocaleFile(MissionSummary.MissionLocaleId);
                        }
                        break;
                    }
            }

            // fire the event handler
            base.OnSpoolerLoaded(sender, e);
        }

        protected override void OnFileSaveBegin()
        {
            // build info buffer is null but there is a build info from the local string
            if (_buildinfo == null & hasBuildInfo == true)
            {
                _buildinfo = new SpoolableBuffer()
                {
                    Context = (int)ChunkType.BuildInfo,
                    Description = "Build info"
                };
                this.Children.Add(_buildinfo);
            }
            if (_buildinfo != null & BuildInfo != null)
                _buildinfo.SetBuffer(Encoding.ASCII.GetBytes(BuildInfo));

            SpoolableResourceFactory.Save(MissionData);
            SpoolableResourceFactory.Save(MissionSummary);
        }

        public MissionScriptFile() : base() { }
        public MissionScriptFile(string filename, bool driverPLMission = false, int missionIndex = 0) {
            MissionIndex = missionIndex;
            isDriverPLMission = driverPLMission;
            Load(filename); // loads file
            Era = filename.ToLower().Contains("then") ? "then" : "now";
        }
    }
}
