namespace Zartex.ScriptEditor
{
    partial class D3M_ScriptEditor
    {
        /// <summary> 
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Designer de Componentes

        /// <summary> 
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.DetailedMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SplitContainer = new System.Windows.Forms.SplitContainer();
            this.LineNumPanel = new System.Windows.Forms.Panel();
            this.ScriptTB = new System.Windows.Forms.RichTextBox();
            this.OutputTB = new System.Windows.Forms.RichTextBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.OpenIBTN = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.NewFromTemplateIBTN = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.SaveIBTN = new System.Windows.Forms.ToolStripButton();
            this.SaveAsIBTN = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.CompileIBTN = new System.Windows.Forms.ToolStripButton();
            this.UndoIBTN = new System.Windows.Forms.ToolStripButton();
            this.RedoIBTN = new System.Windows.Forms.ToolStripButton();
            this.DetailedMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).BeginInit();
            this.SplitContainer.Panel1.SuspendLayout();
            this.SplitContainer.Panel2.SuspendLayout();
            this.SplitContainer.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // DetailedMenu
            // 
            this.DetailedMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.compileToolStripMenuItem});
            this.DetailedMenu.Location = new System.Drawing.Point(0, 0);
            this.DetailedMenu.Name = "DetailedMenu";
            this.DetailedMenu.Size = new System.Drawing.Size(560, 24);
            this.DetailedMenu.TabIndex = 1;
            this.DetailedMenu.Text = "menuStrip2";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // compileToolStripMenuItem
            // 
            this.compileToolStripMenuItem.Name = "compileToolStripMenuItem";
            this.compileToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
            this.compileToolStripMenuItem.Text = "Compile";
            // 
            // SplitContainer
            // 
            this.SplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SplitContainer.Location = new System.Drawing.Point(0, 48);
            this.SplitContainer.Name = "SplitContainer";
            this.SplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitContainer.Panel1
            // 
            this.SplitContainer.Panel1.Controls.Add(this.LineNumPanel);
            this.SplitContainer.Panel1.Controls.Add(this.ScriptTB);
            // 
            // SplitContainer.Panel2
            // 
            this.SplitContainer.Panel2.Controls.Add(this.OutputTB);
            this.SplitContainer.Size = new System.Drawing.Size(560, 417);
            this.SplitContainer.SplitterDistance = 314;
            this.SplitContainer.TabIndex = 2;
            // 
            // LineNumPanel
            // 
            this.LineNumPanel.BackColor = System.Drawing.Color.LightGray;
            this.LineNumPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.LineNumPanel.Location = new System.Drawing.Point(0, 0);
            this.LineNumPanel.Name = "LineNumPanel";
            this.LineNumPanel.Size = new System.Drawing.Size(47, 314);
            this.LineNumPanel.TabIndex = 1;
            this.LineNumPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.LinesPan_Paint);
            // 
            // ScriptTB
            // 
            this.ScriptTB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ScriptTB.Font = new System.Drawing.Font("Consolas", 9F);
            this.ScriptTB.Location = new System.Drawing.Point(53, 0);
            this.ScriptTB.Name = "ScriptTB";
            this.ScriptTB.Size = new System.Drawing.Size(504, 314);
            this.ScriptTB.TabIndex = 0;
            this.ScriptTB.Text = "";
            this.ScriptTB.TextChanged += new System.EventHandler(this.ScriptTB_TextChanged);
            // 
            // OutputTB
            // 
            this.OutputTB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputTB.Location = new System.Drawing.Point(3, 3);
            this.OutputTB.Name = "OutputTB";
            this.OutputTB.ReadOnly = true;
            this.OutputTB.Size = new System.Drawing.Size(554, 93);
            this.OutputTB.TabIndex = 0;
            this.OutputTB.Text = "";
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenIBTN,
            this.toolStripSeparator3,
            this.NewFromTemplateIBTN,
            this.toolStripButton1,
            this.toolStripSeparator2,
            this.SaveIBTN,
            this.SaveAsIBTN,
            this.toolStripSeparator1,
            this.CompileIBTN,
            this.UndoIBTN,
            this.RedoIBTN});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(560, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // OpenIBTN
            // 
            this.OpenIBTN.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.OpenIBTN.Image = global::Zartex.Properties.Resources.folder;
            this.OpenIBTN.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.OpenIBTN.Name = "OpenIBTN";
            this.OpenIBTN.Size = new System.Drawing.Size(23, 22);
            this.OpenIBTN.Text = "Open";
            this.OpenIBTN.ToolTipText = "Open a new file.";
            this.OpenIBTN.Click += new System.EventHandler(this.OpenBTN_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // NewFromTemplateIBTN
            // 
            this.NewFromTemplateIBTN.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.NewFromTemplateIBTN.Image = global::Zartex.Properties.Resources.page;
            this.NewFromTemplateIBTN.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.NewFromTemplateIBTN.Name = "NewFromTemplateIBTN";
            this.NewFromTemplateIBTN.Size = new System.Drawing.Size(23, 22);
            this.NewFromTemplateIBTN.Text = "New file (template)";
            this.NewFromTemplateIBTN.ToolTipText = "New file from a template.";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::Zartex.Properties.Resources.page_white;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "New file";
            this.toolStripButton1.ToolTipText = "New file from scratch.";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // SaveIBTN
            // 
            this.SaveIBTN.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SaveIBTN.Image = global::Zartex.Properties.Resources.disk;
            this.SaveIBTN.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SaveIBTN.Name = "SaveIBTN";
            this.SaveIBTN.Size = new System.Drawing.Size(23, 22);
            this.SaveIBTN.Text = "Save";
            this.SaveIBTN.ToolTipText = "Save current file.";
            this.SaveIBTN.Click += new System.EventHandler(this.Save_BTN);
            // 
            // SaveAsIBTN
            // 
            this.SaveAsIBTN.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SaveAsIBTN.Image = global::Zartex.Properties.Resources.disk_multiple;
            this.SaveAsIBTN.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SaveAsIBTN.Name = "SaveAsIBTN";
            this.SaveAsIBTN.Size = new System.Drawing.Size(23, 22);
            this.SaveAsIBTN.Text = "Save as";
            this.SaveAsIBTN.ToolTipText = "Save as a new file or overwrite file.";
            this.SaveAsIBTN.Click += new System.EventHandler(this.SaveAsIBTN_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // CompileIBTN
            // 
            this.CompileIBTN.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.CompileIBTN.Image = global::Zartex.Properties.Resources.compile;
            this.CompileIBTN.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.CompileIBTN.Name = "CompileIBTN";
            this.CompileIBTN.Size = new System.Drawing.Size(23, 22);
            this.CompileIBTN.Text = "Compile";
            this.CompileIBTN.ToolTipText = "Compile the current script into a MPC file.";
            this.CompileIBTN.Click += new System.EventHandler(this.CompileBTN_Click);
            // 
            // UndoIBTN
            // 
            this.UndoIBTN.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.UndoIBTN.Image = global::Zartex.Properties.Resources.arrow_undo;
            this.UndoIBTN.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.UndoIBTN.Name = "UndoIBTN";
            this.UndoIBTN.Size = new System.Drawing.Size(23, 22);
            this.UndoIBTN.Text = "Undo";
            this.UndoIBTN.Click += new System.EventHandler(this.UndoBTN_Click);
            // 
            // RedoIBTN
            // 
            this.RedoIBTN.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RedoIBTN.Image = global::Zartex.Properties.Resources.arrow_redo;
            this.RedoIBTN.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RedoIBTN.Name = "RedoIBTN";
            this.RedoIBTN.Size = new System.Drawing.Size(23, 22);
            this.RedoIBTN.Text = "Redo";
            this.RedoIBTN.Click += new System.EventHandler(this.Redo_Click);
            // 
            // D3M_ScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.SplitContainer);
            this.Controls.Add(this.DetailedMenu);
            this.Name = "D3M_ScriptEditor";
            this.Size = new System.Drawing.Size(560, 465);
            this.DetailedMenu.ResumeLayout(false);
            this.DetailedMenu.PerformLayout();
            this.SplitContainer.Panel1.ResumeLayout(false);
            this.SplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).EndInit();
            this.SplitContainer.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip DetailedMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compileToolStripMenuItem;
        private System.Windows.Forms.SplitContainer SplitContainer;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton OpenIBTN;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton NewFromTemplateIBTN;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton SaveIBTN;
        private System.Windows.Forms.ToolStripButton SaveAsIBTN;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton CompileIBTN;
        private System.Windows.Forms.RichTextBox ScriptTB;
        private System.Windows.Forms.Panel LineNumPanel;
        private System.Windows.Forms.RichTextBox OutputTB;
        private System.Windows.Forms.ToolStripButton UndoIBTN;
        private System.Windows.Forms.ToolStripButton RedoIBTN;
    }
}
