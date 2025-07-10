using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

using DSCript;
using DSCript.Spooling;

namespace Zartex.ScriptEditor
{
    public partial class D3M_ScriptEditor : UserControl
    {
        public FileStream ScriptFile { get; set; }
        public string[] ScriptLines { get
            {
                return ScriptTB.Text.Split('\n');
            }
        }

        public D3M_ScriptEditor()
        {
            InitializeComponent();

            ScriptTB.VScroll += (s, e) => LineNumPanel.Invalidate();
            ScriptTB.FontChanged += (s, e) => LineNumPanel.Invalidate();
            ScriptTB.TextChanged += (s, e) => LineNumPanel.Invalidate();
            ScriptTB.TextChanged += ScriptTB_TextChanged;
        }

        public void Shutdown()
        {
            if (ScriptFile != null)
                ScriptFile.Close();
        }

        private int GetFirstVisibleLine()
        {
            return ScriptTB.GetLineFromCharIndex(ScriptTB.GetCharIndexFromPosition(new Point(0,0)));
        }

        private int GetTextCharIndexFromLine(int line)
        {
            int charidx = 0;
            for (int id = 0; id < line; id++)
                charidx += ScriptLines[id].Length + 1;

            return charidx;
        }

        private void LinesPan_Paint(object sender, PaintEventArgs e)
        {
            int firstVisibleLine = GetFirstVisibleLine();
            int totalLines = ScriptTB.Lines.Length;

            float lineHeight = ScriptTB.Font.GetHeight();

            for (int i = 0; i < totalLines; i++)
            {
                int lineNumber = firstVisibleLine + i + 1; // /*firstVisibleLine + */i + 1;
                float yPosition = i * lineHeight - ScriptTB.GetPositionFromCharIndex(0).Y; // /*i * lineHeight - */ScriptTB.GetPositionFromCharIndex(GetTextCharIndexFromLine(i)).Y;

                //if (yPosition < i * lineHeight - ScriptTB.GetPositionFromCharIndex(0).Y) continue;
                if (yPosition > LineNumPanel.Height) break;

                e.Graphics.DrawString($"{lineNumber:D4}", ScriptTB.Font, Brushes.Black, 5, yPosition);
            }
        }

        private void ApplyHighlighting(string text)
        {
            // logic keywords (e.g., on_event, else, for)
            HighlightWords(text, @"\b(on_event|onevent|end|on_global_event|onglobalevent|randomly_do|or_randomly_do)\b", Color.Blue); // DEFINE_SPOOLPOSITION|DEFINE_CITY|DEFINE_MOODID|DEFINE_CONSTANT|DEFINE_COUNTER|wait|onsuccess_enable|onsuccess_disable|onfailure_enable|onfailure_disable

            // command keywords (e.g., DEFINE_..., wait, etc.)
            HighlightWords(text, @"\b(DEFINE_SPOOLPOSITION|DEFINE_CITY|DEFINE_MOODID|DEFINE_CONSTANT|DEFINE_COUNTER|wait|onsuccess_enable|onsuccess_disable|onfailure_enable|onfailure_disable)\b", Color.SaddleBrown);

            // comments (e.g., // This is a comment)
            HighlightWords(text, @"//.*$", Color.MidnightBlue);

            // labels (e.g., :MAIN)
            HighlightWords(text, @"\:\w+", Color.Green);
            HighlightWords(text, @"\@\w+", Color.Green); // referenced labels (e.g., @MAIN)

            // opcodes (e.g., 0000:)
            HighlightWords(text, @"\b\d{4}:\b", Color.Black);

            // numbers (e.g., 123, 3.14)
            HighlightWords(text, @"\b\d+(\.\d+)?\b", Color.Brown);

            // hexadecimal numbers (e.g., 0xFF)
            HighlightWords(text, @"0x\w+", Color.Brown);

            // variables and pointers (e.g., $Tanner)
            HighlightWords(text, @"\$\w+", Color.DarkBlue);
        }

        private void HighlightWords(string text, string pattern, Color color)
        {
            Regex regex = new Regex(pattern, RegexOptions.Multiline);
            foreach (Match match in regex.Matches(text))
            {
                ScriptTB.Select(match.Index, match.Length);
                ScriptTB.SelectionColor = color;
            }
        }

        private void ScriptTB_TextChanged(object sender, EventArgs e)
        {
            int cursorPosition = ScriptTB.SelectionStart;

            // Suspend updates to prevent flickering
            ScriptTB.BeginUpdate();

            // Disable events to prevent recursion
            ScriptTB.TextChanged -= ScriptTB_TextChanged;

            // Store current text
            string text = ScriptTB.Text;

            // Clear formatting
            ScriptTB.SelectAll();
            ScriptTB.SelectionColor = Color.Black;

            // Apply syntax highlighting
            ApplyHighlighting(text);

            // Restore cursor position
            ScriptTB.SelectionStart = cursorPosition;
            ScriptTB.SelectionLength = 0;
            ScriptTB.SelectionColor = Color.Black;

            // Re-enable events
            ScriptTB.TextChanged += ScriptTB_TextChanged;

            // Resume updates
            ScriptTB.EndUpdate();
        }

        private void OpenBTN_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Open Driver 3 Mission Script file",
                Filter = "Driver 3 Mission Script (*.d3m)|*.d3m|Text files (*.txt)|*.txt",
                Multiselect = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ScriptFile = new FileStream(openFileDialog.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamReader reader = new StreamReader(ScriptFile);
                {
                    ScriptTB.Text = reader.ReadToEnd();
                }
            }
        }

        private void CompileBTN_Click(object sender, EventArgs e)
        {
            try
            {
                OutputTB.Text = $"Compiling ...";
                D3M_MissionScript d3m = new D3M_MissionScript();
                string scriptName = "untitled";
                if (ScriptFile != null)
                    scriptName = Path.GetFileName(ScriptFile.Name);
                d3m.CompileText(ScriptTB.Text, scriptName);

                // TODO: append/create mission package or modify mission package
                if (Main.MissionPackage != null)
                {
                    // copy data from lua mission to current mission
                    Main.MissionPackage.MissionData.LogicData.Actors.Definitions = d3m.ExportedMission.LogicData.Actors.Definitions;
                    Main.MissionPackage.MissionData.LogicData.Nodes.Definitions = d3m.ExportedMission.LogicData.Nodes.Definitions;
                    Main.MissionPackage.MissionData.LogicData.WireCollection.WireCollections = d3m.ExportedMission.LogicData.WireCollection.WireCollections;

                    Main.MissionPackage.MissionData.LogicData.ActorSetTable.Table = d3m.ExportedMission.LogicData.ActorSetTable.Table;

                    // mission instance data
                    if (Main.MissionPackage.MissionData.MissionInstances == null && d3m.ExportedMission.MissionInstances.Instances.Count != 0)
                    {
                        Main.MissionPackage.MissionData.MissionInstances = new MissionInstanceData();

                        Main.MissionPackage.MissionData.MissionInstances.Spooler = new DSCript.Spooling.SpoolableBuffer()
                        {
                            Context = (int)ChunkType.BuildingInstanceData,
                            Description = "Custom Mission Instances data",
                        };
                        // add the spooler to mission data
                        Main.MissionPackage.MissionData.Spooler.Children.Add(Main.MissionPackage.MissionData.MissionInstances.Spooler);
                    }
                    if (d3m.ExportedMission.MissionInstances.Instances.Count != 0)
                    {
                        Main.MissionPackage.MissionData.MissionInstances.Instances = d3m.ExportedMission.MissionInstances.Instances;
                    }

                    Main.MissionPackage.MissionData.Objects.Objects = d3m.ExportedMission.Objects.Objects;

                    Main.MissionPackage.MissionData.LogicData.StringCollection.Strings = d3m.ExportedMission.LogicData.StringCollection.Strings;

                    Main.MissionPackage.MissionData.LogicData.SoundBankTable.Table = d3m.ExportedMission.LogicData.SoundBankTable.Table;

                    if (Main.MissionPackage.MissionSummary == null)
                        Main.MissionPackage.MissionSummary = new MissionSummaryData();
                    Main.MissionPackage.MissionSummary.StartPosition = d3m.MissionSummary.StartPosition;
                    Main.MissionPackage.MissionSummary.CityType = MissionSummary.GetCityTypeByName(d3m.MissionSummary.Level);
                    Main.MissionPackage.MissionSummary.MissionId = d3m.MissionSummary.MoodId;
                }
                else
                {
                    string msg = "Please open any mission script in File >> Open, to compile.";
                    OutputTB.Text = msg;
                    MessageBox.Show(msg, "Missing mission script for output", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                OutputTB.Text = $"The mission script {scriptName} was compiled with success.\nCurrent loaded mission script was changed but not overwrited.";
            }
            catch (D3M_CompileException ex)
            {
                int lineStart = ScriptTB.GetFirstCharIndexFromLine(ex.ErrorLine);
                int nextLineStart = ScriptTB.GetFirstCharIndexFromLine(ex.ErrorLine + 1);
                int lineLength = (nextLineStart > 0 ? nextLineStart : ScriptTB.Text.Length) - lineStart;

                // select the error line
                ScriptTB.SelectionStart = lineStart;
                ScriptTB.SelectionLength = lineLength;
                OutputTB.Clear();

                OutputTB.Invalidate();
                ScriptTB.Invalidate();
                MessageBox.Show(ex.Message, "Compile Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        SaveFileDialog SaveDialog = new SaveFileDialog()
        {
            Title = "Save D3M file as new file or overwrite file",
            Filter = "Driver 3 Mission Script (*.d3m)|*.d3m|Text files (*.txt)|*.txt"
        };

        private void Save_BTN(object sender, EventArgs e)
        {
            if (ScriptFile == null)
            {
                if (SaveDialog.ShowDialog() == DialogResult.OK)
                {
                    ScriptFile = new FileStream(SaveDialog.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                }
                else
                    return;
            }

            using (StreamWriter sw = new StreamWriter(ScriptFile))
            {
                ScriptFile.SetLength(0);
                ScriptFile.Write(ScriptTB.Text);
            }
        }

        private void SaveAsIBTN_Click(object sender, EventArgs e)
        {
            if (SaveDialog.ShowDialog() == DialogResult.OK)
            {
                ScriptFile = new FileStream(SaveDialog.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                using (StreamWriter sw = new StreamWriter(ScriptFile))
                {
                    sw.Write(ScriptTB.Text);
                }
            }
        }

        private void UndoBTN_Click(object sender, EventArgs e)
        {
            if (ScriptTB.CanUndo)
                ScriptTB.Undo();
        }

        private void Redo_Click(object sender, EventArgs e)
        {
            if (ScriptTB.CanRedo)
                ScriptTB.Redo();
        }
    }

    public static class RichTextBoxExtensions
    {
        private const int WM_SETREDRAW = 0x0B;

        public static void BeginUpdate(this RichTextBox box)
        {
            SendMessage(box.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
        }

        public static void EndUpdate(this RichTextBox box)
        {
            SendMessage(box.Handle, WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
            box.Refresh();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    }
}
