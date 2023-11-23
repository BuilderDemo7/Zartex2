namespace Zartex._3D
{
    partial class viewport
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
            this.SplitContainer = new System.Windows.Forms.SplitContainer();
            this.viewportHost = new System.Windows.Forms.Integration.ElementHost();
            this.viewport3D = new Zartex.InspectorWidget3D();
            this.Inspector = new Zartex.InspectorWidget();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).BeginInit();
            this.SplitContainer.Panel1.SuspendLayout();
            this.SplitContainer.Panel2.SuspendLayout();
            this.SplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // SplitContainer
            // 
            this.SplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SplitContainer.BackColor = System.Drawing.SystemColors.Control;
            this.SplitContainer.Location = new System.Drawing.Point(0, 0);
            this.SplitContainer.Name = "SplitContainer";
            // 
            // SplitContainer.Panel1
            // 
            this.SplitContainer.Panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.SplitContainer.Panel1.Controls.Add(this.viewportHost);
            // 
            // SplitContainer.Panel2
            // 
            this.SplitContainer.Panel2.Controls.Add(this.Inspector);
            this.SplitContainer.Panel2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.SplitContainer.Size = new System.Drawing.Size(768, 375);
            this.SplitContainer.SplitterDistance = 250;
            this.SplitContainer.TabIndex = 1;
            // 
            // viewportHost
            // 
            this.viewportHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.viewportHost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.viewportHost.Location = new System.Drawing.Point(0, 0);
            this.viewportHost.Name = "viewportHost";
            this.viewportHost.Size = new System.Drawing.Size(247, 375);
            this.viewportHost.TabIndex = 0;
            this.viewportHost.Text = "elementHost1";
            this.viewportHost.Child = this.viewport3D;
            // 
            // Inspector
            // 
            this.Inspector.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Inspector.AutoSize = true;
            this.Inspector.BackColor = System.Drawing.Color.Transparent;
            this.Inspector.Location = new System.Drawing.Point(-1, 0);
            this.Inspector.Name = "Inspector";
            this.Inspector.Size = new System.Drawing.Size(515, 378);
            this.Inspector.TabIndex = 1;
            // 
            // viewport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.Controls.Add(this.SplitContainer);
            this.Name = "viewport";
            this.Size = new System.Drawing.Size(768, 376);
            this.SplitContainer.Panel1.ResumeLayout(false);
            this.SplitContainer.Panel2.ResumeLayout(false);
            this.SplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).EndInit();
            this.SplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.Integration.ElementHost viewportHost;
        public InspectorWidget3D viewport3D;
        public System.Windows.Forms.SplitContainer SplitContainer;
        public InspectorWidget Inspector;
    }
}
